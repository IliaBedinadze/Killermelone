using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UnityEngine.UI;
using GS = GameState.State;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Slider hpBar;
    [SerializeField] private Text hPText;
    [SerializeField] private Slider xpBar;
    [SerializeField] private Text currencyText;
    [SerializeField] private GameObject lvlUpAnim;
    public Transform leftHand;
    public Transform rightHand;

    [NonSerialized] public int HP = 100;
    private int maxHP = 100;
    private float _currentXp;
    private float _maxXp = 1000;
    [NonSerialized] public int _ashAmount;

    private GameState _state;
    private WeaponList _weaponList;
    [Inject] DiContainer container;
    [Inject]
    private void Construct(GameState state,WeaponList list)
    {
        _state = state;
        _weaponList = list;
    }

    private void Start()
    {
        hpBar.maxValue = maxHP;
        hpBar.value = HP;
        hPText.text = HP.ToString();
        currencyText.text = _ashAmount.ToString();
        xpBar.maxValue = _maxXp;
        xpBar.value = _currentXp;
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
        if (HP <= 0)
        {
            HP = 0;
            hpBar.value = HP;
            hPText.text = HP.ToString();
            _state.state = GS.gameOver;
        }
    }
    public void TakeDamage(float amount)
    {
        HP -= (int)amount;
        hpBar.value = HP;
        hPText.text = HP.ToString();
    }
    public void GainXP(float amount)
    {
        _currentXp += amount;
        xpBar.value = _currentXp;
        if(_currentXp >= _maxXp)
        {
            StartCoroutine( LevelUp());
            _currentXp -= _maxXp;
            xpBar.value = _currentXp;
            _maxXp *= 1.5f;
            xpBar.maxValue = _maxXp;
        }
    }
    private IEnumerator LevelUp()
    {
        maxHP += 25;
        HP = HP + 50 > maxHP ? maxHP : HP + 50;
        hpBar.maxValue += maxHP;
        hpBar.value = HP;
        hPText.text = HP.ToString();
        var item = Instantiate(lvlUpAnim);
        item.transform.position = transform.position;
        item.transform.SetParent(transform);
        item.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(1.5f);
        Destroy(item);
    }
    public void AddRemoveCurency(bool state,int amount)
    {
        if (state)
        {
            _ashAmount += (int)(amount * _state.TakeCurrentScale);
            currencyText.text = _ashAmount.ToString();
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
        _state.SaveData.currMaxHP = new int[2] { HP,maxHP};
        _state.SaveData.currMaxXP = new float[2] { _currentXp, _maxXp };
        _state.SaveData.ashAmount = _ashAmount;
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
        HP = currMaxHP[0];
        maxHP = currMaxHP[1];
        _currentXp = currMaxXP[0];
        _maxXp = currMaxXP[1];
        _ashAmount = ashamount;
    }
    public void InitializeChoozenWeapon(string[] weaponNames)
    {
        var weaponLeft = _weaponList.Weapons.Find(x => x.name == weaponNames[0]);
        var weaponRight = _weaponList.Weapons.Find(x => x.name == weaponNames[1]);
        InitializeWeapon(weaponLeft, weaponRight);
    }
}
