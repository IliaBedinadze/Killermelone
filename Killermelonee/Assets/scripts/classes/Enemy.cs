using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Enemy
{
    public string name;
    public float health;
    public float speed;
    public float contactDamage;
    public float projectileDamage;
    public float XPGain;
    public float shootRate;
    public int[] ashAmount;
    public string prefPath;


    // for enemy stat scale
    public void ScaleEnemy(float scale)
    {
        health *= scale;
        speed *= scale;
        contactDamage *= scale;
        projectileDamage *= scale;
        XPGain *= scale;
    }
}

// serializing list of enemy from json
[System.Serializable]
public class EnemyList
{
    public List<Enemy> enemies;
    public List<Enemy> Bosses;
}

