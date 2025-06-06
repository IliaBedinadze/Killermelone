using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoundsData
{
    public RoundStats[] roundData;
}
// stats for every round
[System.Serializable]
public class RoundStats
{
    public int roundTimer;      // time amount for round complete
    public int[] roundRarity;   // weapon rarity chance on shop
    public int[] itemRoundRarity; //item rarity chance
    public int[] refreshCost;   // shop refresh goods cost
    public float enemyScale;    // scale for enemy
    public EnemyByRound enemyByRound;
}
// enemy spawners info for rounds
[System.Serializable]
public class EnemyByRound
{
    public string[] baseEnemy;    // which enemy take part in current round
    public bool boss;             // boss round state
    public string bossEnemy;      // boss to spawn
    public string roundType;      // hoard or ordinary
    public float[] roundSpawnRate;// enemy spawn frequency
    public int spawnersAmount;    // spawners amount at same time
}
