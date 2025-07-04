using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    public GameObject auronPrefab;
    public GameObject helronPrefab;
    public Transform spawnPoint;

    void Start()
    {
        string selected = PlayerPrefs.GetString("SelectedCharacter", "Auron");

        GameObject prefabToSpawn = null;

        if (selected == "Auron")
        {
            prefabToSpawn = auronPrefab;
        }
        else if (selected == "Helron")
        {
            prefabToSpawn = helronPrefab;
        }

        if (prefabToSpawn != null && spawnPoint != null)
        {
            Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
        }
        else
        {
            Debug.LogWarning("Character prefab or spawnPoint missing!");
        }
    }
}

