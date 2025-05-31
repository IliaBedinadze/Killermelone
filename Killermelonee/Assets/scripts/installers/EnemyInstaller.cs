using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using static UnityEditor.Progress;

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
        public GameObject EnemyBullet;
    }
    [SerializeField] private EnemyBullets[] _enemyBullets;
    [SerializeField] private GameObject enemy;
    public override void InstallBindings()
    {
        var enemyList = JsonUtility.FromJson<EnemyList>(enemyJsonFile.text);
        Container.Bind<EnemyList>().FromInstance(enemyList).AsSingle();

        // factory that spawns enemy by name
        Container.BindFactory<string,string,EnemyBase, EnemyFactory>().
            FromMethod((container, enemyName, enemyType) =>
            {
                if(enemyType == "ordinary")
                    enemy = container.InstantiatePrefab(Resources.Load<GameObject>(enemyList.enemies.Find(x => x.name == enemyName).prefPath));
                else if(enemyType =="Boss")
                    enemy = container.InstantiatePrefab(Resources.Load<GameObject>(enemyList.Bosses.Find(x => x.name == enemyName).prefPath));
                var enemystats = enemy.GetComponent<EnemyBase>();

                // compress to json and back to let every enemy have their own stats(otherway they ref to one source)
                string json = JsonUtility.ToJson(enemyList.enemies.Find(x => x.name == enemyName));
                enemystats.SetStats(JsonUtility.FromJson<Enemy>(json));
                container.Inject(enemystats);
                return enemystats;
            });
        // creating enemy bullets by direction to player(vector2), enemy name to know wich bullet take from _enemyBullets 
        // and enemy also set damage for this bullet
        Container.BindFactory<Vector2, float, string, EnemyBullet, EnemyBulletFactory>().
            FromMethod((container, direction, damage, enemy) =>
            {
                int index = 0;
                for(int i = 0; i < _enemyBullets.Length;i++)
                {
                    if (_enemyBullets[i].name == enemy)
                    {
                        index = i; break;
                    }
                }
                var choosenBullet = container.InstantiatePrefab(_enemyBullets[index].EnemyBullet);
                var bullet = choosenBullet.GetComponent<EnemyBullet>();
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
