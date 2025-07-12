using UnityEngine;

[CreateAssetMenu(menuName = "Player/BaseStats")]
public class CharacterStatsSO : ScriptableObject
{
    public string characterName;
    public BasePlayerStats baseStats;
}
