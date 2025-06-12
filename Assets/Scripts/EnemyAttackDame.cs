using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackDame : MonoBehaviour
{
    public float damage = 10f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Gây damage cho player
            Debug.Log($"Player nhận {damage} damage!");

            // Thêm logic gây damage ở đây
            // Ví dụ: other.GetComponent<PlayerHealth>()?.TakeDamage(damage);
        }
    }
}
