using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackTrigger : MonoBehaviour
{
	public int damage = 15;
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			Player1 player = collision.GetComponentInParent<Player1>();
			player.TakeDamage(damage);
			Debug.Log("💥 Player trúng 15 dame ");
		}
	}
}
