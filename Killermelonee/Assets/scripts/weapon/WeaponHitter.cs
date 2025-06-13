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
    private Player _player;
    [Inject]
    public void Constructor(WeaponList list, GameState gamestate,Player player)
    {
        _gameState = gamestate;
        string json = JsonUtility.ToJson(list.Weapons.Find(x => x.name == ID));
        weaponData = JsonUtility.FromJson<WeaponData>(json);
        _player = player;
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
            var go = factory.Create(weaponData.name,
                weaponData.TakeCurrentPierceAmount == -5 ? weaponData.TakeCurrentPierceAmount : weaponData.TakeCurrentPierceAmount + _player.CurrentPierceAddition,
                weaponData.TakeCurrentDamage * _player.CurrentDamageMultiplier,
                weaponData.TakeCurrentLifeTime,
                weaponData.TakeCurrentBulletSpeed * _player.CurrentVelocityMultiplier);
            go.transform.position = ShootPoint.position;

        }
        if (_hasShooted && _gameState.state == GS.playing)
        {
            _time += Time.deltaTime;
            if(_player.CurrentAttackSpeedMultiplier >= 1)
            {
                if (weaponData.TakeCurrentAR / _player.CurrentAttackSpeedMultiplier <= _time)
                {
                    _hasShooted = false;
                    _time = 0;
                }
            }
            else if(_player.CurrentAttackSpeedMultiplier < 1 && _player.CurrentAttackSpeedMultiplier >=0) 
            {
                if(weaponData.TakeCurrentAR * (1 + (1 - _player.CurrentAttackSpeedMultiplier)) <= _time)
                {
                    _hasShooted = false;
                    _time = 0;
                }
            }
            else
            {
                if(weaponData.TakeCurrentAR *(1 + Mathf.Abs(_player.CurrentAttackSpeedMultiplier)) <= _time)
                {
                    _hasShooted = false;
                    _time = 0;
                }
            }
        }
    }
}
