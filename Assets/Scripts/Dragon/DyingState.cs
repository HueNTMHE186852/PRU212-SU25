using UnityEngine;

public class DyingState : IDragonState
{
    private DragonController controller;

    public DyingState(DragonController ctrl) => controller = ctrl;

    public void Enter(Player1 player1, AuronPlayerController player2)
    {
        controller.animator.SetTrigger("Death");

        if (player1 != null)
        {
            player1.Win();
            player1.coinManager.AddCoin(500);
        }
        if (player2 != null)
        {
            player2.Win();
            player2.coinManager.AddCoin(500);
        }
        GameProgress.Current.CompleteLevel(4, GameManager.Instance.PlayTimeSeconds);
        GameObject.Destroy(controller.gameObject, 1.5f);
    }


    public void Update() { }

    public void Exit() { }
}
