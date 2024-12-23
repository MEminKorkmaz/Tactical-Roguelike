using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyBuffs : MonoBehaviour
{
    public AllyStats allyStats;
    public TurnManager turnManager;

    [Header("Effects")]
    public GameObject HealingFxPrefab;



    void Awake(){
        allyStats = this.GetComponent<AllyStats>();

        turnManager = GameObject.FindWithTag("GameManager").GetComponent<TurnManager>();
    }
    
    public void Healing(int HealingAmount){
        allyStats.CurrentHealth += HealingAmount;
        if(allyStats.CurrentHealth > allyStats.MaxHealth)
        allyStats.CurrentHealth = allyStats.MaxHealth;

        turnManager.isDuringTurn = false;

        GameObject go = Instantiate(HealingFxPrefab , transform.position , transform.rotation);
        Destroy(go , 5f);
    }
}
