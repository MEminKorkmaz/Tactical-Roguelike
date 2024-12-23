using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIManager : MonoBehaviour
{
    public GameObject[] Enemies;

    public int EnemyIndex = 0;

    public TurnManager turnManager;

    public Pathfinding pf;

    public bool areAllEnemiesDone;

    void Update(){
        // Debug.Log(pf.EnemyActionTile.Count);
    }

    void Awake(){
        turnManager = this.GetComponent<TurnManager>();

        pf = GameObject.FindWithTag("Pathfinding").GetComponent<Pathfinding>();
    }

    void Start(){
        Enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }

    public void DoActions(){
        // Enemies[EnemyIndex].GetComponent<EnemyAction>().CanAction = true;
        StartCoroutine("DoActionsEnum");
    }

    bool isEnemyDoneFromEnemy(){
        return Enemies[EnemyIndex].GetComponent<EnemyAction>().isEnemyDone;
    }
    public IEnumerator DoActionsEnum(){
        areAllEnemiesDone = false;
        pf.EnemyActionTile.Clear();

        // pf.EnemyPathTiles.Clear();
        //     pf.EnemyFrontier.Clear();

        DoEnemyCount();
        while(EnemyIndex < Enemies.Length){
        yield return new WaitForSeconds(0.75f); // Take build try till error - then make it 1.5 or 2 ----- Move Fixed Update try

        if(turnManager.isDuringTurn)
        continue;
        
        Enemies[EnemyIndex].GetComponent<EnemyAction>().CanAction = true;


        yield return new WaitUntil(isEnemyDoneFromEnemy);
        EnemyIndex++;
        }
        if(EnemyIndex >= Enemies.Length){
        yield return new WaitForSeconds(1f);
        // EnemyIndex = 0;
        areAllEnemiesDone = true;
        turnManager.EnemyEnergy = 0;
        }
    }

    public void MakeIndexZero(){
        EnemyIndex = 0;
    }

    // RANDOM LINES ?
    public void DoEnemyCount(){
        Enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for(int i = 0; i < Enemies.Length; i++){
            Enemies[i].GetComponent<EnemyAction>().isEnemyDone = false;
        }
    }
}
