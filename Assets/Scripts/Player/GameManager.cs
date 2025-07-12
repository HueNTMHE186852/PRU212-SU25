using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public SharedPlayerStats sharedStats = new SharedPlayerStats();

    [Header("Character Stat Assets")]
    public CharacterStatsSO auronStats;
    public CharacterStatsSO helronStats;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Load dữ liệu đã lưu (nếu có)
            SharedPlayerStats.GameStats.sharedStats = SharedPlayerStats.LoadFromJson();

            auronStats.LoadFromJson();
            helronStats.LoadFromJson();

        }
        else
        {
            Destroy(gameObject);
        }
    }
}
