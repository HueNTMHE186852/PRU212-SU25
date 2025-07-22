using UnityEngine;

public class IdleState : IDragonState
{
    private DragonController controller;

    public IdleState(DragonController ctrl) => controller = ctrl;

    public void Enter() => controller.animator.Play("Idle");

    public void Update()
    {
        if (controller.isDead)
        {
            controller.TransitionToState(controller.dyingState);
            return;
        }

        if (controller.CanAttack())
        {
            controller.TransitionToState(controller.attackingState);
            return;
        }

        if (controller.CanSeePlayer())
        {
            controller.TransitionToState(controller.walkingState);
            return;
        }

        if (controller.player == null) return; 

        Vector3 dir = (controller.player.position - controller.transform.position).normalized;
        controller.FaceDirection(dir);
    }

    public void Exit() { }
}
