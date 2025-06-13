using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class Player1 : MonoBehaviour
{
    public float moveSpeed = 15f;
    public float jumpForce = 8f;
    public float rollForce = 8f;
    public int maxHealth = 100;
    public int currentHealth;
    public int maxMP = 100;
    public int currentMP;
    public Transform GroundCheck;
    public Transform attackHitbox;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public Player1Healthbar healthBar;
    public Player1MPBar MPBar;

    public int eSkillMPCost = 20;
    public int qSkillMPCost = 30;
    public float mpRegenRate = 5f; 
    private float mpRegenTimer = 0f;

    public int maxJumps = 2;
    public float attackCooldown = 0.3f;
    public float eSkillSlowFactor = 0.3f;
    public float eSkillDuration = 0.6f;

    private int currentAttack = 0;
    private int jumpCount = 0;
    private float timeSinceAttack = 0.0f;
    private float eSkillTimer = 0f;

    private bool isGrounded;
    private bool isRolling;
    private bool isAttacking;
    private bool isUsingESkill;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 movement;

    public int damage = 10;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        healthBar.SetMaxHealth();
        healthBar.gameObject.SetActive(true);

        MPBar.SetMaxMP();
        MPBar.gameObject.SetActive(true);

        Debug.Log("PlayerAttackCollider active on: " + gameObject.name);

    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth((float)currentHealth/maxHealth);
        if (currentHealth <= 0)
        {
            animator.SetBool("IsDead", true);
            Time.timeScale = 0f;
        }
    }

    void Update()
    {
        // Input
        movement.x = Input.GetAxisRaw("Horizontal");
        timeSinceAttack += Time.deltaTime;

        // Ground check
        isGrounded = Physics2D.OverlapCircle(GroundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded)
        {
            jumpCount = 0;
        }

        // Jump input
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumps)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount++;
        }

        // Jump animation
        bool isJumping = !isGrounded && rb.velocity.y > 0.1f;
        bool isFalling = !isGrounded && rb.velocity.y < -0.1f;
        animator.SetBool("isJumping", isJumping);
        animator.SetBool("isFalling", isFalling);

        // Rolling
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isRolling = true;
            animator.SetBool("isRolling", true);
            rb.velocity = new Vector2(movement.x * rollForce, rb.velocity.y);
        }
        else
        {
            isRolling = false;
            animator.SetBool("isRolling", false);
        }

        // Attack combo (Left Click)
        if (Input.GetMouseButtonDown(0) && timeSinceAttack > attackCooldown && !isRolling && !isAttacking)
        {
            currentAttack++;
            if (currentAttack > 3 || timeSinceAttack > 1.0f)
                currentAttack = 1;

            animator.SetTrigger("Attack" + currentAttack);
            timeSinceAttack = 0.0f;
            StartCoroutine(ResetAttackLock(0.4f));
        }

        // Defend (Right Click)
        if (Input.GetMouseButtonDown(1) && !isRolling && !isAttacking)
        {
            animator.SetTrigger("Defend");
            StartCoroutine(ResetAttackLock(0.4f));
        }

        // Skill E (slow move)
        if (Input.GetKeyDown(KeyCode.E) && !isRolling && !isAttacking && currentMP >= eSkillMPCost)
        {
            animator.SetTrigger("Attack4");
            isUsingESkill = true;
            eSkillTimer = eSkillDuration;
            currentMP -= eSkillMPCost;
            MPBar.SetMP((float)currentMP / maxMP);
            StartCoroutine(ResetAttackLock(0.5f));
        }

        // Skill Q
        if (Input.GetKeyDown(KeyCode.Q) && !isRolling && !isAttacking && currentMP >= qSkillMPCost)
        {
            animator.SetTrigger("Attack5");
            currentMP -= qSkillMPCost;
            MPBar.SetMP((float)currentMP / maxMP);
            StartCoroutine(ResetAttackLock(0.5f));
        }


        // Update E skill timer
        if (isUsingESkill)
        {
            eSkillTimer -= Time.deltaTime;
            if (eSkillTimer <= 0f)
            {
                isUsingESkill = false;
            }
        }

        mpRegenTimer += Time.deltaTime;
        if (mpRegenTimer >= 1f)
        {
            mpRegenTimer = 0f;
            currentMP = Mathf.Min(currentMP + (int)mpRegenRate, maxMP);
            MPBar.SetMP((float)currentMP / maxMP);
        }

        // Flip sprite
        bool wasFlipped = spriteRenderer.flipX;
        if (movement.x < 0 && !wasFlipped)
        {
            spriteRenderer.flipX = true;
            FlipHitbox(true);
        }
        else if (movement.x > 0 && wasFlipped)
        {
            spriteRenderer.flipX = false;
            FlipHitbox(false);
        }


        // Animator parameters
        animator.SetBool("isMoving", movement.x != 0);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("VerticalVelocity", rb.velocity.y);
    }
    void FlipHitbox(bool facingLeft)
    {
        Vector3 pos = attackHitbox.localPosition;
        pos.x = Mathf.Abs(pos.x) * (facingLeft ? -1 : 1);
        attackHitbox.localPosition = pos;
    }
    void FixedUpdate()
    {
        if (isRolling) return;

        float speed = isUsingESkill ? moveSpeed * eSkillSlowFactor : moveSpeed;
        rb.velocity = new Vector2(movement.x * speed, rb.velocity.y);
    }

    void OnDrawGizmosSelected()
    {
        if (GroundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(GroundCheck.position, groundCheckRadius);
        }
    }

    private IEnumerator ResetAttackLock(float duration)
    {
        isAttacking = true;
        yield return new WaitForSeconds(duration);
        isAttacking = false;
    }
}
