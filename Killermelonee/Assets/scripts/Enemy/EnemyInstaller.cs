using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemyInstaller : MonoInstaller
{
    public TextAsset enemyJsonFile;
    [SerializeField] private EnemyBullet _enemyBullet;
    [SerializeField] private Currency _currency;

    public override void InstallBindings()
    {
        var enemyList = JsonUtility.FromJson<EnemyList>(enemyJsonFile.text);
        Container.Bind<EnemyList>().FromInstance(enemyList).AsSingle();


        Container.BindFactory<EnemyBase, EnemyFactory>().
            FromMethod((container) =>
            {
                int i = Random.Range(0,enemyList.enemies.Count);

                var enemy = container.InstantiatePrefab(Resources.Load<GameObject>(enemyList.enemies[i].prefPath));
                var enemystats = enemy.GetComponent<EnemyBase>();

                string json = JsonUtility.ToJson(enemyList.enemies[i]);
                enemystats.SetStats(JsonUtility.FromJson<Enemy>(json));
                container.Inject(enemystats);
                return enemystats;
            });
        Container.BindFactory<float, EnemyBullet, EnemyBulletFactory>().
            FromMethod((container, damage) =>
            {
                var bulletGo = container.InstantiatePrefab(_enemyBullet);
                var bullet = bulletGo.GetComponent<EnemyBullet>();
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
