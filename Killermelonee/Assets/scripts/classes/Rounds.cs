using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoundsData
{
    public int currentRound;
    public List<int> Rounds;
    public Rarity[] rarityChanceByRound;
}
[System.Serializable]
public class Rarity
{
    public int[] roundRarity;
    public int[] refreshCost;
}
