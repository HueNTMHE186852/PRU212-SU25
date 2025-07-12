using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public SharedPlayerStats sharedStats = new SharedPlayerStats();

    [Header("Character Stat Assets")]
    public CharacterStatsSO auronStats;
    public CharacterStatsSO helronStats;

    // Dùng để chứa các bản gốc
    public List<CharacterStatsSO> characterStatAssets;

    // Dùng để clone (runtime)
    public CharacterStatsSO selectedRuntimeStats;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Load dữ liệu đã lưu (nếu có)
            SharedPlayerStats.GameStats.sharedStats = SharedPlayerStats.LoadFromJson();

            // Khởi tạo danh sách asset (nếu chưa)
            if (characterStatAssets == null || characterStatAssets.Count == 0)
            {
                characterStatAssets = new List<CharacterStatsSO> { auronStats, helronStats };
            }

            // Lấy nhân vật được chọn từ PlayerPrefs
            string selectedName = PlayerPrefs.GetString("SelectedCharacter", "Auron");
            CharacterStatsSO selectedBase = characterStatAssets.Find(c => c.characterName == selectedName);

            // Clone để dùng trong game (runtime)
            selectedRuntimeStats = ScriptableObject.Instantiate(selectedBase);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
