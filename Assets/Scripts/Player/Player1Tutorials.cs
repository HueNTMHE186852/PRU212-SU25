using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static SharedPlayerStats;



public class Player1Tutorial : MonoBehaviour
{
    public TextMeshProUGUI Player1Hp;
    public TextMeshProUGUI Player1Mp;

    public FinalPlayerStats finalStats = new FinalPlayerStats();
    public BasePlayerStats baseStats;

    // Start is called before the first frame update
    void Start()
    {

        baseStats = GameManager.Instance.helronStats.baseStats;

        SharedPlayerStats sharedStats = SharedPlayerStats.GameStats.sharedStats;

        finalStats.Calculate(baseStats, sharedStats);
        Player1Hp.text = "MP: " + finalStats.MaxHP;
        Player1Mp.text = "HP: " + finalStats.MaxMP;

    }

    // Update is called once per frame
    void Update()
    {

    }
}
