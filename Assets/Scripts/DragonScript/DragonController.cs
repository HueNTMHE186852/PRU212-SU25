using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonController : MonoBehaviour
{
    public EnemyHealthBar healthbar;
    public int damage = 20;
    public float speed = 2.5f;
    public float verticalTolerance = 25f;
    public float attackRange = 12f;
    public float detectionRange = 30f;
    public float attackCooldown = 1.5f;
    public float attackDuration = 1.2f;
    public Transform colliderHolder;
    public Transform attackCollider;
    public GameObject floatingText;
    private bool isDead = false;
    private Vector3 startPosition;
    public float currentHealth;
    public float maxHealth = 100;
    private float lastAttackTime = -10f;
    private bool isAttacking = false;
    private Transform player;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    public bool forceChase = true;
    public bool ignoreVerticalForAttack = true;

    private float cachedHorizontalDistance;
    private float cachedVerticalDistance;
    private bool canAttackNow = false;

    private Vector2 originalColliderOffset;
    private Vector2 originalAttackColliderOffset;
    private BoxCollider2D boxCollider;
    private BoxCollider2D attackBoxCollider;
    private PolygonCollider2D attackPolygonCollider;
    private Vector2[] originalPolygonPoints;

    [SerializeField] private GameObject hpBowlPrefab;
    [SerializeField] private GameObject manaBowlPrefab;

    private void OnMouseDown()
    {
        TakeDamage(15);
        Debug.Log("Dragon nhận 15 dame");
    }

    void Start()
    {
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
            return;
        }

        Transform modelTransform = transform.Find("Model");
        if (modelTransform != null)
        {
            spriteRenderer = modelTransform.GetComponent<SpriteRenderer>();
            animator = modelTransform.GetComponent<Animator>();
        }

        if (colliderHolder != null)
        {
            boxCollider = colliderHolder.GetComponent<BoxCollider2D>();
            if (boxCollider != null)
            {
                originalColliderOffset = boxCollider.offset;
            }
        }

        if (attackCollider != null)
        {
            attackBoxCollider = attackCollider.GetComponent<BoxCollider2D>();
            attackPolygonCollider = attackCollider.GetComponent<PolygonCollider2D>();

            if (attackBoxCollider != null)
            {
                originalAttackColliderOffset = attackBoxCollider.offset;
            }
            else if (attackPolygonCollider != null)
            {
                originalPolygonPoints = new Vector2[attackPolygonCollider.points.Length];
                for (int i = 0; i < attackPolygonCollider.points.Length; i++)
                {
                    originalPolygonPoints[i] = attackPolygonCollider.points[i];
                }
            }
        }
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (player == null || isDead) return;

        UpdateDistances();

        if (canAttackNow && !isAttacking)
        {
            animator.SetBool("IsWalking", false);
            if (animator != null && animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
            {
                animator.Play("Idle", 0, 0f);
            }
            StartAttack();
            return;
        }


        bool shouldChase = (cachedVerticalDistance <= verticalTolerance) && (cachedHorizontalDistance <= detectionRange);

        if (shouldChase)
        {
            HandleChase();
        }
        else
        {
            animator.SetBool("IsWalking", false);
            if (animator != null && animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
            {
                animator.Play("Idle", 0, 0f);
            }
        }

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    void HandleChase()
    {
        if (isAttacking || canAttackNow)
        {
            return;
        }

        FacePlayer();
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


    void Die()
    {
        if (isDead) return;
        isDead = true;

        isAttacking = false;
        animator.ResetTrigger("Attack");
        animator.SetTrigger("Die");

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }

        if (attackBoxCollider != null) attackBoxCollider.enabled = false;
        if (attackPolygonCollider != null) attackPolygonCollider.enabled = false;

        float dropChance = Random.Range(0f, 1f);

        if (dropChance < 1f / 3f)
        {
            Instantiate(hpBowlPrefab, transform.position, Quaternion.identity);
        }
        else if (dropChance < 2f / 3f)
        {
            Instantiate(manaBowlPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.Log("Không rơi gì");
        }

        Destroy(gameObject, 1.5f);
    }

    void MoveTowardsPlayer()
    {
        if (animator != null && animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            animator.Play("Walk", 0, 0f);
        }
        animator.SetBool("IsWalking", true);

        Vector2 direction = (player.position - transform.position).normalized;
        Vector3 movement = (Vector3)(direction * speed * Time.deltaTime);
        transform.position += movement;
        bool flip = direction.x < 0;
        spriteRenderer.flipX = flip;

        UpdateColliderFlip(flip);
    }

    void FacePlayer()
    {
        bool flip = player.position.x < transform.position.x;
        spriteRenderer.flipX = flip;
        UpdateColliderFlip(flip);
    }

    void UpdateColliderFlip(bool isFlipped)
    {
        if (boxCollider != null)
        {
            Vector2 newOffset = originalColliderOffset;
            if (isFlipped)
            {
                newOffset.x = originalColliderOffset.x * -1f;
            }
            boxCollider.offset = newOffset;
        }

        if (attackCollider != null)
        {
            if (attackBoxCollider != null)
            {
                Vector3 newScale = attackCollider.localScale;
                newScale.x = Mathf.Abs(newScale.x) * (isFlipped ? -1f : 1f);
                attackCollider.localScale = newScale;
            }
            else if (attackPolygonCollider != null && originalPolygonPoints != null)
            {
                Vector2[] flippedPoints = new Vector2[originalPolygonPoints.Length];
                for (int i = 0; i < originalPolygonPoints.Length; i++)
                {
                    flippedPoints[i] = originalPolygonPoints[i];
                    if (isFlipped)
                    {
                        flippedPoints[i].x = -originalPolygonPoints[i].x;
                    }
                }
                attackPolygonCollider.points = flippedPoints;
            }
        }
    }

    public void TakeDamage(int amount)
    {
        ShowDame(amount.ToString());

        isAttacking = false;
        currentHealth -= amount;
        Debug.Log("💔 Dragon bị đánh, máu còn: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void ShowDame(string text)
    {
        if (floatingText)
        {
            GameObject prefab = Instantiate(floatingText, transform.position, Quaternion.identity);
            Vector3 fixedPos = prefab.transform.position;
            fixedPos.z = -1;
            prefab.transform.position = fixedPos;

            prefab.GetComponentInChildren<TextMesh>().text = text;
            Destroy(prefab, 0.8f);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        if (player != null)
        {
            Gizmos.color = canAttackNow ? Color.red : Color.cyan;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }

    void UpdateDistances()
    {
        Vector3 playerPos = player.position;
        Vector3 myPos = transform.position;

        cachedHorizontalDistance = Mathf.Abs(myPos.x - playerPos.x);
        cachedVerticalDistance = Mathf.Abs(myPos.y - playerPos.y);

        bool inHorizontalRange = cachedHorizontalDistance <= attackRange;
        bool inVerticalRange = ignoreVerticalForAttack || cachedVerticalDistance <= verticalTolerance;
        bool cooldownReady = Time.time >= lastAttackTime + attackCooldown;

        canAttackNow = inHorizontalRange && inVerticalRange && cooldownReady;
    }
}
