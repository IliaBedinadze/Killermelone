using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponData
{
    public string name;
    public float[] damage;
    public float[] attackRate;
    public float[] lifeTime;
    public float[] bulletSpeed;
    public string spritePath;
    public string prefPath;
    public int currentLevel;
    public int maxLevel;
    public string description;
    public int[] price;
    public int[] pierceAmount;
    public int TakeCurrentPrice => price[currentLevel];
    public float TakeCurrentDamage => damage[currentLevel];
    public float TakeNextDamage => damage[currentLevel + 1];
    public float TakeCurrentAR => attackRate[currentLevel];
    public float TakeNextAR => attackRate[currentLevel + 1];
    public float TakeCurrentLifeTime => lifeTime[currentLevel];
    public float TakeCurrentBulletSpeed => bulletSpeed[currentLevel];
    public int TakeCurrentPierceAmount => pierceAmount[currentLevel];
    public int TakeNextPierceAmount => pierceAmount[currentLevel + 1];
}
[System.Serializable]
public class WeaponList
{
    public List<WeaponData> Weapons;
}
