using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTakeDamage : MonoBehaviour
{

    public EnemyStats enemyStats;

    // public int currentHealth;

    public GameObject DeathFxPrefab;

    public EnemyAIManager enemyAIManager;


    void Awake(){
        enemyStats = this.GetComponent<EnemyStats>();

        enemyAIManager = GameObject.FindWithTag("GameManager").GetComponent<EnemyAIManager>();
    }

    void Start(){
        enemyStats.CurrentHealth = enemyStats.MaxHealth;
    }
    public void GetDamage(int Damage){
        // int rnd = Random.Range(0 , 100);
        // if(rnd <= enemyStats.Evasion){
        //     // Debug.Log("Miss");
        //     return;
        // }
        // Damage = Damage - ((Damage * enemyStats.Defence) / 100);
        // Debug.Log(Damage);
        enemyStats.CurrentHealth -= Damage;

        CheckIfDead();
    }

    void CheckIfDead(){
        if(enemyStats.CurrentHealth <= 0){
            GameObject go = Instantiate(DeathFxPrefab , transform.position , transform.rotation);
            Destroy(go , 5f);
            // enemyAIManager.DoEnemyCount();
            Destroy(gameObject);
        }
    }
}
