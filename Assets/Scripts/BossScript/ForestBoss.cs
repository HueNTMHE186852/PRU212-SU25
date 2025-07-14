using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestBoss : MonoBehaviour
{
    [Header("Boss Settings")]
    public float moveSpeed = 3f;
    public float attackRange = 2f;
    public float detectionRange = 10f;
    public float attackCooldown = 2f;
    public float attackDuration = 1f;

    [Header("FlyDash Settings")]
    public float chaseDurationBeforeDash = 3f;
    public float flyDashSpeed = 8f;
    public float flyDashStopDistance = 2.5f;
    public float flyDashCooldown = 5f;
    public float flyDashPrepareTime = 0.5f;
    private Collider2D bossCollider;
    private bool originalIsTrigger;

    [Header("Tornado Random Settings")]
    public float tornadoInterval = 8f; // thời gian giữa 2 lần tornado tối thiểu
    private float nextTornadoTime = 0f;

    [Header("Tornado Damage Settings")]
    public int tornadoDamage = 10;
    public float tornadoDamageInterval = 0.5f;
    private Dictionary<Collider2D, float> nextDamageTimeMap = new Dictionary<Collider2D, float>();

    [Header("Health Bar")]
    public HealthBar healthBar;
    private bool hasHealthBarAppeared = false;


    [Header("Tornado Skill Settings")]
    public float tornadoSpeed = 30f;
    public float tornadoDuration = 3f;
    private float lastTornadoTime = -6f;
    public float tornadoCooldown = 6f;

    private bool isInTornadoMode = false;
    private Vector3 tornadoPointA;
    private Vector3 tornadoPointB;
    private float tornadoEndTime;
    private Vector3 tornadoCurrentTarget;

    [Header("Rain Attack Settings")]
    public float rangedAttackInterval = 5f;
    public int rainProjectileCount = 8;
    public float rainWidth = 5f;
    public float rainHeight = 10f;

    [Header("Attack Collider")]
    public BoxCollider2D attackCollider;

    [Header("References")]
    public Transform player;
    public GameObject projectilePrefab;

    [Header("Health")]
    public int maxHealth = 100;
    private int currentHealth;

    private Animator animator;
    private Rigidbody2D rb;
    private bool facingRight = true;
    private bool isAttacking = false;
    private bool isFlyDashing = false;
    private bool isPreparingFlyDash = false;
    private float nextRangedAttackTime = 0f;
    private float lastMeleeAttackTime = -10f;
    private float lastFlyDashTime = -10f;
    private float chaseStartTime = 0f;
    private bool isChasing = false;

    private float cachedHorizontalDistance;
    private Vector3 flyDashTargetPosition;

    private Coroutine currentFlyDashCoroutine = null;

    void Start()
    {
        bossCollider = GetComponent<Collider2D>();
        if (bossCollider != null)
            originalIsTrigger = bossCollider.isTrigger;
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.gameObject.SetActive(false);
        }

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth); // tránh âm

        if (healthBar != null)
            healthBar.SetHealth(currentHealth); // ✅ gọi sau khi trừ

        Debug.Log("💥 Boss nhận " + damage + " sát thương. Máu còn: " + currentHealth);
        animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log("💀 Boss đã chết!");
        animator.SetTrigger("Die");
        if (healthBar != null)
            healthBar.gameObject.SetActive(false);

        isAttacking = true;
        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Destroy(gameObject, 2f);
    }

    void Update()
    {
        if (player == null || currentHealth <= 0) return;

        cachedHorizontalDistance = Mathf.Abs(transform.position.x - player.position.x);

        if (cachedHorizontalDistance > detectionRange)
        {
            if (isInTornadoMode)
            {
                Debug.Log("🌀 Player đã ra khỏi vùng — dừng lốc xoáy!");
                EndTornadoSkill();
            }
            rb.velocity = Vector2.zero;
            animator.SetBool("IsRunning", false);
            isChasing = false;

            // 👉 Ẩn thanh máu khi Player ra xa
            if (healthBar != null)
                healthBar.gameObject.SetActive(false);

            hasHealthBarAppeared = false; // cho phép hiển thị lại sau này
            return;
        }

        // ⛔ Nếu khác tầng thì không đuổi
        if (!IsSameLevelAsPlayer())
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("IsRunning", false);
            isChasing = false;

            // 👉 Ẩn thanh máu khi Player không cùng tầng
            if (healthBar != null)
                healthBar.gameObject.SetActive(false);

            hasHealthBarAppeared = false;
            return;
        }

        FlipSprite();

        if (!hasHealthBarAppeared)
        {
            hasHealthBarAppeared = true;
            if (healthBar != null)
                healthBar.gameObject.SetActive(true);
        }

        if (!isAttacking && !isFlyDashing && !isPreparingFlyDash)
        {
            if (cachedHorizontalDistance <= attackRange && Time.time >= lastMeleeAttackTime + attackCooldown)
            {
                rb.velocity = Vector2.zero;
                animator.SetBool("IsRunning", false);
                isChasing = false;
                StartCoroutine(MeleeAttack());
            }
            else if (cachedHorizontalDistance > attackRange)
            {
                if (ShouldFlyDash())
                {
                    currentFlyDashCoroutine = StartCoroutine(FlyDash());
                }
                else
                {
                    MoveTowardsPlayer();

                    if (!isChasing)
                    {
                        isChasing = true;
                        chaseStartTime = Time.time;
                    }

                    if (Time.time >= nextRangedAttackTime)
                    {
                        nextRangedAttackTime = Time.time + rangedAttackInterval;
                        StartCoroutine(RainShot());
                    }
                }
            }
        }

        // Nếu không đang tấn công hoặc dash và đang trong khoảng cách phát hiện
        if (!isAttacking && !isFlyDashing && !isPreparingFlyDash && !isInTornadoMode)
        {
            if (Time.time >= nextTornadoTime && cachedHorizontalDistance <= detectionRange)
            {
                float randomChance = Random.Range(0f, 1f); // 0 → 1

                if (randomChance < 0.2f) // 20% khả năng thi triển skill mỗi lần check
                {
                    nextTornadoTime = Time.time + tornadoInterval;
                    StartCoroutine(StartTornadoSkill());
                    return;
                }
            }
        }



        if (isInTornadoMode)
        {
            PerformTornadoMovement();
            return;
        }


    }
    IEnumerator StartTornadoSkill()
    {
        rb.gravityScale = 0f;

        isAttacking = true;
        isFlyDashing = false;
        isPreparingFlyDash = false;
        rb.velocity = Vector2.zero;

        Vector3 bossPos = transform.position;
        Vector3 toPlayer = player.position - bossPos;
        tornadoPointA = bossPos;
        tornadoPointB = bossPos + toPlayer * 2f;

        isInTornadoMode = true;
        tornadoEndTime = Time.time + tornadoDuration;
        tornadoCurrentTarget = tornadoPointB;

        animator.SetTrigger("TornadoSpin");

        if (bossCollider != null)
            bossCollider.isTrigger = true;

        yield return null; // để coroutine không block cứng
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!isInTornadoMode) return;
        if (!collision.CompareTag("Player")) return;

        // Kiểm tra cooldown riêng cho từng đối tượng
        if (!nextDamageTimeMap.ContainsKey(collision))
            nextDamageTimeMap[collision] = 0f;

        if (Time.time < nextDamageTimeMap[collision]) return;

        nextDamageTimeMap[collision] = Time.time + tornadoDamageInterval;

        bool tookDamage = false;

        AuronPlayerController auronPlayer = collision.GetComponentInParent<AuronPlayerController>();
        if (auronPlayer != null)
        {
            auronPlayer.TakeDamage(tornadoDamage);
            Debug.Log("💥 Tornado gây " + tornadoDamage + " sát thương cho AuronPlayerController");
            tookDamage = true;
        }

        Player1 player = collision.GetComponentInParent<Player1>();
        if (player != null)
        {
            if (player.isDefending)
            {
                Debug.Log("🛡️ Player đang đỡ đòn — lốc xoáy không gây sát thương.");
                return;
            }
            player.TakeDamage(tornadoDamage);
            Debug.Log("💥 Tornado gây " + tornadoDamage + " sát thương cho Player1");
            tookDamage = true;
        }

        if (!tookDamage)
        {
            Debug.Log("❌ Tornado không tìm thấy AuronPlayerController hoặc Player1 trên đối tượng Player");
        }
    }


    void PerformTornadoMovement()
    {
        Vector3 currentPos = transform.position;
        Vector3 targetPos = new Vector3(tornadoCurrentTarget.x, currentPos.y, currentPos.z);
        float directionX = Mathf.Sign(targetPos.x - currentPos.x);
        rb.velocity = new Vector2(directionX * tornadoSpeed, 0f);

        if (Mathf.Abs(targetPos.x - currentPos.x) < 0.5f)
        {
            tornadoCurrentTarget = (tornadoCurrentTarget == tornadoPointA) ? tornadoPointB : tornadoPointA;
        }

        // 👇 Dừng skill đúng lúc
        if (Time.time >= tornadoEndTime)
        {
            EndTornadoSkill();
        }
    }

    void EndTornadoSkill()
    {
        rb.gravityScale = 1f; // hoặc giá trị cũ bạn đang dùng

        isInTornadoMode = false;
        isAttacking = false;
        rb.velocity = Vector2.zero;

        if (bossCollider != null)
            bossCollider.isTrigger = originalIsTrigger;

        animator.ResetTrigger("TornadoSpin");
        nextDamageTimeMap.Clear();

        animator.Play("Idle"); // reset lại animation nếu cần
    }




    bool ShouldFlyDash()
    {
        return isChasing &&
               (Time.time - chaseStartTime) >= chaseDurationBeforeDash &&
               (Time.time - lastFlyDashTime) >= flyDashCooldown &&
               cachedHorizontalDistance > flyDashStopDistance;
    }

    IEnumerator FlyDash()
    {
        isPreparingFlyDash = true;
        isChasing = false;
        lastFlyDashTime = Time.time;

        rb.velocity = Vector2.zero;
        animator.SetBool("IsRunning", false);

        yield return new WaitForSeconds(flyDashPrepareTime);

        isFlyDashing = true;
        isPreparingFlyDash = false;

        float playerX = player.position.x;
        float bossX = transform.position.x;
        float directionToPlayer = Mathf.Sign(playerX - bossX);

        float targetX = playerX - (directionToPlayer * flyDashStopDistance);
        flyDashTargetPosition = new Vector3(targetX, transform.position.y, transform.position.z);

        float distanceToTarget = Mathf.Abs(bossX - targetX);
        float estimatedDuration = Mathf.Clamp(distanceToTarget / flyDashSpeed, 0.3f, 3f);

        animator.speed = 1f / estimatedDuration;
        animator.SetTrigger("FlyDash");

        float flyStartTime = Time.time;

        while (Mathf.Abs(transform.position.x - targetX) > 0.2f &&
               (Time.time - flyStartTime) < 3f &&
               isFlyDashing)
        {
            float directionX = Mathf.Sign(targetX - transform.position.x);
            rb.velocity = new Vector2(directionX * flyDashSpeed, rb.velocity.y);
            yield return null;
        }

        rb.velocity = Vector2.zero;
        animator.speed = 1f;
        yield return new WaitForSeconds(0.5f);
        isFlyDashing = false;
        chaseStartTime = Time.time;
    }

    void StopFlyDashImmediately()
    {
        Debug.Log("🛑 Boss dừng bay do va chạm Player!");
        rb.velocity = Vector2.zero;
        animator.speed = 1f;

        if (currentFlyDashCoroutine != null)
        {
            StopCoroutine(currentFlyDashCoroutine);
            currentFlyDashCoroutine = null;
        }

        isFlyDashing = false;
        chaseStartTime = Time.time;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isFlyDashing && collision.gameObject.CompareTag("Player"))
        {
            StopFlyDashImmediately();
        }

        // ✅ Nếu đang lốc xoáy và đụng bất kỳ thứ gì thì dừng
        if (isInTornadoMode)
        {
            Debug.Log("💥 Boss va chạm trong khi đang lốc xoáy, dừng skill!");
            EndTornadoSkill();
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isInTornadoMode && !collision.CompareTag("Player"))
        {
            Debug.Log("💥 Boss lốc xoáy va chạm Trigger với: " + collision.name);
            EndTornadoSkill();
        }
    }


    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
        animator.SetBool("IsRunning", true);
    }

    void FlipSprite()
    {
        bool shouldFaceRight = player.position.x > transform.position.x;
        if (shouldFaceRight != facingRight)
        {
            facingRight = shouldFaceRight;

            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.flipX = !facingRight;
            }

            if (attackCollider != null)
            {
                Vector2 offset = attackCollider.offset;
                offset.x *= -1;
                attackCollider.offset = offset;
            }
        }
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

    IEnumerator RainShot()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero;
        animator.SetTrigger("AttackRanged");

        float centerX = player.position.x;
        float startX = centerX - rainWidth / 2f;
        float ySpawn = transform.position.y + rainHeight;

        for (int i = 0; i < rainProjectileCount; i++)
        {
            float xPos = startX + (rainWidth / (rainProjectileCount - 1)) * i;
            Vector3 spawnPos = new Vector3(xPos, ySpawn, 0f);

            GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
            float spread = Random.Range(-0.3f, 0.3f);
            Vector2 direction = (Vector2.down + new Vector2(spread, 0)).normalized;

            projectile.GetComponent<BossProjectile>().SetDirection(direction);
        }

        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }
    bool IsSameLevelAsPlayer()
    {
        return Mathf.Abs(transform.position.y - player.position.y) <= 10f;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.cyan;
        if (player != null)
        {
            Vector3 leftStopPos = player.position + Vector3.left * flyDashStopDistance;
            Vector3 rightStopPos = player.position + Vector3.right * flyDashStopDistance;
            Gizmos.DrawWireSphere(leftStopPos, 0.3f);
            Gizmos.DrawWireSphere(rightStopPos, 0.3f);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(leftStopPos, rightStopPos);
        }
    }
}
