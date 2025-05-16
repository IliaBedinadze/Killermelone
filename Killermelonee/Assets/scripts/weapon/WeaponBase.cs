using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject.Asteroids;
public class WeaponBase : MonoBehaviour
{
    [NonSerialized] public WeaponData weaponData;

    public void SetStats(WeaponData data)
    {
        weaponData = data;
    }
}
