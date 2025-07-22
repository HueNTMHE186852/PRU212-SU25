using UnityEngine;

public class WalkingState : IDragonState
{
    private DragonController controller;
    private bool hasStartedMoving = false;

    public WalkingState(DragonController ctrl) => controller = ctrl;

    public void Enter(Player1 player1, AuronPlayerController player2)
    {
        controller.animator.SetBool("IsWalking", true);
        hasStartedMoving = false;
    }

    public void Update()
    {
        AnimatorStateInfo state = controller.animator.GetCurrentAnimatorStateInfo(0);
        if (!hasStartedMoving && state.IsName("Walk"))
        {
            hasStartedMoving = true;
        }

        if (!hasStartedMoving)
            return;

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

        if (!controller.CanSeePlayer())
        {
            controller.TransitionToState(controller.idleState);
            return;
        }

        Vector3 dir = (controller.player.position - controller.transform.position).normalized;
        controller.FaceDirection(dir);
        controller.transform.position += dir * Time.deltaTime * 5f;
    }

    public void Exit()
    {
        controller.animator.SetBool("IsWalking", false);
    }
}
