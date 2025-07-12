using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    public string playerTag = "Player";
    public Vector3 offset = new Vector3(0f, 20f, -10f); 

    private Transform player;

    void Start()
    {
        FindPlayer();
    }

    void LateUpdate()
    {
        if (player == null)
        {
            FindPlayer();
            return;
        }

        Vector3 newPos = player.position + offset;
        transform.position = newPos;
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
