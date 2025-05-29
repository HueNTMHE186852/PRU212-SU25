using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuronPlayerController : MonoBehaviour
{
    private Animator animator;
    public float moveSpeed = 3f;
    private Rigidbody2D rb;

    public float jumpForce = 7f;
    private bool isGrounded = true;
    private bool isAttacking = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(horizontal, 0, vertical);

        bool isMoving = movement.sqrMagnitude > 0f;
        animator.SetBool("IsMoving", isMoving);
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            isAttacking = true;
            animator.SetBool("IsAttacking", true);
        }

        if (isMoving)
        {
            // ✅ Di chuyển
            movement = movement.normalized;
            transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);

            // ✅ Lật nhân vật theo hướng (trái/phải)
            if (horizontal != 0)
            {
                Vector3 scale = transform.localScale;
                scale.x = horizontal > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
                transform.localScale = scale;
            }
        }
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
            animator.SetBool("IsJumping", true);
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetBool("IsJumping", false);

            // Cập nhật IsMoving để đảm bảo chuyển trạng thái đúng sau khi tiếp đất
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            bool isMoving = (new Vector3(horizontal, 0, vertical)).sqrMagnitude > 0f;
            animator.SetBool("IsMoving", isMoving);
        }
    }
    public void EndAttack()
    {
        Debug.Log("EndAttack called");
        isAttacking = false;
        animator.SetBool("IsAttacking", false);
    }
}