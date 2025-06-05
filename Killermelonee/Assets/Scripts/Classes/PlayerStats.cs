using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

[System.Serializable]
public class Heroes
{
    public PlayerStats[] HeroTypes;
}
// Stats for any possible hero
[System.Serializable]
public class PlayerStats
{
    public string Name;
    public int HP;
    public int MaxHP;
    public float XP;
    public float MaxXP;
    public int Pierce;
    public int AshAmount;
    //Multipliers
    public float Speed;
    public float Damage;
    public float AttackSpeed;
    public float Velocity;
    public float Farm;
}
public class PlayerReactiveStats
{
    public readonly ReactiveProperty<int> HP = new();
    public readonly ReactiveProperty<int> MaxHP = new();
    public readonly ReactiveProperty<float> XP = new();
    public readonly ReactiveProperty<float> MaxXP = new();
    public readonly ReactiveProperty<float> Speed = new();
    public readonly ReactiveProperty<float> Damage = new();
    public readonly ReactiveProperty<float> AttackSpeed = new();
    public readonly ReactiveProperty<int> Pierce = new();
    public readonly ReactiveProperty<float> Velocity = new();
    public readonly ReactiveProperty<float> Farm = new();
    public readonly ReactiveProperty<int> AshAmount = new();

    // From PlayerStats => to PlayerReactiveStats
    public void LoadFrom(PlayerStats stats)
    {
        HP.Value = stats.HP;
        MaxHP.Value = stats.MaxHP;
        XP.Value = stats.XP;
        MaxXP.Value = stats.MaxXP;
        Speed.Value = stats.Speed;
        Damage.Value = stats.Damage;
        AttackSpeed.Value = stats.AttackSpeed;
        Velocity.Value = stats.Velocity;
        Farm.Value = stats.Farm;
        AshAmount.Value = stats.AshAmount;

    }
    // To save data as json
    public PlayerStats ToStats()
    {
        return new PlayerStats
        {
            HP = HP.Value,
            MaxHP = MaxHP.Value,
            XP = XP.Value,
            MaxXP = MaxXP.Value,
            Speed = Speed.Value,
            Damage = Damage.Value,
            AttackSpeed = AttackSpeed.Value,
            Velocity = Velocity.Value,
            Farm = Farm.Value,
            AshAmount = AshAmount.Value
        };
    }
}
