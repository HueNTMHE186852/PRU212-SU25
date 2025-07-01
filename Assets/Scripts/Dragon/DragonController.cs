using UnityEngine;

public class DragonController : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public Transform player;
    public ParticleSystem incinerationEffect;
    [SerializeField] private Transform model;
    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private GameObject hpBowlPrefab;
    [SerializeField] private GameObject manaBowlPrefab;

    [Header("Colliders")]
    [SerializeField] private GameObject attackColliderObj;
    [SerializeField] private GameObject fireZoneColliderObj;

    [Header("Stats")]
    public float attackRange = 10f;
    public float detectionRange = 20f;
    public float attackCooldown = 2f;
    public float maxHealth = 100f;
    public int incinerationEvery = 10;
    public float dropItemChance = 0.66f;

    [Header("Runtime Debug")]
    public float currentHealth;
    [HideInInspector] public int normalAttackCount = 0;
    [HideInInspector] public float lastAttackTime;
    [HideInInspector] public bool isDead = false;

    private bool isFacingLeft = false;

    // FSM
    private IDragonState currentState;
    public IdleState idleState;
    public WalkingState walkingState;
    public AttackingState attackingState;
    public IncineratingState incineratingState;
    public DyingState dyingState;

    void Start()
    {
        // Init FSM
        idleState = new IdleState(this);
        walkingState = new WalkingState(this);
        attackingState = new AttackingState(this);
        incineratingState = new IncineratingState(this);
        dyingState = new DyingState(this);

        currentHealth = maxHealth;
        attackColliderObj.SetActive(false);
        fireZoneColliderObj.SetActive(false);

        // Gán sự kiện animation nếu có
        var animEvents = model.GetComponent<DragonAnimationEvents>();
        if (animEvents != null)
        {
            animEvents.controller = this;
        }

        TransitionToState(idleState);
    }

    void Update()
    {
        currentState?.Update();
    }

    public void TransitionToState(IDragonState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public bool CanAttack()
    {
        return Time.time >= lastAttackTime + attackCooldown &&
               Vector2.Distance(transform.position, player.position) <= attackRange;
    }

    public bool CanSeePlayer()
    {
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
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        ShowDame(amount.ToString());
        if (currentHealth <= 0)
        {
            isDead = true;
            TransitionToState(dyingState);
        }
    }

    private void ShowDame(string text)
    {
        if (!floatingTextPrefab) return;
        GameObject floating = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
        floating.GetComponentInChildren<TextMesh>().text = text;
        floating.transform.position += Vector3.up;
        Destroy(floating, 1f);
    }
}
