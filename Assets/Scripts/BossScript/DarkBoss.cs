// DarkBoss.cs - Sửa theo ForestBoss logic
using System.Collections;
using UnityEngine;

public class DarkBoss : MonoBehaviour
{
    [Header("Lightning Skill")]
    public GameObject lightningPrefab;
    public float lightningDelay = 1f;
    public float lightningDamage = 20f;

    [Header("Boss Settings")]
    public float moveSpeed = 3f;
    public float attackRange = 2f;
    public float detectionRange = 10f;
    public float attackCooldown = 2f;
    public float attackDuration = 1f;

    [Header("Attack Collider")]
    public BoxCollider2D attackCollider;

    [Header("References")]
    public Transform player;

    [Header("Health")]
    public int maxHealth = 100;
    public HealthBar healthBar;

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

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    void Update()
    {
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

        if (!isAttacking)
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

        // Lightning skill logic
        if (!isAttacking && Vector2.Distance(transform.position, player.position) < 8f && Random.value < 0.2f)
        {
            StartCoroutine(CastLightningStrike());
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            StartCoroutine(CastLightningStrike());
        }
    }

    IEnumerator CastLightningStrike()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero;

        // 1. Trigger animation giơ tay
        animator.SetTrigger("CastLightning");

        // 2. Đợi anim charge
        yield return new WaitForSeconds(lightningDelay);

        // 3. Tạo prefab tia sét tại vị trí Player
        if (player != null && lightningPrefab != null)
        {
            Vector3 spawnPos = new Vector3(player.position.x, player.position.y + 1f, 0f);
            GameObject lightning = Instantiate(lightningPrefab, spawnPos, Quaternion.identity);

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
            animator.SetTrigger("Idle");
        else
            animator.SetBool("IsRunning", true);

        isAttacking = false;
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