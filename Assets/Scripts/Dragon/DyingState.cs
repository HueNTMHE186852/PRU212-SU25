using UnityEngine;

public class DyingState : IDragonState
{
    private DragonController controller;

    public DyingState(DragonController ctrl) => controller = ctrl;

    public void Enter(Player1 player1, AuronPlayerController player2)
    {
        controller.animator.SetTrigger("Death");

        if (GameProgress.Current.currentLevel != 4)
        {
                if (player1 != null)
                    player1.Win();
                if (player2 != null)
                    player2.Win();
        }

        GameObject.Destroy(controller.gameObject, 1.5f);
    }


    public void Update() { }

    public void Exit() { }
}
