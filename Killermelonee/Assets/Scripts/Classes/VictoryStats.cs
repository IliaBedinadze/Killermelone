using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// class for game over panel stats show
public class VictoryStats 
{
    public int EnemyKilled = 0;
    public int DamageDone = 0;
    public int DamageRecieve = 0;
    public int LevelClaimed = 0;
    public int AshCollected = 0;
    public bool VictoryState = false;
    public VictoryStats(int enemyKilled,int damageDone,int damageRecieved,int levelClaimed,int ashCollcted)
    {
        EnemyKilled = enemyKilled;
        DamageDone = damageDone;
        DamageRecieve = damageRecieved;
        LevelClaimed = levelClaimed;
        AshCollected = ashCollcted;
    }
}
