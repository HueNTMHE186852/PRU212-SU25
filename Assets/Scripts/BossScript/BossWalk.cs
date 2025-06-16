using UnityEngine;

public class BossWalk : StateMachineBehaviour
{
	private BossAI boss;
	private Rigidbody2D rb;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		boss = animator.GetComponent<BossAI>();
		rb = boss.rb;

		// Reset trigger tấn công khi mới bắt đầu trạng thái Walk/Run
		animator.ResetTrigger("meleeAttack");
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (boss.player == null || !boss.player.gameObject.activeInHierarchy) return;

		float distanceX = Mathf.Abs(boss.transform.position.x - boss.player.position.x);

		// Nếu trong phạm vi tấn công thì mới đánh
		if (distanceX <= boss.attackRange)
		{
			// Set trigger để chuyển sang trạng thái tấn công
			animator.SetTrigger("meleeAttack");
		}
		else
		{
			// Ngược lại thì tiếp tục di chuyển
			Vector2 target = new Vector2(boss.player.position.x, rb.position.y);
			Vector2 newPos = Vector2.MoveTowards(rb.position, target, boss.speed * Time.deltaTime);
			rb.MovePosition(newPos);
		}
	}
}
