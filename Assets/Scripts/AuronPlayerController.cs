using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuronPlayerController : MonoBehaviour
{
    private Animator animator;
    public float moveSpeed = 3f;
    public GameObject arrowPrefab; 
    public Transform firePoint;    // Vị trí xuất phát mũi tên
    public float arrowForce = 10f; // Lực bắn mũi tên
    public float fireRate = 0.5f;  // Thời gian giữa các lần bắn

    private Rigidbody2D rb;

    public float jumpForce = 7f;
    private bool isGrounded = true;
    private bool isAttacking = false;
    private bool isDefending = false; 

    public GameObject arrowFallEffectPrefab; // Prefab hiệu ứng cung rơi
    public Transform arrowFallSpawnPoint;    // Vị trí rơi xuống (có thể là ground hoặc vị trí chỉ định)

    public Player1Healthbar healthBar;
    public Player1MPBar MPBar;
    public int maxHealth = 100;
    public int currentHealth;
    public int maxMP = 100;
    public int currentMP;
    public int eSkillMPCost = 20;
    public float mpRegenRate = 5f;
    private float mpRegenTimer = 0f;
    public int damage = 10;
    private SpriteRenderer spriteRenderer;
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // Thêm dòng này

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


        // Đòn đánh tay (ví dụ phím X)
        if (Input.GetKeyDown(KeyCode.X))
        {
            isAttacking = true;
            animator.SetBool("IsAttacking", true);
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



        // Di chuyển bằng Rigidbody2D (chuẩn vật lý)
        rb.velocity = new Vector2(movement.x * moveSpeed, rb.velocity.y);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
            animator.SetBool("IsJumping", true);
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
            animator.SetBool("IsJumping", false);

            // Cập nhật IsMoving để đảm bảo chuyển trạng thái đúng sau khi tiếp đất
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            bool isMoving = (new Vector3(horizontal, 0, vertical)).sqrMagnitude > 0f;
            animator.SetBool("IsMoving", isMoving);
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

        float direction = spriteRenderer.flipX ? -1f : 1f;
        Vector2 shootDir = new Vector2(direction, 0f);

        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        arrow.GetComponent<Arrow>().damage = damage;
        Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();
        arrowRb.velocity = shootDir * arrowForce;

        // Xoay mũi tên cho đúng hướng
        float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.Euler(0, 0, angle);

        // Tăng kích thước arrow (không lật scale X)
        float scaleMultiplier = 5f;
        arrow.transform.localScale = new Vector3(scaleMultiplier, scaleMultiplier, 1f);

        Debug.Log("🚀 Arrow bắn ra hướng: " + shootDir);
        Destroy(arrow, 2f);
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

            // RaycastAll từ trên xuống để tìm ground cao nhất
            Vector2 rayOrigin = new Vector2(mouseWorldPos.x, transform.position.y + 10f);
            RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, Vector2.down, 30f, LayerMask.GetMask("Ground", "Tilemap"));

            Vector3 spawnPos = mouseWorldPos;
            if (hits.Length > 0)
            {
                // Lấy ground cao nhất (y lớn nhất)
                float maxY = float.MinValue;
                foreach (var h in hits)
                {
                    if (h.point.y > maxY)
                    {
                        maxY = h.point.y;
                        spawnPos = h.point;
                    }
                }
                spawnPos.z = 0f;
                spawnPos.y += 0.6f; // Offset nhỏ để không bị chìm
            }
            else
            {
                // Nếu không trúng ground, mặc định spawn ở y của player
                spawnPos.y = transform.position.y;
            }

            float dirToMouse = mouseWorldPos.x - transform.position.x;
            bool isFacingRight = !spriteRenderer.flipX;

            if ((isFacingRight && dirToMouse >= 0) || (!isFacingRight && dirToMouse <= 0))
            {
                GameObject effect = Instantiate(arrowFallEffectPrefab, spawnPos, Quaternion.identity);
                effect.transform.localScale *= 5f;
                Destroy(effect, 1f);
            }
        }
    }



}


