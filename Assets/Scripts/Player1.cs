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
    public Transform GroundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public Player1Healthbar healthBar;

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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        healthBar.SetMaxHealth(maxHealth);
        healthBar.gameObject.SetActive(false);
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        healthBar.SetHealth(currentHealth);
        if (currentHealth <= 0)
        {

            // Call Die Function
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
        if (Input.GetKeyDown(KeyCode.E) && !isRolling && !isAttacking)
        {
            animator.SetTrigger("Attack4");
            isUsingESkill = true;
            eSkillTimer = eSkillDuration;
            StartCoroutine(ResetAttackLock(0.5f));
        }

        // Skill Q
        if (Input.GetKeyDown(KeyCode.Q) && !isRolling && !isAttacking)
        {
            animator.SetTrigger("Attack5");
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

        // Flip sprite
        if (movement.x < 0)
            spriteRenderer.flipX = true;
        else if (movement.x > 0)
            spriteRenderer.flipX = false;

        // Animator parameters
        animator.SetBool("isMoving", movement.x != 0);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("VerticalVelocity", rb.velocity.y);
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
