using UnityEngine;

public class EnemyRun : MonoBehaviour
{
    //public EnemyHealthBar healthbar;
    public float speed = 2f;
    public float verticalTolerance = 5f;
    public float attackRange = 2.5f;
    public float detectionRange = 10f;
    public float attackCooldown = 0.1f;
    public float attackDuration = 1f;
    public Transform colliderHolder;  // Kéo thả ColliderHolder từ Inspector
    private Vector3 startPosition;
    public float patrolDistance = 5f; // Enemy tuần tra trái-phải bao nhiêu đơn vị
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

    private void OnMouseDown()
    {
        currentHeatlh -= 10;
        //healthbar.updateHeathBar(currentHeatlh, maxHealth);
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

        // DEBUG CHI TIẾT
        if (inHorizontalRange)
        {
            Debug.Log($"🎯 IN ATTACK RANGE! H:{cachedHorizontalDistance:F2} V:{cachedVerticalDistance:F2} CD:{cooldownReady} => CanAttack:{canAttackNow}");
        }
    }

    void HandleChase()
    {
        // KIỂM TRA ATTACK TRƯỚC KHI DI CHUYỂN
        if (canAttackNow)
        {
            Debug.Log($"🚀 ATTACK NOW! Distance: {cachedHorizontalDistance:F3}");
            StartAttack();
            return;
        }

        // Nếu trong attack range nhưng chưa attack được (do cooldown)
        if (cachedHorizontalDistance <= attackRange)
        {
            float remainingCooldown = (lastAttackTime + attackCooldown) - Time.time;
            Debug.Log($"⏸️ WAITING FOR COOLDOWN: {remainingCooldown:F2}s");
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
        Debug.Log($"⚔️ ATTACK STARTED at distance {cachedHorizontalDistance:F2}!");
    }

    void EndAttack()
    {
        isAttacking = false;

        if (animator != null)
        {
            animator.ResetTrigger("Attack");
            animator.Play("Run");
        }

        Debug.Log("✅ Attack ended, back to Run");
    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        Vector3 movement = (Vector3)(direction * speed * Time.deltaTime);
        transform.position += movement;

        // Flip sprite theo hướng di chuyển
        bool flip = direction.x < 0;
        spriteRenderer.flipX = flip;

        // Lật collider con theo sprite
        UpdateColliderFlip(flip);

        Debug.Log($"🏃 Moving towards player. Current distance: {cachedHorizontalDistance:F2}");
    }


    void FacePlayer()
    {
        spriteRenderer.flipX = player.position.x < transform.position.x;
    }

    void Patrol()
    {
        float dir = movingRight ? 1f : -1f;
        transform.Translate(Vector2.right * dir * speed * Time.deltaTime);
        spriteRenderer.flipX = !movingRight;

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


    void UpdateColliderFlip(bool flipX)
    {
        if (colliderHolder == null) return;

        Vector3 localScale = colliderHolder.localScale;
        localScale.x = flipX ? -Mathf.Abs(localScale.x) : Mathf.Abs(localScale.x);
        colliderHolder.localScale = localScale;
    }


    // Debug đơn giản hơn
    void OnGUI()
    {
        if (player == null) return;

        GUI.Label(new Rect(10, 10, 300, 20), $"Distance: {cachedHorizontalDistance:F2}");
        GUI.Label(new Rect(10, 30, 300, 20), $"Can Attack: {canAttackNow}");
        GUI.Label(new Rect(10, 50, 300, 20), $"Is Attacking: {isAttacking}");
        GUI.Label(new Rect(10, 70, 300, 20), $"Cooldown: {(lastAttackTime + attackCooldown - Time.time):F1}s");
        GUI.Label(new Rect(10, 90, 300, 20), $"Time: {Time.time:F1}s");
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
}