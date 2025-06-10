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
	public GameObject laserPrefab;
	public Transform laserSpawnPoint;
	public float laserLifetime = 0.5f;
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

        float distance = Vector2.Distance(transform.position, player.position);
        bool inAttackRange = distance <= attackRange;
		if (!hasHealthBarAppeared && distance <= detectionRange)
		{
			hasHealthBarAppeared = true;
			healthBar.gameObject.SetActive(true);
		}

		if (!animator.GetBool("isCharging") && !isShooting)
        {
            LookAtPlayer();
        }

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

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
		currentHealth = Mathf.Max(currentHealth, 0);
		healthBar.SetHealth(currentHealth);
		if (currentHealth <= 0)
		{
			healthBar.gameObject.SetActive(false);
			// Call Die Function
		}
	}

    void FixedUpdate()
    {
        if (player == null) return;

        float distanceX = Mathf.Abs(transform.position.x - player.position.x);
        float distanceY = Mathf.Abs(transform.position.y - player.position.y);
        bool inDetectionRange = distanceX <= detectionRange;
        bool inAttackRange = distanceX <= attackRange && distanceY < 1f;

        if (animator.GetBool("isRunning") && inDetectionRange && !inAttackRange)
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

    public void OnShootAnimationEnd()
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

	public void ShootLaser()
	{
		if (laserPrefab != null && laserSpawnPoint != null)
		{
			GameObject laser = Instantiate(laserPrefab, laserSpawnPoint.position, Quaternion.identity);

			Vector3 scale = laser.transform.localScale;
			scale.x = Mathf.Abs(scale.x);
			laser.transform.localScale = scale;

			if (!isFlipped)
			{
				laser.transform.rotation = Quaternion.Euler(0, 180f, 0);
			}

			Vector3 offset = new Vector3(1f, 0, 0);
			if (!isFlipped)
				offset.x *= -1;

			laser.transform.position += offset;

			Destroy(laser, laserLifetime);
		}
	}

}
