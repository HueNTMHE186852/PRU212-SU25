using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class BossAI : MonoBehaviour
{
    [SerializeField] private Material flashMaterial;
    [SerializeField] private float duration;

    private Material originMaterial;
    private Coroutine flashRoutine;

    [Header("Stats")]
    public int maxHealth = 100;
    public int currentHealth;

    [Tooltip("Melee damage per hit")]
    public int damage = 10;
    [Tooltip("Seconds between 2 melee attacks")]
    public float attackCooldown = 0.1f;
    [Tooltip("Duration the attack animation keeps the boss locked in place")]
    public float attackDuration = 1f;


    [Header("Detection & Movement")]
    public float speed = 3.5f;
    public float detectionRange = 62f;
    public float attackRange = 10f;
    public float verticalTolerance = 20f;

    [Tooltip("Should the boss ignore vertical distance when deciding to attack?")]
    public bool ignoreVerticalForAttack = true;
    [Tooltip("Force chasing even when outside detection (e.g. after being hit)")]
    public bool forceChase = true;

    [Header("Patrol")]
    public bool usePatrol = true;
    public float patrolDistance = 5f;


    public HealthBar healthBar;
    public GameObject floatingText;


    [Header("Laser Shot")]
    public GameObject laserPrefab;
    public Transform laserSpawnPoint;
    public float laserLifetime = 0.5f;

    [Header("Ice Spike (Phase 2)")]
    public IceSpikeManager iceSpikeManager;

    private Animator animator;
    public Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    public Transform player;
    public Player1 player1;
    private float cachedHorizontalDistance;
    private float cachedVerticalDistance;
    private bool canAttackNow;

    private float lastAttackTime = -10f;
    private bool isAttacking = false;
    private bool isDead = false;
    private bool isPatrolling = true;
    private bool movingRight = true;
    private bool hasHealthBarAppeared = false;
    private bool hasTriggered70 = false;
    private bool hasTriggered30 = false;

    // Patrol helpers
    private Vector3 startPosition;

    // Collider handling / flipping (shared with small‑enemy logic)
    [Header("Colliders")] public Transform colliderHolder;
    public Transform attackCollider;
    private BoxCollider2D boxCollider;
    private BoxCollider2D attackBoxCollider;
    private PolygonCollider2D attackPolygonCollider;
    private Vector2 originalColliderOffset;
    private Vector2 originalAttackColliderOffset;
    private Vector2[] originalPolygonPoints;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originMaterial = spriteRenderer.material;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj)
        {
            player = playerObj.transform;
            player1 = player.GetComponent<Player1>();
        }

        // Cache colliders & their original offsets/points for proper flipping
        CacheColliders();
    }

    private void Start()
    {
        startPosition = transform.position;
        currentHealth = maxHealth;
        if (healthBar) { healthBar.gameObject.SetActive(false); healthBar.SetMaxHealth(maxHealth); }
        animator.Play("Idle", 0, 0f);
    }

    private void Update()
    {
        if (!player || isDead) return;

        // Healthbar appears when boss detected
        if (!hasHealthBarAppeared && DistanceToPlayer() <= detectionRange)
        {
            hasHealthBarAppeared = true;
            if (healthBar) healthBar.gameObject.SetActive(true);
        }

        if (isAttacking)
        {
            if (Time.time >= lastAttackTime + attackDuration)
            {
                Debug.Log("⏱️ Đã hết thời gian tấn công, gọi EndAttack()");
                EndAttack();
            }
            return;
        }

        UpdateDistances();

        bool shouldChase = (cachedVerticalDistance <= verticalTolerance) && (cachedHorizontalDistance <= detectionRange);
        if (!shouldChase && forceChase && currentHealth < maxHealth) shouldChase = true; // chase if already aggroed

        if (shouldChase)
        {
            isPatrolling = false;
            HandleChase();
        }
        else if (usePatrol)
        {
            if (!isPatrolling) { isPatrolling = true; movingRight = true; }
            Patrol();
        }
    }

    // Physics‑based movement in FixedUpdate when running state is active
    private void FixedUpdate()
    {
        if (player == null || isDead) return;
        if (animator.GetBool("isRunning") && !isAttacking)
        {
            Vector2 target = new Vector2(player.position.x, rb.position.y);
            Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
        }
    }

    private void UpdateDistances()
    {
        Vector3 p = player.position;
        Vector3 me = transform.position;

        cachedHorizontalDistance = Mathf.Abs(me.x - p.x);
        cachedVerticalDistance = Mathf.Abs(me.y - p.y);

        bool inHorizontal = cachedHorizontalDistance <= attackRange;
        bool inVertical = ignoreVerticalForAttack || cachedVerticalDistance <= verticalTolerance;
        bool cooldownReady = Time.time >= lastAttackTime + attackCooldown;
        canAttackNow = inHorizontal && inVertical && cooldownReady;
    }

    private float DistanceToPlayer() => Vector2.Distance(transform.position, player.position);

    private void HandleChase()
    {
        if (canAttackNow)
        {
            int rand = Random.Range(0, 2); 
            if (rand == 0)
            {
                StartAttack();
            }
            else
            {
                StartLaserAttack();
            }
            return;
        }

        // Move toward player
        MoveTowardsPlayer();
    }

    private void StartLaserAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        animator.ResetTrigger("meleeAttack");
        animator.SetBool("isRunning", false);

        animator.SetBool("isCharging", true);

        FacePlayer();
    }

    private void MoveTowardsPlayer()
    {
        if (isAttacking) return;
        Vector2 dir = (player.position - transform.position).normalized;
        Vector3 movement = dir * speed * Time.deltaTime;
        transform.position += movement;

        bool flip = dir.x < 0;
        spriteRenderer.flipX = flip;
        UpdateColliderFlip(flip);

        animator.SetBool("isRunning", true);
    }

    private void Patrol()
    {
        float dir = movingRight ? 1f : -1f;
        transform.Translate(Vector2.right * dir * speed * Time.deltaTime);

        bool flip = !movingRight;
        spriteRenderer.flipX = flip;
        UpdateColliderFlip(flip);

        float buffer = .5f;
        float deltaFromStart = transform.position.x - startPosition.x;
        if (movingRight && deltaFromStart >= patrolDistance + buffer) movingRight = false;
        else if (!movingRight && deltaFromStart <= -patrolDistance - buffer) movingRight = true;

        animator.SetBool("isRunning", true);
    }
    
    private void StartAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        animator.ResetTrigger("meleeAttack");
        animator.SetTrigger("meleeAttack");
        animator.SetBool("isRunning", false);

        FacePlayer();
    }

    private void EndAttack()
    {
        isAttacking = false;
        animator.ResetTrigger("meleeAttack");
        animator.SetBool("isRunning", false);
    }

    private void FacePlayer()
    {
        bool flip = player.position.x < transform.position.x;
        spriteRenderer.flipX = flip;
        UpdateColliderFlip(flip);
    }

    // ———————————————————————————————————————————————————————————
    //  🏹 Ranged & Special Attacks
    // ———————————————————————————————————————————————————————————
    public void ShootLaser()
    {
        if (!laserPrefab || !laserSpawnPoint) return;

        GameObject laser = Instantiate(laserPrefab, laserSpawnPoint.position, Quaternion.identity);
        Vector3 scale = laser.transform.localScale;
        scale.x = Mathf.Abs(scale.x);
        laser.transform.localScale = scale;

        bool facingRight = !spriteRenderer.flipX;
        if (facingRight)
        {
            laser.transform.rotation = Quaternion.Euler(0, 0f, -10f);
        }
        else
        {
            laser.transform.rotation = Quaternion.Euler(0, 180f, -10f);
        }

        laser.transform.position += new Vector3(facingRight ? -1f : -95f, 0, 0);
        Destroy(laser, laserLifetime);
    }

    public void EndLaserAttack()
    {
        isAttacking = false;
        animator.SetBool("isCharging", false); 
        animator.SetBool("isRunning", false);
    }

    public void SummonIceSpikes()
    {
        if (iceSpikeManager) iceSpikeManager.StartSpikeAttack();
    }

    public void LaunchIceSpikes()
    {
        if (!iceSpikeManager) return;
        iceSpikeManager.LaunchAllSpikes();
    }

    // ———————————————————————————————————————————————————————————
    //  💔 Damage & Death
    // ———————————————————————————————————————————————————————————
    public void TakeDamage(int amount)
    {
        if (isDead) return;

        ShowDamage(amount.ToString());
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);
        if (healthBar) healthBar.SetHealth(currentHealth);
        Flash();
        Vector2 knockDir = (transform.position - player.position).normalized;
        float knockForce = 1.9f; // Có thể tùy chỉnh
        ApplyKnockback(knockDir * knockForce);

        float pct = (float)currentHealth / maxHealth;
        if (!hasTriggered70 && pct <= 0.7f)
        {
            hasTriggered70 = true;
            damage = Mathf.RoundToInt(damage * 0.8f);
            speed += 1.2f;
            animator.SetTrigger("bossHit");
            float len = GetAnimationClipLength("BossHit");
            if (CameraShake.Instance != null)
            {
                StartCoroutine(CameraShake.Instance.Shake(len, 0.15f));
            }
        }
        if (!hasTriggered30 && pct <= 0.3f)
        {
            hasTriggered30 = true;
            damage = Mathf.RoundToInt(damage * 0.6f);
            speed += 1.5f;
            animator.SetTrigger("bossHit");
            float len = GetAnimationClipLength("BossHit");
            if (CameraShake.Instance != null)
            {
                StartCoroutine(CameraShake.Instance.Shake(len, 0.2f));
            }
        }

        if (currentHealth <= 0) Die();
    }

    public void Flash()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }
        flashRoutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        spriteRenderer.material = flashMaterial;

        yield return new WaitForSeconds(duration);

        spriteRenderer.material = originMaterial;
        flashRoutine = null;
    }

    private float GetAnimationClipLength(string clipName)
    {
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
            if (clip.name == clipName) return clip.length;
        return 0.5f;
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        animator.SetTrigger("die");

        healthBar.gameObject.SetActive(false);
        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;
        if(player1 != null)
        {
            player1.Win();
        }
        StartCoroutine(WaitAndDie());
    }

    private IEnumerator WaitAndDie()
    {
        float len = GetAnimationClipLength("BossDie");
        yield return new WaitForSeconds(len);

        //win scene or next wave
        Destroy(gameObject);
    }

    // ———————————————————————————————————————————————————————————
    //  🎨 GUI / Debug / Helpers
    // ———————————————————————————————————————————————————————————
    private void ShowDamage(string text)
    {
        if (!floatingText) return;
        GameObject go = Instantiate(floatingText, transform.position, Quaternion.identity);
        Vector3 p = go.transform.position; p.z = -1; go.transform.position = p;
        go.GetComponentInChildren<TextMesh>().text = text;
        Destroy(go, 0.8f);
    }

    private void CacheColliders()
    {
        if (colliderHolder)
        {
            boxCollider = colliderHolder.GetComponent<BoxCollider2D>();
            if (boxCollider) originalColliderOffset = boxCollider.offset;
        }

        if (attackCollider)
        {
            attackBoxCollider = attackCollider.GetComponent<BoxCollider2D>();
            attackPolygonCollider = attackCollider.GetComponent<PolygonCollider2D>();
            if (attackBoxCollider) originalAttackColliderOffset = attackBoxCollider.offset;
            if (attackPolygonCollider)
            {
                originalPolygonPoints = new Vector2[attackPolygonCollider.points.Length];
                for (int i = 0; i < originalPolygonPoints.Length; i++) originalPolygonPoints[i] = attackPolygonCollider.points[i];
            }
        }
    }

    public void ApplyKnockback(Vector2 force)
    {
        if (rb != null)
        {
            rb.velocity = new Vector2(force.x, rb.velocity.y);
        }
    }   

    private void UpdateColliderFlip(bool flipped)
    {
        // Main collider
        if (boxCollider)
        {
            Vector2 off = originalColliderOffset;
            off.x = Mathf.Abs(off.x) * (flipped ? -1f : 1f);
            boxCollider.offset = off;
        }

        // Attack collider
        if (attackBoxCollider)
        {
            Vector2 off = originalAttackColliderOffset;
            off.x = Mathf.Abs(off.x) * (flipped ? -1f : 1f);
            attackBoxCollider.offset = off;
        }
        else if (attackPolygonCollider && originalPolygonPoints != null)
        {
            Vector2[] pts = new Vector2[originalPolygonPoints.Length];
            for (int i = 0; i < pts.Length; i++)
            {
                pts[i] = originalPolygonPoints[i];
                pts[i].x = Mathf.Abs(pts[i].x) * (flipped ? -1f : 1f);
            }
            attackPolygonCollider.points = pts;
        }
    }

    // Optional: expose Gizmos similar to EnemyRun
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, detectionRange);
        if (player)
        {
            Gizmos.color = canAttackNow ? Color.red : Color.cyan;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}
