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
    public int[] currMaxHP = new int[2];
    public float[] currMaxXP = new float[2];
}
