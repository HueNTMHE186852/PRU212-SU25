using UnityEngine;

public class AttackColliderTrigger : MonoBehaviour
{
    Player1 player = new Player1();
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
         
            Debug.Log("💥 Player trúng 10 dame " );
        }
    }
}
