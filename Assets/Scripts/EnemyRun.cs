﻿using System.Collections;
using TreeEditor;
using UnityEngine;

public class EnemyRun : MonoBehaviour
{
    public EnemyHealthBar healthbar;
    public int damage = 10;
    public float speed = 3.5f;
    public float verticalTolerance = 20f;
    public float attackRange = 10f;
    public float detectionRange = 25f;
    public float attackCooldown = 0.1f;
    public float attackDuration = 1f;
    public Transform colliderHolder;  // Kéo thả ColliderHolder từ Inspector
    public Transform attackCollider;  // Kéo thả AttackCollider từ Inspector
    public GameObject floatingText;
    private bool isDead = false;
    private Vector3 startPosition;
    public float patrolDistance = 5f;
    public float currentHeatlh;
    public float maxHealth = 50;
    private float lastAttackTime = -10f;
    private bool isAttacking = false;
    private Transform player;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool isPatrolling = true;
    private bool movingRight = true;

    public bool forceChase = true;
    public bool ignoreVerticalForAttack = true;

    // Cache để tránh tính toán liên tục
    private float cachedHorizontalDistance;
    private float cachedVerticalDistance;
    private bool canAttackNow = false;

    // Lưu offset gốc của BoxCollider
    private Vector2 originalColliderOffset;
    private Vector2 originalAttackColliderOffset;
    private BoxCollider2D boxCollider;
    private BoxCollider2D attackBoxCollider;

    // Thêm biến cho PolygonCollider2D
    private PolygonCollider2D attackPolygonCollider;
    private Vector2[] originalPolygonPoints;

    // Prefabs cho bowl máu và mana
    [SerializeField] private GameObject hpBowlPrefab;
    [SerializeField] private GameObject manaBowlPrefab;

    private void OnMouseDown()
    {
        TakeDamage(10);
        Debug.Log("Enemy nhận 10 dame");
    }

    void Start()
    {
        // Tìm player
        startPosition = transform.position;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            playerObj = GameObject.Find("Player");
        }

        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("❌ Không tìm thấy Player!");
            return;
        }

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Lưu offset gốc của BoxCollider
        if (colliderHolder != null)
        {
            boxCollider = colliderHolder.GetComponent<BoxCollider2D>();
            if (boxCollider != null)
            {
                originalColliderOffset = boxCollider.offset;
                Debug.Log($"💾 Saved original BoxCollider offset: {originalColliderOffset}");
            }
            else
            {
                Debug.LogError("❌ ColliderHolder không có BoxCollider2D component!");
            }
        }
        else
        {
            Debug.LogError("❌ ColliderHolder chưa được gán trong Inspector!");
        }

        // Kiểm tra và lưu thông tin cho AttackCollider
        if (attackCollider != null)
        {
            // Kiểm tra xem là BoxCollider2D hay PolygonCollider2D
            attackBoxCollider = attackCollider.GetComponent<BoxCollider2D>();
            attackPolygonCollider = attackCollider.GetComponent<PolygonCollider2D>();

            if (attackBoxCollider != null)
            {
                originalAttackColliderOffset = attackBoxCollider.offset;
            }
            else if (attackPolygonCollider != null)
            {
                // Lưu các điểm gốc của PolygonCollider
                originalPolygonPoints = new Vector2[attackPolygonCollider.points.Length];
                for (int i = 0; i < attackPolygonCollider.points.Length; i++)
                {
                    originalPolygonPoints[i] = attackPolygonCollider.points[i];
                }

            }
            else
            {
                Debug.LogError("❌ AttackCollider không có BoxCollider2D hoặc PolygonCollider2D component!");
            }
        }
        else
        {
            Debug.LogError("❌ AttackCollider chưa được gán trong Inspector!");
        }

        // Khởi tạo animation
        if (animator != null)
        {
            animator.Play("Run", 0, 0f);
        }

        currentHeatlh = maxHealth;
        //healthbar.updateHeathBar(currentHeatlh, maxHealth);
    }

    void Update()
    {
        if (player == null) return;
        if (isDead) return;

        // Nếu đang attack, chỉ cần chờ hết thời gian
        if (isAttacking)
        {
            if (Time.time >= lastAttackTime + attackDuration)
            {
                EndAttack();
            }
            return;
        }

        // Tính khoảng cách 1 lần
        UpdateDistances();

        // Kiểm tra có nên chase không
        bool shouldChase = (cachedVerticalDistance <= verticalTolerance) && (cachedHorizontalDistance <= detectionRange);

        if (shouldChase)
        {
            isPatrolling = false;
            HandleChase();
        }
        else
        {
            if (!isPatrolling)
            {
                isPatrolling = true;
                movingRight = true;
            }
            Patrol();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            Time.timeScale = 0.2f; // Làm chậm 5 lần
            Debug.Log("Slow motion ON");
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            Time.timeScale = 1f; // Trở lại bình thường
            Debug.Log("Slow motion OFF");
        }
        if (currentHeatlh <= 0 && !isDead)
        {
            Die();
        }
    }

    void UpdateDistances()
    {
        Vector3 playerPos = player.position;
        Vector3 myPos = transform.position;

        cachedHorizontalDistance = Mathf.Abs(myPos.x - playerPos.x);
        cachedVerticalDistance = Mathf.Abs(myPos.y - playerPos.y);

        // Kiểm tra điều kiện attack - SỬA LẠI LOGIC
        bool inHorizontalRange = cachedHorizontalDistance <= attackRange;
        bool inVerticalRange = ignoreVerticalForAttack || cachedVerticalDistance <= verticalTolerance;
        bool cooldownReady = Time.time >= lastAttackTime + attackCooldown;

        canAttackNow = inHorizontalRange && inVerticalRange && cooldownReady;


    }

    void HandleChase()
    {
        // KIỂM TRA ATTACK TRƯỚC KHI DI CHUYỂN
        if (canAttackNow)
        {

            StartAttack();
            return;
        }

        // Nếu trong attack range nhưng chưa attack được (do cooldown)
        if (cachedHorizontalDistance <= attackRange)
        {
            float remainingCooldown = (lastAttackTime + attackCooldown) - Time.time;
            FacePlayer();
            // KHÔNG RETURN - vẫn cho di chuyển gần hơn nếu cần
        }

        // Di chuyển về phía player (chỉ dừng khi đang attack thật sự)
        MoveTowardsPlayer();
    }

    void StartAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        if (animator != null)
        {
            animator.ResetTrigger("Attack");
            animator.SetTrigger("Attack");
        }

        FacePlayer();
    }

    void EndAttack()
    {
        isAttacking = false;

        if (animator != null)
        {
            animator.ResetTrigger("Attack");
            animator.Play("Run");
        }

    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        isAttacking = false;
        animator.ResetTrigger("Attack");
        animator.SetTrigger("Die");

        // Ngăn rơi khỏi đất
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static; // Hoặc FreezeAll
        }

        // Tùy chọn tắt collider tấn công
        if (attackBoxCollider != null) attackBoxCollider.enabled = false;
        if (attackPolygonCollider != null) attackPolygonCollider.enabled = false;

        // Đừng tắt collider chính nếu đang chạm đất
        // if (boxCollider != null) boxCollider.enabled = false;

        // Lọ hồi mana và máu spawn ra khi quái die
        // 🎯 Spawn bowl ngẫu nhiên (máu hoặc mana)
        // 🎯 Tỉ lệ rơi vật phẩm khi enemy chết
        float dropChance = Random.Range(0f, 1f);

        if (dropChance < 1f / 3f)
        {
            // 33.3% rơi máu
            Instantiate(hpBowlPrefab, transform.position, Quaternion.identity);
        }
        else if (dropChance < 2f / 3f)
        {
            // 33.3% rơi mana
            Instantiate(manaBowlPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            // 33.3% không rơi gì
            Debug.Log("Không rơi gì");
        }

        // 10% còn lại: không rơi gì


        // Delay hủy
        Destroy(gameObject, 1f);
    }


    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        Vector3 movement = (Vector3)(direction * speed * Time.deltaTime);
        transform.position += movement;

        // Flip sprite theo hướng di chuyển
        bool flip = direction.x < 0;
        spriteRenderer.flipX = flip;

        UpdateColliderFlip(flip);

    }

    void FacePlayer()
    {
        bool flip = player.position.x < transform.position.x;
        spriteRenderer.flipX = flip;

        // Cập nhật attack collider khi quay mặt
        UpdateColliderFlip(flip);
    }

    void Patrol()
    {
        float dir = movingRight ? 1f : -1f;
        transform.Translate(Vector2.right * dir * speed * Time.deltaTime);

        bool flip = !movingRight;
        spriteRenderer.flipX = flip;

        // Cập nhật collider khi patrol
        UpdateColliderFlip(flip);

        float distanceFromStart = transform.position.x - startPosition.x;
        float buffer = 0.5f; // Tăng giới hạn thêm một tí

        if (movingRight && distanceFromStart >= patrolDistance + buffer)
        {
            movingRight = false;
        }
        else if (!movingRight && distanceFromStart <= -patrolDistance - buffer)
        {
            movingRight = true;
        }
    }

    // Hàm để lật BoxCollider của colliderHolder và attackCollider theo sprite
    void UpdateColliderFlip(bool isFlipped)
    {
        // Lật colliderHolder (BoxCollider2D)
        if (boxCollider != null)
        {
            Vector2 newOffset = originalColliderOffset;

            if (isFlipped)
            {
                newOffset.x = originalColliderOffset.x * -1f;
            }

            boxCollider.offset = newOffset;
        }

        // Xử lý AttackCollider tùy theo loại
        if (attackCollider != null)
        {
            if (attackBoxCollider != null)
            {
                // Nếu là BoxCollider2D - dùng localScale như cũ
                Vector3 newScale = attackCollider.localScale;
                newScale.x = Mathf.Abs(newScale.x) * (isFlipped ? -1f : 1f);
                attackCollider.localScale = newScale;

            }
            else if (attackPolygonCollider != null && originalPolygonPoints != null)
            {
                // Nếu là PolygonCollider2D - lật các điểm theo trục X
                Vector2[] flippedPoints = new Vector2[originalPolygonPoints.Length];

                for (int i = 0; i < originalPolygonPoints.Length; i++)
                {
                    flippedPoints[i] = originalPolygonPoints[i];

                    if (isFlipped)
                    {
                        // Lật điểm theo trục X (đảo dấu x)
                        flippedPoints[i].x = -originalPolygonPoints[i].x;
                    }
                }

                attackPolygonCollider.points = flippedPoints;
            }
        }
    }

    // Debug đơn giản hơn
    void OnGUI()
    {
        if (player == null) return;

        //GUI.Label(new Rect(10, 10, 300, 20), $"Distance: {cachedHorizontalDistance:F2}");
        //GUI.Label(new Rect(10, 30, 300, 20), $"Can Attack: {canAttackNow}");
        //GUI.Label(new Rect(10, 50, 300, 20), $"Is Attacking: {isAttacking}");
        //GUI.Label(new Rect(10, 70, 300, 20), $"Cooldown: {(lastAttackTime + attackCooldown - Time.time):F1}s");
        //GUI.Label(new Rect(10, 90, 300, 20), $"Time: {Time.time:F1}s");
    }

    public void TakeDamage(int amount)
    {
        ShowDame(amount.ToString());
        animator.SetTrigger("Hurt");

        isAttacking = false;
        currentHeatlh -= amount;
        Debug.Log("💔 Enemy bị đánh, máu còn: " + currentHeatlh);

        if (currentHeatlh <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(PlayHurtAndRecover());
        }
    }

    IEnumerator PlayHurtAndRecover()
    {
        float originalSpeed = speed;
        speed = 0;

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        speed = originalSpeed;
        // Let the Update() method handle setting movement and animation
    }





    void ShowDame(string text)
    {
        if (floatingText)
        {
            GameObject prefab = Instantiate(floatingText, transform.position, Quaternion.identity);

            // Ép lại vị trí z để không bị ẩn
            Vector3 fixedPos = prefab.transform.position;
            fixedPos.z = -1; // hoặc 0
            prefab.transform.position = fixedPos;

            prefab.GetComponentInChildren<TextMesh>().text = text;

            // 💥 Thêm dòng này để hủy sau 1 giây
            Destroy(prefab, 0.8f);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Attack range (đỏ)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Detection range (vàng)  
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Line đến player
        if (player != null)
        {
            Gizmos.color = canAttackNow ? Color.red : Color.cyan;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
    public void ApplyKnockback(Vector2 force)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = new Vector2(force.x, rb.velocity.y);
        }
    }




}