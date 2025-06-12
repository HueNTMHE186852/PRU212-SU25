using UnityEngine;

public class AttackColliderTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("💥 Player bị trúng đòn từ AttackZone!");
        }
    }
}
