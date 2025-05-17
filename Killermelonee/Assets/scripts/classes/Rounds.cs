using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoundsData
{
    public Rarity[] roundData;
}
[System.Serializable]
public class Rarity
{
    public int roundTimer;
    public int[] roundRarity;
    public int[] refreshCost;
    public float enemyScale;
}
