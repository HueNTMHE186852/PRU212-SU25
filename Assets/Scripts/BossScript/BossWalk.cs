using UnityEngine;

public class BossWalk : StateMachineBehaviour
{
    private BossAI boss;
    private Rigidbody2D rb;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss = animator.GetComponent<BossAI>();
        rb = boss.rb;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (boss.player == null) return;

        float distanceX = Mathf.Abs(boss.transform.position.x - boss.player.position.x);

        if (distanceX > boss.attackRange)
        {
            Vector2 target = new Vector2(boss.player.position.x, rb.position.y);
            Vector2 newPos = Vector2.MoveTowards(rb.position, target, boss.speed * Time.deltaTime);
            rb.MovePosition(newPos);
        }
        else
        {
            animator.SetTrigger("meleeAttack");
        }
    }
}
