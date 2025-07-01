using UnityEngine;

public class AttackingState : IDragonState
{
    private DragonController controller;

    public AttackingState(DragonController ctrl) => controller = ctrl;

    public void Enter()
    {
        controller.lastAttackTime = Time.time;
        controller.animator.ResetTrigger("Attack");
        controller.animator.SetTrigger("Attack");

    }


    public void Update()
    {
        Vector3 dir = (controller.player.position - controller.transform.position).normalized;
        controller.FaceDirection(dir);
    }

    public void Exit()
    {
    }

    public void OnAttackEnd()
    {
        controller.normalAttackCount++;

        if (controller.normalAttackCount >= controller.incinerationEvery)
        {
            controller.normalAttackCount = 0;
            controller.TransitionToState(controller.incineratingState);
        }
        else
        {
            controller.TransitionToState(controller.idleState);
        }
    }
}
