using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public int MaxHealth;
    public int CurrentHealth;

    [Header("DAMAGE & DEF & MISS CRIT")]

    public int Damage;
    public int Defence;
    public int Evasion;
    public int CritChance;
    public int CritMultiplier;

    public void Start(){
        CurrentHealth = MaxHealth;
    }
}
