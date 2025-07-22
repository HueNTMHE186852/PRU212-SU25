using UnityEngine;

public class DyingState : IDragonState
{
    private DragonController controller;

    public DyingState(DragonController ctrl) => controller = ctrl;

    public void Enter()
    {
        controller.animator.SetTrigger("Death");

        GameObject.Destroy(controller.gameObject, 1.5f);
    }


    public void Update() { }

    public void Exit() { }
}
