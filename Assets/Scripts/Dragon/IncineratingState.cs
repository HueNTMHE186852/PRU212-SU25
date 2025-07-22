using UnityEngine;
using System.Collections;

public class IncineratingState : IDragonState
{
    private DragonController controller;
    private Coroutine loopRoutine;

    public IncineratingState(DragonController ctrl) => controller = ctrl;

    public void Enter()
    {
        controller.animator.Play("Cast", 0, 0f);
        loopRoutine = controller.StartCoroutine(LoopFrame5To6());
    }

    public void Update() { }

    public void Exit()
    {
        if (loopRoutine != null)
            controller.StopCoroutine(loopRoutine);

        controller.incinerationEffect?.Stop();
        controller.DisableFireZoneCollider();
    }

    private IEnumerator LoopFrame5To6()
    {
        float clipLength = controller.animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        float frameTime = clipLength / 12f;

        yield return new WaitForSeconds(frameTime * 5);

        float loopDuration = 8f;
        float elapsed = 0f;

        // 🔊 Phát SFX lửa
        SFXController.Instance?.Play("FireBreath");
        Debug.Log("Playing FireBreath SFX");
        while (elapsed < loopDuration)
        {
            controller.incinerationEffect?.Play();
            controller.EnableFireZoneCollider();
            controller.animator.Play("Cast", 0, 5f / 12f);
            yield return new WaitForSeconds(frameTime);
            controller.animator.Play("Cast", 0, 6f / 12f);
            yield return new WaitForSeconds(frameTime);
            elapsed += frameTime * 2;
        }

        controller.TransitionToState(controller.idleState);
    }

}
