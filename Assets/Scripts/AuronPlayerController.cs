using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuronPlayerController : MonoBehaviour
{
    private Animator animator;
    public float moveSpeed = 3f;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(horizontal, 0, vertical);

        bool isMoving = movement.sqrMagnitude > 0f;
        animator.SetBool("IsMoving", isMoving);

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
    }
}
