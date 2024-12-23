using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBuffs : MonoBehaviour
{
    public EnemyStats enemyStats;
    public TurnManager turnManager;

    [Header("Effects")]
    public GameObject HealingFxPrefab;



    void Awake(){
        enemyStats = this.GetComponent<EnemyStats>();

        turnManager = GameObject.FindWithTag("GameManager").GetComponent<TurnManager>();
    }
    
    public void Healing(int HealingAmount){
        enemyStats.CurrentHealth += HealingAmount;
        if(enemyStats.CurrentHealth > enemyStats.MaxHealth)
        enemyStats.CurrentHealth = enemyStats.MaxHealth;

        turnManager.isDuringTurn = false;

        GameObject go = Instantiate(HealingFxPrefab , transform.position , transform.rotation);
        Destroy(go , 5f);
    }
}
