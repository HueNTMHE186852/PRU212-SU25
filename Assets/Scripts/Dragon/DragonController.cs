using System.Collections;
using UnityEngine;

public class DragonController : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    [HideInInspector]
    public Transform player;
    public ParticleSystem incinerationEffect;
    [SerializeField] private Transform model;
    [SerializeField] private HealthBar healthBar;
    private bool hasHealthBarAppeared = false;

    [Header("Colliders")]
    [SerializeField] private GameObject attackColliderObj;
    [SerializeField] public GameObject fireZoneColliderObj;

    [Header("Stats")]
    public float attackRange = 10f;
    public float detectionRange = 20f;
    public float attackCooldown = 2f;
    public float maxHealth = 100f;
    public int incinerationEvery = 10;

    [Header("Runtime Debug")]
    public float currentHealth;
    [HideInInspector] public int normalAttackCount = 0;
    [HideInInspector] public float lastAttackTime;
    [HideInInspector] public bool isDead = false;

    private bool isFacingLeft = false;

    public Player1 player1;
    public AuronPlayerController player2;

    // FSM
    private IDragonState currentState;
    public IdleState idleState;
    public WalkingState walkingState;
    public AttackingState attackingState;
    public IncineratingState incineratingState;
    public DyingState dyingState;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Init FSM
        idleState = new IdleState(this);
        walkingState = new WalkingState(this);
        attackingState = new AttackingState(this);
        incineratingState = new IncineratingState(this);
        dyingState = new DyingState(this);

        currentHealth = maxHealth;
        attackColliderObj.SetActive(false);
        fireZoneColliderObj.SetActive(false);
        if (healthBar != null)
        {
            healthBar.SetMaxHealth((int)maxHealth);
            healthBar.gameObject.SetActive(false);
        }

        // Gán sự kiện animation nếu có
        var animEvents = model.GetComponent<DragonAnimationEvents>();
        if (animEvents != null)
        {
            animEvents.controller = this;
        }

        TransitionToState(idleState);
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
        currentState?.Update();
        if (!hasHealthBarAppeared && CanSeePlayer())
        {
            hasHealthBarAppeared = true;
            if (healthBar != null)
                healthBar.gameObject.SetActive(true);
        }
    }

    public void TransitionToState(IDragonState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter(player1, player2);
    }

    public bool CanAttack()
    {
        if (player == null) return false;
        return Time.time >= lastAttackTime + attackCooldown &&
               Vector2.Distance(transform.position, player.position) <= attackRange;
    }

    public bool CanSeePlayer()
    {
        if (player == null) return false;
        return Vector2.Distance(transform.position, player.position) <= detectionRange;
    }


    public void OnAttackEnd()
    {
        if (currentState is AttackingState attackState)
        {
            attackState.OnAttackEnd();
        }
    }

    public void EnableNormalAttackCollider() => attackColliderObj.SetActive(true);
    public void DisableNormalAttackCollider() => attackColliderObj.SetActive(false);
    public void EnableFireZoneCollider() => fireZoneColliderObj.SetActive(true);
    public void DisableFireZoneCollider() => fireZoneColliderObj.SetActive(false);
    public void OnAttackFrameStart() => EnableNormalAttackCollider();
    public void OnAttackFrameEnd() => DisableNormalAttackCollider();

    public void FaceDirection(Vector2 dir)
    {
        bool shouldFaceLeft = dir.x < 0;
        if (shouldFaceLeft == isFacingLeft) return;

        isFacingLeft = shouldFaceLeft;

        // 👉 Flip object cha
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (shouldFaceLeft ? -1f : 1f);
        transform.localScale = scale;

        // 👉 Flip particle shape nếu cần
        if (incinerationEffect != null)
        {
            var shape = incinerationEffect.shape;
            shape.rotation = new Vector3(0f, shouldFaceLeft ? 180f : 0f, 0f);
        }

        // 👉 Prevent HealthBar from flipping
        if (healthBar != null)
        {
            Vector3 healthBarScale = healthBar.transform.localScale;
            healthBarScale.x = Mathf.Abs(healthBarScale.x);
            healthBar.transform.localScale = healthBarScale;
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (healthBar != null)
            healthBar.SetHealth((int)currentHealth);

        if (currentHealth <= 0)
        {
            isDead = true;
            if (healthBar != null)
                healthBar.gameObject.SetActive(false);

            if (animator != null)
                animator.enabled = false;

            if (model != null)
                model.gameObject.SetActive(false);

            TransitionToState(dyingState);
        }
    }
}