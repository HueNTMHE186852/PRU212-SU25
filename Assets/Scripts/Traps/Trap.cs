using UnityEngine;

public class Trap : MonoBehaviour
{
    public int damage = 50; // Sát thương trap gây ra
    private GameObject playerObj = null;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || (other.transform.parent != null && other.transform.parent.CompareTag("Player")))
        {
            playerObj = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (playerObj == other.gameObject)
        {
            playerObj = null;
        }
    }

    public void DealDamageToPlayer()
    {
        if (playerObj == null) return;

        AuronPlayerController player = playerObj.GetComponent<AuronPlayerController>();
        if (player == null)
            player = playerObj.GetComponentInParent<AuronPlayerController>();
        if (player != null)
        {
            player.TakeDamage(damage);
            return;
        }
        Player1 player1 = playerObj.GetComponent<Player1>();
        if (player1 == null)
            player1 = playerObj.GetComponentInParent<Player1>();
        if (player1 != null)
        {
            player1.TakeDamage(damage);
        }
    }




}
