using UnityEngine;

[System.Serializable]
public class SpawnZone
{
    public string zoneName;
    public BoxCollider2D area; // Vùng hình chữ nhật
    public GameObject[] enemyTypes; // Loại quái được phép spawn ở vùng này
    public int spawnCount = 5;
}
