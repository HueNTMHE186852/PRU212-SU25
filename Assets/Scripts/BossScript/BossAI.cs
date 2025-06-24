using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BossAI : MonoBehaviour
{
    public float detectionRange = 16f;
    public float attackRange = 2.5f;
    public float speed = 3f;
    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;
	public GameObject incinerationParticles;
	public Transform incinerationSpawnPoint;
	public float incinerationLifetime = 0.5f;
	private bool hasHealthBarAppeared = false;

	[HideInInspector] public Transform player;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public bool isFlipped = true;

    private Animator animator;
    private bool isAttacking = false;
    private bool isChargingFinished = false;
    private bool isShooting = false;
    private float idleTimer;
    private bool decidedAction = false;

	void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        idleTimer = Random.Range(0.5f, 1f);
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

		healthBar.gameObject.SetActive(false);
	}

	void Update()
	{
		if (player == null) return;

		// Gọi kiểm tra khoảng cách
		bool inDetectionRange = InDetectionRange();
		bool inAttackRange = InAttackRange();

		// Hiện thanh máu nếu trong vùng phát hiện
		if (!hasHealthBarAppeared && inDetectionRange)
		{
			hasHealthBarAppeared = true;
			healthBar.gameObject.SetActive(true);
		}

		// Nếu chưa phát hiện, boss đứng yên
		if (!inDetectionRange) return;

		if (!animator.GetBool("isCharging") && !isShooting)
		{
			LookAtPlayer();
		}


		// Hành vi AI
		if (!decidedAction)
		{
			idleTimer -= Time.deltaTime;
			if (idleTimer <= 0)
			{
				decidedAction = true;
				int rand = Random.Range(0, 2);

				if (rand == 0)
				{
					animator.SetBool("isRunning", true);
					animator.SetBool("isCharging", false);
				}
				else
				{
					animator.SetBool("isCharging", true);
					animator.SetBool("isRunning", false);
					isChargingFinished = false;
				}
			}
		}

		if (animator.GetBool("isRunning") && inAttackRange && !isAttacking)
		{
			isAttacking = true;
			animator.SetTrigger("meleeAttack");
			animator.SetBool("isRunning", false);
		}

		if (animator.GetBool("isCharging") && inAttackRange && isChargingFinished && !isShooting)
		{
			isShooting = true;
			animator.SetBool("isShooting", true);
		}
	}
	public void TakeDamage(int damage)
    {
		Debug.Log("nhan " + damage + "dame");
        currentHealth -= damage;
		currentHealth = Mathf.Max(currentHealth, 0);
		healthBar.SetHealth(currentHealth);
		if (currentHealth <= 0)
		{
			healthBar.gameObject.SetActive(false);
			Die();
		}
	}

	void FixedUpdate()
	{
		if (player == null) return;

		bool inDetectionRange = InDetectionRange();
		bool inAttackRange = InAttackRange();

		if (animator.GetBool("isWalking") && inDetectionRange && !inAttackRange)
		{
			Vector2 target = new Vector2(player.position.x, rb.position.y);
			Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
			rb.MovePosition(newPos);
		}
	}


	public void LookAtPlayer()
    {
        if (player == null) return;

        if (transform.position.x > player.position.x && isFlipped)
        {
            Flip();
        }
        else if (transform.position.x < player.position.x && !isFlipped)
        {
            Flip();
        }
    }

    void Flip()
    {
        isFlipped = !isFlipped;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void OnAttackAnimationEnd()
    {
        isAttacking = false;
        animator.SetBool("isRunning", false);
        ResetState();
    }

    public void OnChargingAnimationEnd()
    {
        isChargingFinished = true;
    }

    public void OnCastAnimationEnd()
    {
        isShooting = false;
        animator.SetBool("isCharging", false);
        animator.SetBool("isShooting", false);

        LookAtPlayer();

        ResetState();
    }

    private void ResetState()
    {
        decidedAction = false;
        idleTimer = Random.Range(0.5f, 1f);
    }

	void Die()
	{
		animator.SetTrigger("Death");
		rb.velocity = Vector2.zero; 
		this.enabled = false;
		Destroy(gameObject, 2.5f);
	}

private bool InDetectionRange()
{
	Vector2 bossPos = new Vector2(transform.position.x, transform.position.y);
	Vector2 playerPos = new Vector2(player.position.x, player.position.y);
	return Vector2.Distance(bossPos, playerPos) <= detectionRange;
}

	private bool InAttackRange()
	{
		float dx = Mathf.Abs(transform.position.x - player.position.x);
		float dy = Mathf.Abs(transform.position.y - player.position.y);
		return dx <= attackRange && dy < 1f;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			hasCollidedWithPlayer = true;
			animator.SetTrigger("meleeAttack");
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			hasCollidedWithPlayer = false;	
		}
	}
}
