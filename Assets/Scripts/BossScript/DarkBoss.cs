
    using System.Collections;
    using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

    public class DarkBoss : MonoBehaviour
{
    [Header("Lightning Skill")]
    public GameObject lightningPrefab;
    public float lightningDelay = 1f;
    public float lightningDamage = 20f;
    [Header("Lightning Skill Settings")]
    [Range(0, 100)] public float lightningChancePercent = 20f; // Cơ hội % dùng skill
    public float lightningCheckInterval = 3f; // Thời gian giữa các lần kiểm tra
    private float lastLightningCheckTime = -10f; // Thời gian lần kiểm tra trước

    [Header("Boss Settings")]
    public float moveSpeed = 3f;
    public float attackRange = 2f;
    public float detectionRange = 10f;
    public float attackCooldown = 2f;
    public float attackDuration = 1f;
    public float wallHeightThreshold = 2.5f;   // Nếu player cao hơn ngưỡng này thì kéo dài tường
    public float extendedWallHeight = 6f;      // Chiều cao kéo dài

    [Header("Attack Collider")]
    public BoxCollider2D attackCollider;

    [Header("References")]
    public Transform player;
    private bool isCastingUltimate = false;

    [Header("Health")]
    public int maxHealth = 100;
    public HealthBar healthBar;
    public Player1 player1;
    public AuronPlayerController player2; 

    [Header("Ultimate Skill")]
    public GameObject wallPrefab;
    public Transform lightningSpawnY; // Empty ở trên trời để lấy Y cho tia sét
    public float wallOffsetX = 4f;
    public float ultimateDelay = 1.5f;

    [Header("Random Ulti")]
    [Range(0, 100)] public float ultimateChancePercent = 10f; // Tỷ lệ thi triển ultimate
    public float ultimateCheckInterval = 6f; // Khoảng thời gian giữa các lần kiểm tra
    private float lastUltimateCheckTime = -10f;

    private Animator animator;
    private Rigidbody2D rb;
    private int currentHealth;
    private float lastMeleeAttackTime = -10f;
    public bool isAttacking = false; // 👈 PUBLIC để AttackColliderTrigger có thể truy cập
    private bool facingRight = true;
    private bool hasHealthBarAppeared = false;

    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.gameObject.SetActive(false);
        }

        StartCoroutine(FindPlayerAfterDelay());
    }
    IEnumerator FindPlayerAfterDelay()
    {
        while (player == null || (player1 == null && player2 == null))
        {
            GameObject found = GameObject.FindGameObjectWithTag("Player");
            if (found != null)
            {
                player = found.transform;

                if (player1 == null)
                {
                    player1 = player.GetComponent<Player1>();
                    if (player1 == null)
                        player1 = player.GetComponentInChildren<Player1>();
                }

                if (player2 == null)
                {
                    player2 = player.GetComponent<AuronPlayerController>();
                    if (player2 == null)
                        player2 = player.GetComponentInChildren<AuronPlayerController>();
                }

                // If either player1 or player2 is found, break
                if (player1 != null || player2 != null)
                {
                    Debug.Log("✅ Player found and assigned.");
                    yield break;
                }
            }
            yield return null;
        }
    }

    void Update()
    {
        StartCoroutine(FindPlayerAfterDelay());
        if (player == null || currentHealth <= 0) return;

        float distanceToPlayer = Mathf.Abs(transform.position.x - player.position.x);

        // 👉 Nếu Player ra xa thì ẩn thanh máu như ForestBoss
        if (distanceToPlayer > detectionRange)
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("IsRunning", false);

            if (healthBar != null)
                healthBar.gameObject.SetActive(false);
            hasHealthBarAppeared = false;
            return;
        }

        FlipSprite();

        // 👉 Hiển thị thanh máu khi Player vào tầm
        if (!hasHealthBarAppeared && healthBar != null)
        {
            healthBar.gameObject.SetActive(true);
            hasHealthBarAppeared = true;
        }

        if (!isAttacking || isCastingUltimate)
        {
            if (distanceToPlayer <= attackRange && Time.time >= lastMeleeAttackTime + attackCooldown)
            {
                StartCoroutine(MeleeAttack());
            }
            else if (distanceToPlayer > attackRange)
            {
                MoveTowardsPlayer();
            }
        }

        // 👉 Kiểm tra random Lightning Strike mỗi X giây
        if (!isAttacking && !isCastingUltimate && Time.time >= lastLightningCheckTime + lightningCheckInterval)
        {
            lastLightningCheckTime = Time.time;

            float roll = Random.Range(0f, 100f);
            if (roll < lightningChancePercent)
            {
                StartCoroutine(CastLightningStrike());
                return; // Ưu tiên skill, không xử lý gì thêm frame này
            }
        }
        // 👉 Kiểm tra random Ultimate Skill mỗi X giây
        if (!isAttacking && !isCastingUltimate && Time.time >= lastUltimateCheckTime + ultimateCheckInterval)
        {
            lastUltimateCheckTime = Time.time;

            float roll = Random.Range(0f, 100f);
            if (roll < ultimateChancePercent)
            {
                StartCoroutine(UltimateSkill());
                return; // Ưu tiên thi triển skill
            }
        }



        if (Input.GetKeyDown(KeyCode.L))
        {
            StartCoroutine(CastLightningStrike());
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(50);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            StartCoroutine(UltimateSkill());
        }

    }

    IEnumerator CastLightningStrike()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero;

        // 1. Trigger animation giơ tay
        animator.SetTrigger("CastLightning");

        // 👉 GHI NHỚ vị trí player ngay lúc bắt đầu animation
        Vector3 targetPos = new Vector3(player.position.x, player.position.y - 4f, 0f);

        // 2. Đợi anim charge
        yield return new WaitForSeconds(lightningDelay);

        // 3. Tạo prefab tia sét tại vị trí đã ghi nhớ
        if (lightningPrefab != null)
        {
            GameObject lightning = Instantiate(lightningPrefab, targetPos, Quaternion.identity);

            LightningStrike strike = lightning.GetComponent<LightningStrike>();
            if (strike != null)
            {
                strike.damage = lightningDamage;
            }
        }

        // 4. Delay trước khi kết thúc skill
        yield return new WaitForSeconds(1f);

        // 5. Trở lại trạng thái bình thường
        float dist = Mathf.Abs(transform.position.x - player.position.x);
        if (dist <= attackRange)
            animator.SetBool("IsRunning", false); // Idle
        else
            animator.SetBool("IsRunning", true);  // Run

        isAttacking = false;
    }
    IEnumerator UltimateSkill()
    {
        isCastingUltimate = true;

        rb.velocity = Vector2.zero;
        animator.SetTrigger("CastLightning"); // Hoặc animation ultimate riêng

        Vector3 playerPos = player.position;

        // 1. Tạo 2 bức tường hai bên player với khoảng cách xa hơn
        float wallLowerOffsetY = 2f; // 👈 thêm dòng này để hạ thấp tường

        Vector3 leftWallPos = new Vector3(playerPos.x - wallOffsetX, playerPos.y - wallLowerOffsetY, 0f);
        Vector3 rightWallPos = new Vector3(playerPos.x + wallOffsetX, playerPos.y - wallLowerOffsetY, 0f);


        GameObject leftWall = null;
        GameObject rightWall = null;

        if (wallPrefab != null)
        {
            float currentWallHeight = wallPrefab.transform.localScale.y;
            bool shouldExtend = player.position.y > wallHeightThreshold;

            Vector3 scale = wallPrefab.transform.localScale;
            Vector3 offset = Vector3.zero;

            if (shouldExtend)
            {
                scale.y = extendedWallHeight;

                float extraHeight = extendedWallHeight - currentWallHeight;
                offset = new Vector3(0f, -extraHeight / 2f, 0f); // Dời xuống 1 nửa để vẫn chạm đất
            }

            leftWall = Instantiate(wallPrefab, leftWallPos + offset, Quaternion.identity);
            leftWall.transform.localScale = scale;

            rightWall = Instantiate(wallPrefab, rightWallPos + offset, Quaternion.identity);
            rightWall.transform.localScale = scale;
        }

        // 2. Đợi 1.5 giây (hiệu ứng + căng thẳng)
        yield return new WaitForSeconds(ultimateDelay);

        // 3. Gọi 5 tia sét rơi xuống player
        float startX = playerPos.x - 2f;
        float spacing = 1f;

        for (int i = 0; i < 5; i++)
        {
            float lightningY = Mathf.Max(lightningSpawnY.position.y, player.position.y - 10f);
            Vector3 strikePos = new Vector3(startX + i * spacing, lightningY, 0f);

            if (lightningPrefab != null)
            {
                GameObject lightning = Instantiate(lightningPrefab, strikePos, Quaternion.identity);
                LightningStrike strike = lightning.GetComponent<LightningStrike>();
                if (strike != null) strike.damage = lightningDamage;
            }
        }

        // 4. Đợi 2 giây trước khi phá tường và kết thúc ultimate
        yield return new WaitForSeconds(2f);

        // Phá tường nếu còn tồn tại
        if (leftWall != null) Destroy(leftWall);
        if (rightWall != null) Destroy(rightWall);

        // Đảm bảo animation và velocity reset
        rb.velocity = Vector2.zero;
        animator.SetBool("IsRunning", false);
        animator.SetBool("IsAttacking", false);

        // 5. Trở về trạng thái hoạt động
        isCastingUltimate = false;


    }


    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
        animator.SetBool("IsRunning", true);
    }

    IEnumerator MeleeAttack()
    {
        isAttacking = true;
        lastMeleeAttackTime = Time.time;

        rb.velocity = Vector2.zero;
        animator.SetBool("IsRunning", false);
        animator.SetBool("IsAttacking", true);

        yield return new WaitForSeconds(attackDuration);

        animator.SetBool("IsAttacking", false);
        isAttacking = false;
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        if (healthBar != null)
            healthBar.SetHealth(currentHealth);

        animator.SetTrigger("Hurt");
        Debug.Log("DarkBoss nhận sát thương: " + damage + ". Máu còn: " + currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log("💀 DarkBoss đã chết!");
        animator.SetTrigger("Die");

        if (healthBar != null)
            healthBar.gameObject.SetActive(false);

        isAttacking = true; // Ngừng mọi hành động
        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        if (player != null)
        {
            if (player1 != null)
                player1.Win();
            if (player2 != null)
                player2.Win();
            GameProgress.Current.CompleteLevel(3, GameManager.Instance.PlayTimeSeconds);
        }
        Destroy(gameObject, 2f);
    }

    void FlipSprite()
    {
        bool shouldFaceRight = player.position.x > transform.position.x;
        if (shouldFaceRight != facingRight)
        {
            facingRight = shouldFaceRight;
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null) sr.flipX = !facingRight;

            if (attackCollider != null)
            {
                Vector2 offset = attackCollider.offset;
                offset.x *= -1;
                attackCollider.offset = offset;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Vẽ vòng tròn phạm vi tấn công
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Vẽ vòng tròn phạm vi phát hiện
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Vẽ BoxCollider2D
        if (attackCollider != null)
        {
            Gizmos.color = Color.cyan;
            Vector3 colliderCenter = attackCollider.bounds.center;
            Vector3 colliderSize = attackCollider.bounds.size;
            Gizmos.DrawWireCube(colliderCenter, colliderSize);
        }
    }
}