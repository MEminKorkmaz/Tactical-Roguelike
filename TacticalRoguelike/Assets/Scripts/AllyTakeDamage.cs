using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyTakeDamage : MonoBehaviour
{

    public AllyStats allyStats;

    // public int currentHealth;

    public GameObject DeathFxPrefab;


    void Awake(){
        allyStats = this.GetComponent<AllyStats>();
    }

    void Start(){
        allyStats.CurrentHealth = allyStats.MaxHealth;
    }
    public void GetDamage(int Damage){
        // int rnd = Random.Range(0 , 100);
        // if(rnd <= allyStats.Evasion){
        //     // Debug.Log("Miss");
        //     return;
        // }
        // Damage = Damage - ((Damage * allyStats.Defence) / 100);
        // Debug.Log(Damage);
        allyStats.CurrentHealth -= Damage;

        CheckIfDead();
    }

    void CheckIfDead(){
        if(allyStats.CurrentHealth <= 0){
            GameObject go = Instantiate(DeathFxPrefab , transform.position , transform.rotation);
            Destroy(go , 5f);
            Destroy(gameObject);
        }
    }
}
