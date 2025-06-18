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

    private float idleTimer = 0f;
    private const float idleDuration = 10f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        switch (currentState)
        {
            case DragonState.Idle:
                idleTimer += Time.deltaTime;
                if (idleTimer >= idleDuration)
                {
                    EnterDie();
                }
                break;
        }
    }

    void EnterDie()
    {
        currentState = DragonState.Dying;
        animator.SetTrigger("isDying");
        StartCoroutine(DisappearAfterAnimation());
    }

    private System.Collections.IEnumerator DisappearAfterAnimation()
    {
        // Wait for the dying animation to finish
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float dieAnimLength = stateInfo.length > 0 ? stateInfo.length : 1f;
        yield return new WaitForSeconds(dieAnimLength);

        // Pause on the last frame of the dying animation
        animator.speed = 0f;

        // Fade out
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            float fadeDuration = 1f;
            float elapsed = 0f;
            Color originalColor = sr.color;
            while (elapsed < fadeDuration)
            {
                float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                elapsed += Time.deltaTime;
                yield return null;
            }
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        }

        gameObject.SetActive(false);
    }
}
