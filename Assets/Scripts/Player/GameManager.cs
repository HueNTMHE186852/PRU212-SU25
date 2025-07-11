using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public SharedPlayerStats sharedStats = new SharedPlayerStats();

    [Header("Character Stat Assets")]
    public CharacterStatsSO auronStats;
    public CharacterStatsSO helronStats;

    public CharacterStatsSO SelectedCharacterStats { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSelectedCharacterStats();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadSelectedCharacterStats()
    {
        string selected = PlayerPrefs.GetString("SelectedCharacter", "Auron");

        if (selected == "Helron")
            SelectedCharacterStats = helronStats;
        else
            SelectedCharacterStats = auronStats;

        Debug.Log("Loaded stats for: " + selected);
    }
}
