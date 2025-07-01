using UnityEngine;

public class DragonAnimationEvents : MonoBehaviour
{
    public DragonController controller;

    public void OnAttackFrameStart() => controller?.EnableNormalAttackCollider();
    public void OnAttackFrameEnd() => controller?.DisableNormalAttackCollider();
}
