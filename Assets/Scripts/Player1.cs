using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class Player1 : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float jumpForce = 8f;
    public float rollForce = 8f;
    public Transform GroundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public bool isRolling;
    public float timeSinceAttack = 0.0f;
    public int currentAttack = 0;
    public bool isAttack;

    public int maxJumps = 1;         // Max number of jumps (double jump = 2)
    private int jumpCount = 0;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 movement;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
            jumpCount = 0; // Reset jumps when grounded
        }

        // Jump input with double jump
        // Jump input (double jump)
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumps)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount++;
        }

        // Jump animation control
        bool isJumping = !isGrounded && rb.velocity.y > 0.1f;   // going up
        bool isFalling = !isGrounded && rb.velocity.y < -0.1f;  // going down

        animator.SetBool("isJumping", isJumping);
        animator.SetBool("isFalling", isFalling);


        if (Input.GetKeyDown("left shift"))
        {
            isRolling = true;
            animator.SetBool("isRolling", isRolling);
            rb.velocity = new Vector2(movement.x * rollForce, rb.velocity.y);
        }
        else
        {
            isRolling = false;
        }

        if (Input.GetMouseButtonDown(0) && timeSinceAttack > 0.3f && !isRolling)
        {
            currentAttack++;
            if (currentAttack > 3)
                currentAttack = 1;

            // Reset attack combo if too slow
            if (timeSinceAttack > 1.0f)
                currentAttack = 1;

            animator.SetTrigger("Attack" + currentAttack);
            timeSinceAttack = 0.0f;
        }

        if (Input.GetMouseButtonDown(1) && !isRolling)
        {
            animator.SetTrigger("Defend");
        }

        if (Input.GetKeyDown("e") && !isRolling)
        {
            animator.SetTrigger("Attack4");
        }
        else
        {
        }

        if (Input.GetKeyDown("q") && !isRolling)
        {
            animator.SetTrigger("Attack5");
        }
        else
        {
        }

        // Flip sprite
        if (movement.x < 0)
            spriteRenderer.flipX = true;
        else if (movement.x > 0)
            spriteRenderer.flipX = false;

        // Animator parameters
        animator.SetBool("isMoving", movement.x != 0);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isRolling", isRolling);
        animator.SetFloat("VerticalVelocity", rb.velocity.y);
    }

    void FixedUpdate()
    {
        if (isRolling)
        {
            // Do not override velocity during rolling; preserve initial roll impulse
            return;
        }
        rb.velocity = new Vector2(movement.x * moveSpeed, rb.velocity.y);
    }

    void OnDrawGizmosSelected()
    {
        if (GroundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(GroundCheck.position, groundCheckRadius);
        }
    }
}
