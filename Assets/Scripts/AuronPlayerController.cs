using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuronPlayerController : MonoBehaviour
{
    private Animator animator;
    public float moveSpeed = 3f;
    public GameObject arrowPrefab; // Prefab mũi tên
    public Transform firePoint;    // Vị trí xuất phát mũi tên
    public float arrowForce = 10f; // Lực bắn mũi tên
    public float fireRate = 0.5f;  // Thời gian giữa các lần bắn

    private Rigidbody2D rb;

    public float jumpForce = 7f;
    private bool isGrounded = true;
    private bool isAttacking = false;
    private bool isDefending = false; // Add defend state

    public GameObject arrowFallEffectPrefab; // Prefab hiệu ứng cung rơi
    public Transform arrowFallSpawnPoint;    // Vị trí rơi xuống (có thể là ground hoặc vị trí chỉ định)


    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(horizontal, vertical).normalized;
        bool isMoving = movement.sqrMagnitude > 0f;
        animator.SetBool("IsMoving", isMoving);


        // Đòn đánh tay (ví dụ phím X)
        if (Input.GetKeyDown(KeyCode.X))
        {
            isAttacking = true;
            animator.SetBool("IsAttacking", true);
        }
        else if (Input.GetKeyUp(KeyCode.X))
        {
            isAttacking = false;
            animator.SetBool("IsAttacking", false);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("SetTrigger BowShoot");
            animator.SetTrigger("BowShoot");

        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("SetTrigger SkillAttack");
            animator.SetTrigger("IsAttacking2");
        }


        // Defend (hold right mouse button)
        isDefending = Input.GetMouseButton(1);
        animator.SetBool("IsDefending", isDefending);

        if (horizontal != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = horizontal > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }

        // Di chuyển bằng Rigidbody2D (chuẩn vật lý)
        rb.velocity = new Vector2(movement.x * moveSpeed, rb.velocity.y);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
            animator.SetBool("IsJumping", true);
        }


    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Tilemap"))
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
    public void FireArrow()
    {
        Debug.Log("✅ FireArrow() được gọi!");

        // Xác định hướng dựa vào hướng nhân vật
        float direction = transform.localScale.x > 0 ? 1f : -1f;
        Vector2 shootDir = new Vector2(direction, 0f);

        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();
        arrowRb.velocity = shootDir * arrowForce;

        // Xoay mũi tên cho đúng hướng
        float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.Euler(0, 0, angle);

        // Lật sprite nếu bắn sang trái (nếu cần)
        if (direction < 0)
            arrow.transform.localScale = new Vector3(-1f, 1f, 1f);
        else
            arrow.transform.localScale = new Vector3(1f, 1f, 1f);

        Debug.Log("🚀 Arrow bắn ra hướng: " + shootDir);
        Destroy(arrow, 2f);
    }


    public void TestEvent()
    {
        Debug.Log("✅ TestEvent() được gọi!");
    }
    public void SpawnArrowFallEffect()
    {
        if (arrowFallEffectPrefab != null && arrowFallSpawnPoint != null)
        {
            GameObject effect = Instantiate(arrowFallEffectPrefab, arrowFallSpawnPoint.position, Quaternion.identity);
            Destroy(effect, 1f);
        }
    }

}
