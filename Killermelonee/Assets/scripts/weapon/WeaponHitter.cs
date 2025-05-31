using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Zenject.Asteroids;
using GS = GameState.State;

public class WeaponHitter : WeaponBase
{
    [Inject]
    private bulletFactory factory;

    private bool _hasShooted;
    private float _time;

    [SerializeField] private string ID;
    [SerializeField] private Transform ShootPoint;
    private AudioSource _audioSource;
    private GameState _gameState;
    [Inject]
    public void Constructor(WeaponList list, GameState gamestate)
    {
        _gameState = gamestate;
        string json = JsonUtility.ToJson(list.Weapons.Find(x => x.name == ID));
        weaponData = JsonUtility.FromJson<WeaponData>(json);
    }
    private void Start()
    {
        _hasShooted = true;
        _audioSource = ShootPoint.GetComponent<AudioSource>();
    }
    private void Update()
    {
        if (!_hasShooted && _gameState.state == GS.playing)
        {
            _hasShooted = true;
            _audioSource.Play();
            var go = factory.Create(weaponData.name,weaponData.TakeCurrentPierceAmount,weaponData.TakeCurrentDamage, weaponData.TakeCurrentLifeTime, weaponData.TakeCurrentBulletSpeed);
            go.transform.position = ShootPoint.position;
        }
        if (_hasShooted && _gameState.state == GS.playing)
        {
            _time += Time.deltaTime;
            if (weaponData.TakeCurrentAR <= _time)
            {
                _hasShooted = false;
                _time = 0;
            }
        }
    }
}
