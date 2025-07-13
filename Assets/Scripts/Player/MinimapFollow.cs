using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MinimapFollow : MonoBehaviour
{
    public string playerTag = "Player";
    public Vector3 offset = new Vector3(0f, 20f, -10f);
    public float fixedOrthographicSize = 90f; // Đặt mức zoom mong muốn

    private Transform player;
    private Camera minimapCam;

    void Start()
    {
        minimapCam = GetComponent<Camera>();
        minimapCam.orthographic = true;
        minimapCam.orthographicSize = fixedOrthographicSize;

        FindPlayer();
    }

    void LateUpdate()
    {
        if (player == null)
        {
            FindPlayer();
            return;
        }

        // Theo dõi player với offset
        Vector3 newPos = player.position + offset;
        transform.position = newPos;

        // Đảm bảo không bị thay đổi size
        minimapCam.orthographicSize = fixedOrthographicSize;
    }

    void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }
}
