using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class EnemyInstaller : MonoInstaller
{
    public TextAsset enemyJsonFile;
    [SerializeField] private EnemyBullet _enemyBullet;
    [SerializeField] private EnemyBullet _bossDagger;
    [SerializeField] private Currency _currency;
    [Serializable]
    public class EnemyBullets
    {
        public string name;
        public EnemyBullet EnemyBullet;
    }
    [SerializeField] private EnemyBullets[] _enemyBullets;
    private GameObject _choosenBullet;

    public override void InstallBindings()
    {
        var enemyList = JsonUtility.FromJson<EnemyList>(enemyJsonFile.text);
        Container.Bind<EnemyList>().FromInstance(enemyList).AsSingle();


        Container.BindFactory<EnemyBase, EnemyFactory>().
            FromMethod((container) =>
            {
                int i = UnityEngine.Random.Range(0,enemyList.enemies.Count);

                var enemy = container.InstantiatePrefab(Resources.Load<GameObject>(enemyList.enemies[i].prefPath));
                var enemystats = enemy.GetComponent<EnemyBase>();

                string json = JsonUtility.ToJson(enemyList.enemies[i]);
                enemystats.SetStats(JsonUtility.FromJson<Enemy>(json));
                container.Inject(enemystats);
                return enemystats;
            });
        Container.BindFactory<Vector2,float,string, EnemyBullet, EnemyBulletFactory>().
            FromMethod((container,direction , damage, enemy) =>
            {
                _choosenBullet = null;
                foreach(var item in _enemyBullets)
                {
                    if(item.name == enemy)
                    {
                        _choosenBullet = container.InstantiatePrefab(item.EnemyBullet);
                    }
                }
                var bullet = _choosenBullet.GetComponent<EnemyBullet>();
                bullet.direction = direction;
                bullet.damage = damage;
                container.Inject(bullet);
                return bullet;
            });
        Container.BindFactory<int, Currency, CurrencyFactory>().
            FromMethod((container, amount) =>
            {
                var ash = container.InstantiatePrefab(_currency);
                var ashAmount = ash.GetComponent<Currency>();
                ashAmount.CurrencyAmount = amount;
                container.Inject(ashAmount);
                return ashAmount;
            });
    }
}
