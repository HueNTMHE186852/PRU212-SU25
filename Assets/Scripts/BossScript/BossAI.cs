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
	public GameObject laserPrefab;
	public Transform laserSpawnPoint;
	public float laserLifetime = 0.5f;
	private bool hasHealthBarAppeared = false;
	public Transform colliderHolder;
	public Transform attackCollider;
	[HideInInspector] public Transform player;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public bool isFlipped = true;
    [HideInInspector] public bool isKnockback = false;
    private bool hasTriggered70 = false;
    private bool hasTriggered30 = false;
    private Animator animator;
    public  bool isAttacking = false;
    private bool isChargingFinished = false;
    private bool isShooting = false;
    private float idleTimer;
    private bool decidedAction = false;
    public IceSpikeManager iceSpikeManager;
    [HideInInspector] public bool hasCollidedWithPlayer = false;
    private BoxCollider2D boxCollider;
	private BoxCollider2D attackBoxCollider;
	private PolygonCollider2D attackPolygonCollider;
	private Vector2 originalColliderOffset;
	private Vector2 originalAttackColliderOffset;
	private Vector2[] originalPolygonPoints;

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


		// Lưu offset gốc của BoxCollider
		if (colliderHolder != null)
		{
			boxCollider = colliderHolder.GetComponent<BoxCollider2D>();
			if (boxCollider != null)
			{
				originalColliderOffset = boxCollider.offset;
				Debug.Log($"💾 Saved original BoxCollider offset: {originalColliderOffset}");
			}
			else
			{
				Debug.LogError("❌ ColliderHolder không có BoxCollider2D component!");
			}
		}
		else
		{
			Debug.LogError("❌ ColliderHolder chưa được gán trong Inspector!");
		}

		// Kiểm tra và lưu thông tin cho AttackCollider
		if (attackCollider != null)
		{
			// Kiểm tra xem là BoxCollider2D hay PolygonCollider2D
			attackBoxCollider = attackCollider.GetComponent<BoxCollider2D>();
			attackPolygonCollider = attackCollider.GetComponent<PolygonCollider2D>();

			if (attackBoxCollider != null)
			{
				originalAttackColliderOffset = attackBoxCollider.offset;
			}
			else if (attackPolygonCollider != null)
			{
				// Lưu các điểm gốc của PolygonCollider
				originalPolygonPoints = new Vector2[attackPolygonCollider.points.Length];
				for (int i = 0; i < attackPolygonCollider.points.Length; i++)
				{
					originalPolygonPoints[i] = attackPolygonCollider.points[i];
				}

			}
			else
			{
				Debug.LogError("❌ AttackCollider không có BoxCollider2D hoặc PolygonCollider2D component!");
			}
		}
		else
		{
			Debug.LogError("❌ AttackCollider chưa được gán trong Inspector!");
		}
	}

	void UpdateColliderFlip(bool isFlipped)
	{
		// Lật BoxCollider2D chính (gắn vào colliderHolder)
		if (boxCollider != null)
		{
			Vector2 newOffset = originalColliderOffset;
			newOffset.x = originalColliderOffset.x * (isFlipped ? -1f : 1f);
			boxCollider.offset = newOffset;
		}

		// Lật AttackCollider
		if (attackCollider != null)
		{
			if (attackBoxCollider != null)
			{
				// Lật offset BoxCollider2D tấn công
				Vector2 newOffset = originalAttackColliderOffset;
				newOffset.x = originalAttackColliderOffset.x * (isFlipped ? -1f : 1f);
				attackBoxCollider.offset = newOffset;
			}
			else if (attackPolygonCollider != null && originalPolygonPoints != null)
			{
				// Lật điểm của PolygonCollider2D tấn công
				Vector2[] flippedPoints = new Vector2[originalPolygonPoints.Length];

				for (int i = 0; i < originalPolygonPoints.Length; i++)
				{
					flippedPoints[i] = originalPolygonPoints[i];
					flippedPoints[i].x *= (isFlipped ? -1f : 1f);
				}

				attackPolygonCollider.points = flippedPoints;
			}
		}
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
			animator.SetBool("isRunning", false);
			isAttacking = true;
			animator.SetTrigger("meleeAttack");
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
        float percent = (float)currentHealth / maxHealth;

        if (!hasTriggered70 && percent <= 0.7f)
        {
            hasTriggered70 = true;
            animator.SetTrigger("bossHit");

            float bossHitDuration = GetAnimationClipLength("BossHit");
            if (CameraShake.Instance != null)
            {
                StartCoroutine(CameraShake.Instance.Shake(bossHitDuration, 0.1f));
            }
        }

        if (!hasTriggered30 && percent <= 0.3f)
        {
            hasTriggered30 = true;
            animator.SetTrigger("bossHit");

            float bossHitDuration = GetAnimationClipLength("BossHit");
            if (CameraShake.Instance != null)
            {
                StartCoroutine(CameraShake.Instance.Shake(bossHitDuration, 0.15f));
            }
        }
        if (currentHealth <= 0)
		{
			healthBar.gameObject.SetActive(false);
			Die();
		}
	}

    private float GetAnimationClipLength(string clipName)
    {
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        foreach (var clip in ac.animationClips)
        {
            if (clip.name == clipName)
            {
                return clip.length;
            }
        }
        Debug.LogWarning("Animation clip not found: " + clipName);
        return 0.5f; // fallback duration
    }

    void FixedUpdate()
	{
		if (player == null) return;

		bool inDetectionRange = InDetectionRange();
		bool inAttackRange = InAttackRange();

		if (animator.GetBool("isRunning") && inDetectionRange && !inAttackRange && !isAttacking)
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
				// Boss quay phải → xoay ngược lại 180 độ + lệch xuống 10 độ
				laser.transform.rotation = Quaternion.Euler(0, 180f, -10f);
			}
			else
			{
				// Boss quay trái → lệch xuống 10 độ
				laser.transform.rotation = Quaternion.Euler(0, 0f, -10f);
			}

			Vector3 offset = new Vector3(1f, 0, 0);
			if (!isFlipped)
				offset.x *= -1;

			laser.transform.position += offset;

			Destroy(laser, laserLifetime);
		}
	}

	public void ApplyKnockback(Vector2 force)
    {
        if (rb != null)
        {
            rb.velocity = new Vector2(force.x, rb.velocity.y);
        }
    }

	void Die()
	{
		animator.SetTrigger("die");
		rb.velocity = Vector2.zero; 
		this.enabled = false;
		Destroy(gameObject, 2.5f);
	}

	private bool InDetectionRange()
	{
		return DistanceToPlayer() <= detectionRange;
	}

	private bool InAttackRange()
	{
		Vector2 delta = AxisDistanceToPlayer();
		return delta.x <= attackRange && delta.y <= 1f; // dy là 1f để đảm bảo cùng tầng
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			hasCollidedWithPlayer = true;

			animator.SetBool("isRunning", false);
			isAttacking = true;
			animator.SetTrigger("meleeAttack");
			Debug.Log($"Cham Player");
		}
	}


	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			hasCollidedWithPlayer = false;	
		}
	}
	private float DistanceToPlayer()
	{
		return Vector2.Distance(transform.position, player.position);
	}
	private Vector2 AxisDistanceToPlayer()
	{
		Vector2 bossPos = transform.position;
		Vector2 playerPos = player.position;

		return new Vector2(Mathf.Abs(bossPos.x - playerPos.x), Mathf.Abs(bossPos.y - playerPos.y));
	}
    public void SummonIceSpikes()
    {
        Debug.Log("⛄ SummonIceSpikes called!");

        if (iceSpikeManager != null)
        {
            iceSpikeManager.StartSpikeAttack();
        }
        else
        {
            Debug.LogWarning("⚠️ IceSpikeManager not assigned on BossAI!");
        }
    }
    public void LaunchIceSpikes()
    {
        Debug.Log("🚀 LaunchIceSpikes called from Animation Event!");

        if (iceSpikeManager != null)
        {
            if (CameraShake.Instance != null)
            {
                StartCoroutine(CameraShake.Instance.Shake(0.15f, 0.05f));
            }
            iceSpikeManager.LaunchAllSpikes();
        }
        else
        {
            Debug.LogWarning("⚠️ IceSpikeManager not assigned on BossAI!");
        }
    }
}
