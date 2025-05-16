using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using Zenject.SpaceFighter;

public class WeaponInstaller : MonoInstaller
{
    [Serializable]
    public class WeaponSupply
    {
        public string Name;
        public Bullet Bullet;
    }

    [SerializeField] private WeaponSupply[] _weaponSupply;
    private Bullet _bullet;
    public override void InstallBindings()
    {
        Container.BindFactory<string,int, float, float, float, Bullet, bulletFactory>().
        FromMethod((container,name , pierce, damage, lifetime, speed) =>
        {
            foreach (var item in _weaponSupply)
            {
                if (item.Name == name)
                {
                    _bullet = item.Bullet;
                    break;
                }
            }
            var pref = container.InstantiatePrefab(_bullet);
            var bullet = pref.GetComponent<Bullet>();

            bullet.SetStats(pierce, damage, lifetime, speed);
            container.Inject(bullet);
            return bullet;
        });
    }
}
