using UnityEngine;

public class AnimationRelay : MonoBehaviour
{
    private DragonController controller;

    void Awake()
    {
        controller = GetComponentInParent<DragonController>();
    }

    public void OnAttackEnd()
    {
        controller?.OnAttackEnd();
    }
}
