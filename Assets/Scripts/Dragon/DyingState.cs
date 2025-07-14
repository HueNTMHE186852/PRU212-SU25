using UnityEngine;

public class DyingState : IDragonState
{
    private DragonController controller;

    public DyingState(DragonController ctrl) => controller = ctrl;

    public void Enter()
    {
        controller.animator.SetTrigger("Death");

        //// Rơi item
        //float r = Random.Range(0f, 1f);
        //if (r < controller.dropItemChance)
        //{
        //    GameObject prefab = (r < controller.dropItemChance / 2f)
        //        ? controller.hpBowlPrefab
        //        : controller.manaBowlPrefab;

        //    GameObject.Instantiate(prefab, controller.transform.position, Quaternion.identity);
        //}
        GameManager.Instance.OnBossDefeated();
        GameObject.Destroy(controller.gameObject, 1.5f);
    }


    public void Update() { }

    public void Exit() { }
}
