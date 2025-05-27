using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// stats for every round,serealize by json
[System.Serializable]
public class RoundsData
{
    public RoundStats[] roundData;
}
[System.Serializable]
public class RoundStats
{
    // time amount for round complete
    public int roundTimer;
    // weapon rarity chance on shop
    public int[] roundRarity;
    // shop refresh goods cost
    public int[] refreshCost;
    // scale for enemy
    public float enemyScale;
    // which enemy can be spawn on current round
    public EnemyByRound enemyByRound;
}
[System.Serializable]
public class EnemyByRound
{
    //which enemy take part in current round
    public string[] baseEnemy;
    // to let know if boss should be swapned
    public bool boss;
    // boss to spawn
    public string bossEnemy;
    // what type is wave, hoard or ordinary
    public string roundType;
    // frequency of base enemy spawn
    public float[] roundSpawnRate;
    // how many spawners should work
    public int spawnersAmount;
}
