using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{

    public EnemyAIManager enemyAIManager;

    public int Turn; // ALLY 1 // ENEMY -1 //
    public bool isDuringTurn;

    public bool isDuringChannel; // When you select an ally or enemy to channel them a skill, you won't make them 'selected'

    public int MaxAllyEnergy;
    public int AllyEnergy;

    public int MaxEnemyEnergy;
    public int EnemyEnergy;

    public int TurnCounter;
    

    void Awake(){
        enemyAIManager = this.GetComponent<EnemyAIManager>();
    }

    void Start()
    {
        AllyEnergy = MaxAllyEnergy;
        EnemyEnergy = MaxEnemyEnergy;

        Turn = 1;

        TurnCounter = 0;
    }


    void Update()
    {
        // Debug.Log("TURNS : " + TurnCounter);
        if(isDuringTurn) return;
        CheckGameStatus();
    }

    public void CheckGameStatus(){
        if(AllyEnergy <= 0){
            Turn = -1;
            enemyAIManager.DoActions();
            AllyEnergy = MaxAllyEnergy;

            TurnCounter++;
        }

        // else if(EnemyEnergy <= 0){
        //     Turn = 1;
        //     EnemyEnergy = MaxEnemyEnergy;
        //     enemyAIManager.MakeIndexZero();
        // }
        else if(enemyAIManager.areAllEnemiesDone){
            Turn = 1;
            enemyAIManager.MakeIndexZero();
        }
    }
}
