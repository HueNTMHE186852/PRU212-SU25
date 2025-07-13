using UnityEngine;

public class Trap : MonoBehaviour
{
    public int damage = 50; // Sát thương trap gây ra

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trap triggered by: " + other.name + ", tag: " + other.tag);
        if (other.CompareTag("Player") || (other.transform.parent != null && other.transform.parent.CompareTag("Player")))
        {
            AuronPlayerController player = other.GetComponent<AuronPlayerController>();
            if (player == null)
                player = other.GetComponentInParent<AuronPlayerController>();
            if (player != null)
            {
                Debug.Log("Trap hit player, damage: " + damage);
                player.TakeDamage(damage);
            }
        }
    }



}
