using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    public Transform player;
    [Tooltip("How far in front/above the player the minimap camera should sit")]
    public Vector3 offset = new Vector3(0f, 5f, 0f);   // tweak in Inspector

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 newPos = player.position + offset;
        newPos.z = transform.position.z;   // keep original z (‑10, etc.)
        transform.position = newPos;
    }
}
