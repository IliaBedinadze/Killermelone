using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Save 
{
    public WeaponData LeftHand;
    public WeaponData RightHand;
    public int currentRound = 0;
    public int ashAmount = 0;
    public PlayerStats PlayerStats = new();
    public VictoryStats VictoryStats;
}
