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
        // Cận chiến (X)
        if (Input.GetKeyDown(KeyCode.X))
        {
            animator.SetBool("IsAttacking", true);
            Attack(damage, 5f, 1f);
        }
        else if (Input.GetKeyUp(KeyCode.X))
        {
            animator.SetBool("IsAttacking", false);
        }

        // Bắn cung (chuột trái)
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("BowShoot");
            FireArrow();
        }

        // Skill E
        if (Input.GetKeyDown(KeyCode.E))
        {
            animator.SetTrigger("IsAttacking2");
            Attack(damage * 2, 7f, 1.5f);
        }
    }

    void Attack(int dmg, float radius, float offsetX)
    {
        Vector3 center = transform.position + new Vector3(spriteRenderer.flipX ? -offsetX : offsetX, 0, 0);
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius);

        HashSet<GameObject> damagedRoots = new HashSet<GameObject>();

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            Transform parent = hit.transform.parent;
            if (parent == null) continue;

            GameObject root = parent.gameObject;

            if (!damagedRoots.Contains(root))
            {
                DamageManager.ApplyDamage(hit.gameObject, dmg); // Truyền hit.gameObject để giữ quy tắc tag và parent
                damagedRoots.Add(root);
            }
        }
    }


    void FireArrow()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        spriteRenderer.flipX = (mouseWorldPos.x < transform.position.x);

        Vector3 firePointLocalPos = firePoint.localPosition;
        firePointLocalPos.x = Mathf.Abs(firePointLocalPos.x) * (spriteRenderer.flipX ? -1 : 1);
        firePoint.localPosition = firePointLocalPos;

        Vector2 shootDir = (mouseWorldPos - firePoint.position).normalized;
        float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;

        firePoint.rotation = Quaternion.Euler(0, 0, angle);

        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.Euler(0, 0, angle));
        arrow.GetComponent<Arrow>().damage = damage;
        arrow.GetComponent<Rigidbody2D>().velocity = shootDir * arrowForce;
        arrow.transform.localScale = new Vector3(5f, 5f, 1f);
        Destroy(arrow, 1f);
    }
}
