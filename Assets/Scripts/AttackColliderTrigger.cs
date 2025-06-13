using UnityEngine;

public class AttackColliderTrigger : MonoBehaviour
{
<<<<<<< HEAD
    Player1 player = new Player1();
=======
    public int damage = 10; // Số damage gây ra

>>>>>>> f7b46e86ceef7ecbd564988dbdb067a7d7a22e8b
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
<<<<<<< HEAD
         
            Debug.Log("💥 Player trúng 10 dame " );
=======
            Debug.Log("💥 Player bị trúng đòn từ AttackZone!");

            Player1 player = collision.GetComponent<Player1>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
>>>>>>> f7b46e86ceef7ecbd564988dbdb067a7d7a22e8b
        }
    }
}
