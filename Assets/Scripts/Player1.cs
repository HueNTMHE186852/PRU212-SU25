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

        // Jump input
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        if (Input.GetKeyDown("left shift") && isRolling == false)
        {
            isRolling = true;
            animator.SetBool("isRolling", isRolling);
            rb.velocity = new Vector2(movement.x * rollForce, rb.velocity.y);
        }
        else
            isRolling = false;
        if (Input.GetMouseButtonDown(0) && timeSinceAttack > 0.3f && !isRolling)
        {
            currentAttack++;
            if (currentAttack > 3)
                currentAttack = 1;
            // Reset attack combo if too slow
            if (timeSinceAttack > 1.0f)
                currentAttack = 1;



            // Set the new attack trigger
            animator.SetTrigger("Attack" + currentAttack);

            // Reset timer
            timeSinceAttack = 0.0f;
        }


        // Flip sprite
        if (movement.x < 0) spriteRenderer.flipX = true;
        else if (movement.x > 0) spriteRenderer.flipX = false;

        // Animator parameters
        animator.SetBool("isMoving", movement.x != 0);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isRolling", isRolling);
        animator.SetFloat("VerticalVelocity", rb.velocity.y);
    }

    void FixedUpdate()
    {
        // Apply horizontal movement
        rb.velocity = new Vector2(movement.x * moveSpeed, rb.velocity.y);
    }

    // Debug circle in Scene view
    void OnDrawGizmosSelected()
    {
        if (GroundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(GroundCheck.position, groundCheckRadius);
        }
    }
}
