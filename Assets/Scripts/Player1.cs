﻿using UnityEngine;
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
    private bool isDead = false;
    public Transform GroundCheck;
    //public Transform attackHitbox;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public Player1Healthbar healthBar;
    public Player1MPBar MPBar;
    public PlayerAttackTrigger attackTrigger;

    public int eSkillMPCost = 20;
    public int qSkillMPCost = 30;
    public float mpRegenRate = 5f; 
    private float mpRegenTimer = 0f;
    public int damage = 10;

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
    [SerializeField] 
    private Transform attackCollider; 
    private Vector2 movement;
    [SerializeField] private Vector3 attackColliderRightPos = new Vector3(-0.93f, 0f, 0f);
    [SerializeField] private Vector3 attackColliderLeftPos = new Vector3(-1.86f, 0f, 0f);




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

    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthBar != null)
        {
            healthBar.SetHealth((float)currentHealth / maxHealth);
        }
        else
        {
            Debug.LogWarning("healthBar is not assigned in Player1.");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("Hurt");
        }

    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        // Stop attacking
        isAttacking = false;
        animator.SetTrigger("Die");

        // Stop physics motion and freeze the player
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static; // Freeze position
        }

        Destroy(gameObject, 1f);
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

        bool wasFlipped = spriteRenderer.flipX;

        if (movement.x < 0 && !wasFlipped)
        {
            FlipCharacter(true);
        }
        else if (movement.x > 0 && wasFlipped)
        {
            FlipCharacter(false);
        }



        // Animator parameters
        animator.SetBool("isMoving", movement.x != 0);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("VerticalVelocity", rb.velocity.y);
    }
    void FlipCharacter(bool facingLeft)
    {
        spriteRenderer.flipX = facingLeft;

        if (attackCollider != null)
        {
            attackCollider.localPosition = facingLeft ? attackColliderLeftPos : attackColliderRightPos;
            Debug.Log($"🔁 Hardcoded flip to {(facingLeft ? "LEFT" : "RIGHT")}, new pos: {attackCollider.localPosition}");
        }
    }

    public void EnableAttackCollider()
    {
        if (attackCollider == null) return;

        attackCollider.localPosition = spriteRenderer.flipX ? attackColliderLeftPos : attackColliderRightPos;

        var col = attackCollider.GetComponent<Collider2D>();
        if (col != null) col.enabled = true;

        Debug.Log($"✅ Attack collider enabled at {attackCollider.localPosition}");
    }

    public void DisableAttackCollider()
    {
        if (attackCollider == null) return;

        BoxCollider2D col = attackCollider.GetComponent<BoxCollider2D>();
        if (col != null)
            col.enabled = false;

        Debug.Log("🛑 Collider disabled");
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
    public void SetSkillType(string skillName)
    {
        if (attackTrigger == null)
        {
            Debug.LogWarning("⚠️ Player1: attackTrigger chưa được gán!");
            return;
        }

        if (System.Enum.TryParse(skillName, out PlayerAttackTrigger.SkillType parsedSkill))
        {
            attackTrigger.skillType = parsedSkill;
            Debug.Log("✅ Skill type set to: " + parsedSkill);
        }
        else
        {
            Debug.LogWarning("❌ Không thể chuyển đổi skill: " + skillName);
        }
    }
}
