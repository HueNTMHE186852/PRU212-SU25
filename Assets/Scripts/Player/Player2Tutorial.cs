using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static SharedPlayerStats;



public class Player2Tutorial : MonoBehaviour
{
    public TextMeshProUGUI Player2Hp;
    public TextMeshProUGUI Player2Mp;

    public FinalPlayerStats finalStats = new FinalPlayerStats();
    public BasePlayerStats baseStats;

    // Start is called before the first frame update
    void Start()
    {

        baseStats = GameManager.Instance.auronStats.baseStats;

        SharedPlayerStats sharedStats = SharedPlayerStats.GameStats.sharedStats;

        finalStats.Calculate(baseStats, sharedStats);
        Player2Hp.text ="MP: " + finalStats.MaxHP;
        Player2Mp.text ="HP: " + finalStats.MaxMP;

    }

    // Update is called once per frame
    void Update()
    {

    }
}
