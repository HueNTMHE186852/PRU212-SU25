using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private bool hasHit = false;
    public int damage = 10;
    public GameObject explosionEffectPrefab; // Prefab hiệu ứng nổ (chỉ là explosion, không phải arrow)
    public bool isQSkillArrow = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasHit) return;
        AudioManager.Instance.PlaySFX("PlayerArrowHit");

        // Va chạm với Enemy hoặc Boss
        if (collision.gameObject.CompareTag("Enemy"))
        {
            hasHit = true;
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
            animator.SetTrigger("StickToEnemy");

            if (collision.contacts.Length > 0)
            {
                Vector2 hitPoint = collision.contacts[0].point;
                transform.position = hitPoint;

                if (isQSkillArrow && explosionEffectPrefab != null)
                {
                    var sr = GetComponent<SpriteRenderer>();
                    if (sr != null) sr.enabled = false;
                    GameObject effect = Instantiate(explosionEffectPrefab, hitPoint, transform.rotation);
                    effect.transform.localScale *= 11f;
                    var effectSR = effect.GetComponent<SpriteRenderer>();
                    if (effectSR != null)
                    {
                        effectSR.sortingLayerName = "Default";
                        effectSR.sortingOrder = 10;
                    }
                    Destroy(effect, 1f);
                }
            }

            transform.parent = collision.transform;

            // Gây dame cho các loại boss và enemy
            BossAI bossAI = collision.gameObject.GetComponent<BossAI>();
            if (bossAI != null)
            {
                float direction = collision.transform.position.x > transform.position.x ? 1f : -1f;
                float knockbackForce = 5f;
                Vector2 knockback = new Vector2(direction * knockbackForce, 0f);
                bossAI.ApplyKnockback(knockback);
                bossAI.TakeDamage(damage);
            }

            EnemyRun enemyRun = collision.gameObject.GetComponent<EnemyRun>();
            if (enemyRun != null)
            {
                enemyRun.TakeDamage(damage);
            }

            ForestBoss forestBoss = collision.gameObject.GetComponent<ForestBoss>();
            if (forestBoss != null)
            {
                forestBoss.TakeDamage(damage);
            }

            DarkBoss darkBoss = collision.gameObject.GetComponent<DarkBoss>();
            if (darkBoss != null)
            {
                darkBoss.TakeDamage(damage);
            }
            DragonController dragonController = collision.gameObject.GetComponent<DragonController>();
            if (dragonController != null)
            {
                dragonController.TakeDamage(damage);
            }

            GetComponent<Collider2D>().enabled = false;
            Destroy(gameObject, 1f);
            return;
        }

        // Va chạm với Ground hoặc Tilemap
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Tilemap"))
        {
            hasHit = true;
            rb.velocity = Vector2.zero;
            rb.isKinematic = false;
            rb.bodyType = RigidbodyType2D.Static;

            if (collision.contacts.Length > 0)
            {
                Vector2 hitPoint = collision.contacts[0].point;
                Vector2 hitNormal = collision.contacts[0].normal;
                transform.position = hitPoint;

                // Xoay mũi tên theo hướng tiếp xúc mặt đất
                float angle = Mathf.Atan2(hitNormal.y, hitNormal.x) * Mathf.Rad2Deg - 90f;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }

            transform.parent = collision.transform;
            GetComponent<Collider2D>().enabled = false;
            Destroy(gameObject, 1f);
            return;
        }
    }


}
