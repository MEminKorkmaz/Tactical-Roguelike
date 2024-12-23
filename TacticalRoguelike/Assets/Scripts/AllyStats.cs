using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyStats : MonoBehaviour
{
    public int MaxLevel; // 30
    // public int StatsAfterLevelUp; // 10
    private int CurrentLevel;
    public int MaxHealth;
    public int CurrentHealth;

    [Header("DAMAGE & DEF & MIS CRIT")]
    public int Damage;
    public int Defence;
    public int Evasion;
    public int CritChance;
    public int CritMultiplier;

    // [Header("BASE VALUES")]
    // public int Damage; // Base Damage + Strength / 1 -- FROM %20 Less TO %20 More
    // public int Healing; // Base Healing + Defence / 5 -- FROM %10 Less TO %20 More AS PERCENTAGE
    // public int DamageReduction; // Base Damage Reduction + Endurance / 5 AS PERCENTAGE
    // public int Evasion; // Base Evasion + Agility / 5 AS PERCENTAGE
    // public int CritChance; // Base Crit Chance + Agility / 5 AS PERCENTAGE
    // public int PhysicalPierce; // // Base Physical Pierce + Piercing / 5 AS PERCENTAGE
    // private int CritMultiplier; // %250

    // [Header("STATS")] 
    // public int MaxHealth; // + 5 Health
    // public int Strength; // + 1 Damage
    // public int Defence; // + 0.3 Healing
    // public int Endurance; // + 0.2 Damage Reduction
    // public int Agility; // + 0.2 Evasion -- + 0.2 Crit Chance
    // public int Piercing; // + 0.2 Physical Pierce
}
