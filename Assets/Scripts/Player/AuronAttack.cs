using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Animator))]
public class AuronAttack : MonoBehaviour
{
    public int damage = 10;
    public GameObject arrowPrefab;
    public Transform firePoint;
    public float arrowForce = 10f;
    public GameObject arrowFallEffectPrefab;
    public Transform groundCheckPoint;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Attack
        if (Input.GetKeyDown(KeyCode.X))
        {
            animator.SetBool("IsAttacking", true);
            Attack();
        }
        else if (Input.GetKeyUp(KeyCode.X))
        {
            animator.SetBool("IsAttacking", false);
        }

        // Bow shoot
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("BowShoot");
            FireArrow();
        }

        // Skill E
        if (Input.GetKeyDown(KeyCode.E))
        {
            animator.SetTrigger("IsAttacking2");
            SkillE();
        }
    }

    void Attack()
    {
        float attackRadius = 5f;
        float attackOffsetX = 1.0f;
        Vector3 attackCenter = transform.position + new Vector3(spriteRenderer.flipX ? -attackOffsetX : attackOffsetX, 0, 0);

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackCenter, attackRadius);
        HashSet<EnemyRun> damagedEnemies = new HashSet<EnemyRun>();
        foreach (var hit in hits)
        {
            EnemyRun enemy = hit.GetComponent<EnemyRun>();
            if (enemy == null)
                enemy = hit.GetComponentInParent<EnemyRun>();

            if (enemy != null && enemy.gameObject.CompareTag("Enemy") && !damagedEnemies.Contains(enemy))
            {
                enemy.TakeDamage(damage);
                damagedEnemies.Add(enemy);
            }
        }
    }

    void FireArrow()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        if (mouseWorldPos.x < transform.position.x)
            spriteRenderer.flipX = true;
        else
            spriteRenderer.flipX = false;

        Vector3 firePointLocalPos = firePoint.localPosition;
        firePointLocalPos.x = Mathf.Abs(firePointLocalPos.x) * (spriteRenderer.flipX ? -1 : 1);
        firePoint.localPosition = firePointLocalPos;

        Vector2 shootDir = (mouseWorldPos - firePoint.position).normalized;
        float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;
        firePoint.rotation = Quaternion.Euler(0, 0, angle);

        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.Euler(0, 0, angle));
        arrow.GetComponent<Arrow>().damage = damage;
        Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();
        arrowRb.velocity = shootDir * arrowForce;
        arrow.transform.localScale = new Vector3(5f, 5f, 1f);
        Destroy(arrow, 1f);
    }

    void SkillE()
    {
        float eSkillRadius = 7f;
        float eSkillOffsetX = 1.5f;
        Vector3 eSkillCenter = transform.position + new Vector3(spriteRenderer.flipX ? -eSkillOffsetX : eSkillOffsetX, 0, 0);

        Collider2D[] hits = Physics2D.OverlapCircleAll(eSkillCenter, eSkillRadius);
        HashSet<EnemyRun> damagedEnemies = new HashSet<EnemyRun>();
        foreach (var hit in hits)
        {
            EnemyRun enemy = hit.GetComponent<EnemyRun>();
            if (enemy == null)
                enemy = hit.GetComponentInParent<EnemyRun>();

            if (enemy != null && enemy.gameObject.CompareTag("Enemy") && !damagedEnemies.Contains(enemy))
            {
                enemy.TakeDamage(damage * 2);
                damagedEnemies.Add(enemy);
            }
        }
    }
}
