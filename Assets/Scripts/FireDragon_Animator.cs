using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class FireDragonController : MonoBehaviour
{
    public DragonState currentState = DragonState.Idle;

    private Rigidbody2D rb;
    private Animator animator;

    public float flyForce = 7f;
    public LayerMask groundLayer;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        CheckGrounded();

        switch (currentState)
        {
            case DragonState.Idle:
                if (Input.GetKeyDown(KeyCode.UpArrow))
                    EnterFly();
                if (Input.GetKeyDown(KeyCode.Space))
                    EnterAttack();
                break;

            case DragonState.Flying:
                if (Input.GetKeyDown(KeyCode.DownArrow))
                    EnterLanding();
                if (Input.GetKeyDown(KeyCode.Space))
                    EnterAttack();
                break;

            case DragonState.Landing:
                if (isGrounded)
                    EnterIdle(); // Đã hạ cánh xong
                break;

            case DragonState.Attacking:
                // Sau khi tấn công xong thì về trạng thái trước đó
                break;
        }
    }

    void EnterFly()
    {
        rb.velocity = new Vector2(rb.velocity.x, flyForce);
        currentState = DragonState.Flying;
        animator.SetTrigger("isFlying");
    }

    void EnterLanding()
    {
        currentState = DragonState.Landing;
        animator.SetTrigger("isLanding");
    }

    void EnterIdle()
    {
        currentState = DragonState.Idle;
        animator.SetTrigger("isIdle");
    }

    void EnterAttack()
    {
        currentState = DragonState.Attacking;
        animator.SetTrigger("isAttacking");
        Invoke(nameof(BackToIdleOrFly), 0.5f); // giả định attack mất 0.5s
    }

    void BackToIdleOrFly()
    {
        if (isGrounded)
            EnterIdle();
        else
            currentState = DragonState.Flying;
    }

    void CheckGrounded()
    {
        // Dùng Raycast từ chân rồng xuống đất
        Vector2 position = transform.position;
        Vector2 direction = Vector2.down;
        float distance = 0.1f;

        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, groundLayer);
        isGrounded = hit.collider != null;
    }
}
