using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UnityEngine.UI;
using GS = GameState.State;
using UniRx;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private GameObject lvlUpAnim;
    public Transform leftHand;
    public Transform rightHand;

    private readonly ReactiveProperty<int> hp = new ReactiveProperty<int>(100);
    public IReadOnlyReactiveProperty<int> HP => hp;

    private readonly ReactiveProperty<int> maxHP = new ReactiveProperty<int>(100);
    public IReadOnlyReactiveProperty<int> MaxHP => maxHP;

    private readonly ReactiveProperty<float> _currentXp = new ReactiveProperty<float>(0);
    public IReadOnlyReactiveProperty<float> CurrentXP => _currentXp;

    private readonly ReactiveProperty<float> _maxXp = new ReactiveProperty<float>(1000);
    public IReadOnlyReactiveProperty<float> MaxXP => _maxXp;

    private readonly ReactiveProperty<int> _ashNum = new ReactiveProperty<int>(0);
    public IReadOnlyReactiveProperty<int> ashNum => _ashNum;

    private CompositeDisposable _disposable = new CompositeDisposable();
    private GameState _state;
    private WeaponList _weaponList;
    [Inject] DiContainer container;
    [Inject]
    private void Construct(GameState state,WeaponList list)
    {
        _state = state;
        _weaponList = list;
    }
    private void Update()
    {
        if(_state.state == GS.playing)
        {
            float verticalMove = Input.GetAxis("Vertical");
            float horizontalMove = Input.GetAxis("Horizontal");
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f;
            Vector3 lookDirection = mousePosition - transform.position;
            float lookAngle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0,0,lookAngle - 90);

            Vector2 moveDirection = new Vector2(horizontalMove, verticalMove).normalized;
            if(moveDirection.magnitude > 0.1)
            {
                transform.Translate(moveDirection * speed * Time.deltaTime,Space.World);
            }
        }
        if (hp.Value <= 0)
        {
            hp.Value = 0;
            _state.state = GS.gameOver;
        }
    }
    public void TakeDamage(float amount)
    {
        hp.Value -= (int)amount;
        _state.VictoryStats.DamageRecieve += (int)amount;
    }
    public void GainXP(float amount)
    {
        _currentXp.Value += amount;
        if(_currentXp.Value >= _maxXp.Value)
        {
            StartCoroutine( LevelUp());
            _currentXp.Value -= _maxXp.Value;
            _maxXp.Value *= 1.5f;
        }
    }
    private IEnumerator LevelUp()
    {
        _state.VictoryStats.LevelClaimed++;
        maxHP.Value += 25;
        hp.Value = hp.Value + 50 > maxHP.Value ? maxHP.Value : hp.Value + 50;
        var item = Instantiate(lvlUpAnim);
        item.transform.position = transform.position;
        item.transform.SetParent(transform);
        item.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(1.5f);
        Destroy(item);
    }
    public void AddRemoveCurency(bool fromEnemy,bool state,int amount)
    {
        if (state)
        {
            if (fromEnemy)
            {
                _ashNum.Value += (int)(amount * _state.TakeCurrentScale);
                _state.VictoryStats.AshCollected += (int)(amount * _state.TakeCurrentScale);
            }
            else
                _ashNum.Value += (int)amount;

        }
        if (!state)
        {
            _ashNum.Value -= (int)amount;
        }
    }
    public void SaveData()
    {
        if(leftHand.transform.childCount != 0)
        {
            WeaponData left = leftHand.GetComponentInChildren<WeaponBase>().weaponData;
            _state.SaveData.LeftHand = left;
            _state.SaveData.LeftHand.currentLevel = left.currentLevel;
        }
        else
            _state.SaveData.LeftHand = null;
        if(rightHand.transform.childCount != 0)
        {
            WeaponData right = rightHand.GetComponentInChildren<WeaponBase>().weaponData;
            _state.SaveData.RightHand = right;
            _state.SaveData.RightHand.currentLevel = right.currentLevel;
        }
        else
            _state.SaveData.RightHand = null;
        _state.SaveData.currMaxHP = new int[2] { hp.Value,maxHP.Value};
        _state.SaveData.currMaxXP = new float[2] { _currentXp.Value, _maxXp.Value };
        _state.SaveData.ashAmount = _ashNum.Value;
    }
    public void InitializeWeapon(WeaponData lefthand,WeaponData righthand)
    {
        if(lefthand != null && Resources.Load<GameObject>(lefthand.prefPath) != null)
        {
            var leftweapon = container.InstantiatePrefab(Resources.Load<GameObject>(lefthand.prefPath));
            leftweapon.transform.SetParent(leftHand.transform, false);
            leftweapon.GetComponent<WeaponBase>().weaponData.currentLevel = lefthand.currentLevel;
        }
        if(righthand != null && Resources.Load<GameObject>(righthand.prefPath) != null)
        {
            var rightweapon = container.InstantiatePrefab(Resources.Load<GameObject>(righthand.prefPath));
            rightweapon.transform.SetParent(rightHand.transform, false);
            rightweapon.GetComponent<WeaponBase>().weaponData.currentLevel = righthand.currentLevel;
        }
    }
    public void InitializePlayerData(int[] currMaxHP, float[] currMaxXP,int ashamount)
    {
        hp.Value = currMaxHP[0];
        maxHP.Value = currMaxHP[1];
        _currentXp.Value = currMaxXP[0];
        _maxXp.Value = currMaxXP[1];
        _ashNum.Value = ashamount;
    }
    public void InitializeChoozenWeapon(string[] weaponNames)
    {
        var weaponLeft = _weaponList.Weapons.Find(x => x.name == weaponNames[0]);
        var weaponRight = _weaponList.Weapons.Find(x => x.name == weaponNames[1]);
        InitializeWeapon(weaponLeft, weaponRight);
    }
}
