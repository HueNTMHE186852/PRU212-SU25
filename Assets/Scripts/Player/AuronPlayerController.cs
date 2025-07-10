using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuronPlayerController : MonoBehaviour
{
    private Animator animator;
    public float moveSpeed = 25f;
    public GameObject arrowPrefab;
    public Transform firePoint;    // Vị trí xuất phát mũi tên
    public float arrowForce = 70f; // Lực bắn mũi tên
    public float fireRate = 0.8f;  // Thời gian giữa các lần bắn

    private Rigidbody2D rb;

    public float jumpForce = 25f;
    public Transform groundCheckPoint;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded = true;
    private bool isAttacking = false;
    private bool isDefending = false;
    private int jumpCount = 0;
    public int maxJumpCount = 2; // Allow double jump
    private bool isFalling = false;


    public GameObject arrowFallEffectPrefab; // Prefab hiệu ứng cung rơi
    public Transform arrowFallSpawnPoint;    // Vị trí rơi xuống (có thể là ground hoặc vị trí chỉ định)

    public Player1Healthbar healthBar;
    public Player1MPBar MPBar;
    public Player1Coin coinManager;

    public int maxHealth = 1000;
    public int currentHealth;
    public int maxMP = 200;
    public int currentMP;
    public int eSkillMPCost = 20;
    public int qSkillMPCost = 30;
    public float mpRegenRate = 5f;
    private float mpRegenTimer = 0f;
    public int damage = 10;
    private SpriteRenderer spriteRenderer;

    public float slideSpeed = 18f;
    public float slideDuration = 1f;
    private bool isSliding = false;
    private float slideTimer = 0f;

    public float rollSpeed = 18f;
    public float rollDuration = 0.5f;
    private bool isRolling = false;
    private float rollTimer = 0f;

    public GameObject explosionEffectPrefab; // Gán trong Inspector

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // Thêm dòng này
        currentHealth = maxHealth;
        currentMP = 100;
        if (healthBar != null)
        {
            healthBar.SetMaxHealth();
            healthBar.gameObject.SetActive(true);
        }
        if (MPBar != null)
        {
            MPBar.SetMaxMP();
            MPBar.gameObject.SetActive(true);
        }

    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(horizontal, vertical).normalized;
        bool isMoving = movement.sqrMagnitude > 0f;
        animator.SetBool("IsMoving", isMoving);

        if (Input.GetKeyDown(KeyCode.X))
        {
            isAttacking = true;
            animator.SetBool("IsAttacking", true);

            float attackRadius = 5f;
            float attackOffsetX = 1.0f;
            Vector3 attackCenter = transform.position + new Vector3(spriteRenderer.flipX ? -attackOffsetX : attackOffsetX, 0, 0);

            Collider2D[] hits = Physics2D.OverlapCircleAll(attackCenter, attackRadius);
            Debug.Log("Attack hits: " + hits.Length);
            foreach (var hit in hits)
            {
                EnemyRun enemy = hit.GetComponent<EnemyRun>();
                if (enemy == null)
                    enemy = hit.GetComponentInParent<EnemyRun>();

                if (enemy != null && enemy.gameObject.CompareTag("Enemy"))
                {
                    Debug.Log("Gây damage lên: " + enemy.gameObject.name);
                    enemy.TakeDamage(damage);
                }
            }
        }

        else if (Input.GetKeyUp(KeyCode.X))
        {
            isAttacking = false;
            animator.SetBool("IsAttacking", false);
        }


        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("SetTrigger BowShoot");
            animator.SetTrigger("BowShoot");

        }
        if (Input.GetKeyDown(KeyCode.E) && currentMP >= eSkillMPCost)
        {
            currentMP -= eSkillMPCost;
            if (MPBar != null)
                MPBar.SetMP((float)currentMP / maxMP);
            Debug.Log("SetTrigger SkillAttack");
            animator.SetTrigger("IsAttacking2");

        }
        if (Input.GetKeyDown(KeyCode.Q) && currentMP >= qSkillMPCost)
        {
            currentMP -= qSkillMPCost;
            if (MPBar != null)
                MPBar.SetMP((float)currentMP / maxMP);
            animator.SetTrigger("BowShootQ");
        }

        // Defend (hold right mouse button)
        isDefending = Input.GetMouseButton(1);
        animator.SetBool("IsDefending", isDefending);
        bool wasFlipped = spriteRenderer.flipX;
        if (movement.x < 0 && !wasFlipped)
        {
            spriteRenderer.flipX = true;

        }
        else if (movement.x > 0 && wasFlipped)
        {
            spriteRenderer.flipX = false;

        }
        // Slide input (LeftShift)
        if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded && !isSliding)
        {
            isSliding = true;
            slideTimer = 0f;
            animator.SetTrigger("Slide");
        }

        // Roll input (phím C)
        if (Input.GetKeyDown(KeyCode.LeftControl) && isGrounded && !isRolling && !isSliding)
        {
            isRolling = true;
            rollTimer = 0f;
            animator.SetTrigger("Roll");
        }
        if (isRolling)
        {
            rollTimer += Time.deltaTime;
            float rollDirection = spriteRenderer.flipX ? -1f : 1f;
            rb.velocity = new Vector2(rollDirection * rollSpeed, rb.velocity.y);

            // Không cho nhảy hoặc tấn công khi đang roll
            animator.SetBool("IsAttacking", false);
            animator.SetBool("IsJumping", false);

            if (rollTimer >= rollDuration)
            {
                isRolling = false;
            }
        }
        else if (isSliding)
        {
            slideTimer += Time.deltaTime;
            float slideDirection = spriteRenderer.flipX ? -1f : 1f;
            rb.velocity = new Vector2(slideDirection * slideSpeed, rb.velocity.y);

            animator.SetBool("IsAttacking", false);
            animator.SetBool("IsJumping", false);

            if (slideTimer >= slideDuration)
            {
                isSliding = false;
            }
        }
        else
        {
            // Di chuyển bình thường
            rb.velocity = new Vector2(movement.x * moveSpeed, rb.velocity.y);
        }



        // Di chuyển bằng Rigidbody2D (chuẩn vật lý)
        rb.velocity = new Vector2(movement.x * moveSpeed, rb.velocity.y);
        // Check if grounded
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            jumpCount = 0;
            isFalling = false;
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsFalling", false);
        }

        // Handle jump input
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumpCount)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f); // Reset vertical velocity for consistent jump
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpCount++;

            animator.SetBool("IsJumping", true);
            animator.SetBool("IsFalling", false);
            animator.Play("jump_up"); // Play your custom jump up animation
        }

        // Detect falling
        if (rb.velocity.y < -0.1f && !isGrounded)
        {
            isFalling = true;
            animator.SetBool("IsFalling", true);
            animator.SetBool("IsJumping", false);
            animator.Play("jump_down"); // Play your custom jump down animation
        }
        else if (rb.velocity.y >= -0.1f && !isGrounded)
        {
            animator.SetBool("IsJumping", true);
            animator.SetBool("IsFalling", false);
        }

        mpRegenTimer += Time.deltaTime;
        if (mpRegenTimer >= 1f)
        {
            mpRegenTimer = 0f;
            currentMP = Mathf.Min(currentMP + (int)mpRegenRate, maxMP);
            if (MPBar != null)
                MPBar.SetMP((float)currentMP / maxMP);
        }

    }
    public void TakeDamage(int damage)
    {
        if (isDefending)
        {
            Debug.Log("Blocked damage while defending!");
            return;
        }
        animator.SetTrigger("Hit"); // Gọi animation nhận damage

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log($"Current Health: {currentHealth}/{maxHealth}");
        if (healthBar != null)
        {
            healthBar.SetHealth((float)currentHealth / maxHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Die")) return;
        animator.speed = 0.7f;
        animator.SetTrigger("Die");
        isAttacking = false;

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }

        Destroy(gameObject, 2f);
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Tilemap"))
        {
            isGrounded = true;
            jumpCount = 0;
            isFalling = false;
            if (animator != null)
            {
                animator.SetBool("IsJumping", false);
                animator.SetBool("IsFalling", false);

                // Update IsMoving as before
                float horizontal = Input.GetAxisRaw("Horizontal");
                float vertical = Input.GetAxisRaw("Vertical");
                bool isMoving = (new Vector3(horizontal, 0, vertical)).sqrMagnitude > 0f;
                animator.SetBool("IsMoving", isMoving);
            }
            else
            {
                Debug.LogWarning("Animator is null in OnCollisionEnter2D!");
            }
        }
    }


    public void EndAttack()
    {
        Debug.Log("EndAttack called");
        isAttacking = false;
        animator.SetBool("IsAttacking", false);
    }
    public void FireArrow()
    {
        Debug.Log("✅ FireArrow() được gọi!");

        // Lấy vị trí chuột trên màn hình và chuyển sang tọa độ thế giới
        Vector3 mouseScreenPos = Input.mousePosition;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        // Xác định hướng mặt nhân vật theo chuột
        if (mouseWorldPos.x < transform.position.x)
            spriteRenderer.flipX = true;
        else
            spriteRenderer.flipX = false;

        // Đảo vị trí firePoint theo flipX
        Vector3 firePointLocalPos = firePoint.localPosition;
        firePointLocalPos.x = Mathf.Abs(firePointLocalPos.x) * (spriteRenderer.flipX ? -1 : 1);
        firePoint.localPosition = firePointLocalPos;

        // Tính hướng bắn từ firePoint đến vị trí chuột
        Vector2 shootDir = (mouseWorldPos - firePoint.position).normalized;

        // Xoay firePoint theo hướng bắn
        float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;
        firePoint.rotation = Quaternion.Euler(0, 0, angle);

        // Tạo mũi tên
        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.Euler(0, 0, angle));
        arrow.GetComponent<Arrow>().damage = damage;
        Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();
        arrowRb.velocity = shootDir * arrowForce;
        Debug.Log("Arrow velocity: " + arrowRb.velocity);
        // Tăng kích thước arrow (không lật scale X)
        float scaleMultiplier = 11f;
        arrow.transform.localScale = new Vector3(scaleMultiplier, scaleMultiplier, 1f);

        Debug.Log("🚀 Arrow bắn ra hướng: " + shootDir);
        Destroy(arrow, 1f);
    }



    public void TestEvent()
    {
        Debug.Log("✅ TestEvent() được gọi!");
    }
    public void SpawnArrowFallEffect()
    {
        if (arrowFallEffectPrefab != null)
        {
            Vector3 mouseScreenPos = Input.mousePosition;
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            mouseWorldPos.z = 0f;

            // RaycastAll từ trên xuống để tìm ground dưới chân player
            Vector2 rayOrigin = new Vector2(mouseWorldPos.x, transform.position.y + 10f);
            RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, Vector2.down, 30f, LayerMask.GetMask("Ground", "Tilemap"));

            Vector3 spawnPos = mouseWorldPos;
            bool foundGroundBelow = false;
            float maxY = float.MinValue;

            if (hits.Length > 0)
            {
                foreach (var h in hits)
                {
                    // Chỉ lấy ground dưới hoặc ngang chân player
                    if (h.point.y <= groundCheckPoint.position.y && h.point.y > maxY)
                    {
                        maxY = h.point.y;
                        spawnPos = h.point;
                        foundGroundBelow = true;
                    }
                }
                if (foundGroundBelow)
                {
                    spawnPos.z = 0f;
                    spawnPos.y += 0.6f;
                }
            }

            if (!foundGroundBelow)
            {
                float mouseToPlayerX = Mathf.Abs(mouseWorldPos.x - transform.position.x);

                if (isGrounded && groundCheckPoint != null && mouseToPlayerX < 1.0f)
                {
                    // Nếu chuột gần player, spawn tại groundCheckPoint
                    spawnPos = groundCheckPoint.position;
                    spawnPos.z = 0f;
                    spawnPos.y += 0.6f;
                }
                else
                {
                    // Nếu chuột xa player, spawn ở vị trí chuột nhưng không cao hơn chân player
                    spawnPos.y = Mathf.Min(mouseWorldPos.y, groundCheckPoint.position.y + 0.6f);
                    spawnPos.z = 0f;
                }
            }

            float dirToMouse = mouseWorldPos.x - transform.position.x;
            bool isFacingRight = !spriteRenderer.flipX;

            if ((isFacingRight && dirToMouse >= 0) || (!isFacingRight && dirToMouse <= 0))
            {
                GameObject effect = Instantiate(arrowFallEffectPrefab, spawnPos, Quaternion.identity);
                effect.transform.localScale *= 5f;
                Destroy(effect, 1f);

                // Gây damage cho enemy trong vùng spawn
                float fallRadius = 3f; // bán kính vùng gây damage, chỉnh theo ý bạn
                int fallDamage = damage * 2; // damage, có thể chỉnh theo ý bạn

                Collider2D[] enemies = Physics2D.OverlapCircleAll(spawnPos, fallRadius);
                HashSet<EnemyRun> damagedEnemies = new HashSet<EnemyRun>();
                foreach (var col in enemies)
                {
                    EnemyRun enemy = col.GetComponent<EnemyRun>();
                    if (enemy == null)
                        enemy = col.GetComponentInParent<EnemyRun>();

                    if (enemy != null && enemy.gameObject.CompareTag("Enemy") && !damagedEnemies.Contains(enemy))
                    {
                        Debug.Log("ArrowFallEffect gây damage lên: " + enemy.gameObject.name);
                        enemy.TakeDamage(fallDamage);
                        damagedEnemies.Add(enemy);
                    }
                }
            }
        }
    }
    public void FireQSkillArrow()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        if (mouseWorldPos.x < transform.position.x)
            spriteRenderer.flipX = true;
        else
            spriteRenderer.flipX = false;

        Vector3 firePointLocalPos = firePoint.localPosition;
        firePointLocalPos.x = Mathf.Abs(firePointLocalPos.x) * (spriteRenderer.flipX ? -1 : 1);
        firePoint.localPosition = firePointLocalPos;

        Vector2 shootDir = (mouseWorldPos - firePoint.position).normalized;
        float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;
        firePoint.rotation = Quaternion.Euler(0, 0, angle);

        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.Euler(0, 0, angle));
        Arrow arrowScript = arrow.GetComponent<Arrow>();
        arrowScript.damage = 15;
        arrowScript.explosionEffectPrefab = explosionEffectPrefab; // Gán hiệu ứng nổ
        arrowScript.isQSkillArrow = true; // Đánh dấu là mũi tên Q

        Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();
        arrowRb.velocity = shootDir * arrowForce;

        float scaleMultiplier = 11f;
        arrow.transform.localScale = new Vector3(scaleMultiplier, scaleMultiplier, 1f);

        Destroy(arrow, 1f);
    }




}


