using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using GS = GameState.State;

public class Shop : MonoBehaviour
{
    [SerializeField] private Button readyButton;
    [SerializeField] private Button refreshButton;
    [SerializeField] private GameObject shopItems;
    [SerializeField] private WeaponIcon handLeft;
    [SerializeField] private WeaponIcon handRight;
    [SerializeField] private Sprite empty;
    [SerializeField] private Button sellButtonLeft;
    [SerializeField] private Button sellButtonRight;
    [SerializeField] private Text ashAmount;
    [SerializeField] private Text refresherText;

    private string _refresherFormat = "refresh({0})";
    private string _ashAmountFormat = ":{0}";
    private int _refresherCount = 0;

    private WeaponBase _weaponLeft;
    private WeaponBase _weaponRight;

    private GameObject _CurrentShop;
    private Button[] _shopItems;

    private RoundStats _roundStats;
    private GameState _state;
    private UI _ui;
    private Player _player;
    private ExceptionPanel _exceptionPanel;
    private List<WeaponData> _weaponList;

    [Inject]
    public DiContainer container;
    [Inject]
    public void Constructor(GameState state, UI ui, Player player, ExceptionPanel exceptionPanel, RoundsData roundData, WeaponList weaponList)
    {
        _weaponList = weaponList.Weapons;
        _state = state;
        _ui = ui;
        _player = player;
        _exceptionPanel = exceptionPanel;
        _roundStats = roundData.roundData[_state.TakeCurrentRound - 1];
    }
    private void Start()
    {
        readyButton.onClick.AddListener(delegate { Ready(); });
        refreshButton.onClick.AddListener(delegate { Refresher(false); });
        sellButtonLeft.onClick.AddListener(delegate { Sell("left"); });
        sellButtonRight.onClick.AddListener(delegate { Sell("right"); });
        gameObject.transform.SetParent(_ui.gameObject.transform, false);
        ashAmount.text = string.Format(_ashAmountFormat, _player._ashAmount);

        Refresher(true);

        InitializePlayerWeapon(true, "right");
        InitializePlayerWeapon(true, "left");
    }
    private void Ready()
    {
        _state.state = GS.playing;
        _state.NextRound(false);
        _state.SaveGame();
        Destroy(gameObject);
    }
    private void Refresher(bool start)
    {
        if (start)
        {
            InitializeRefresher();
        }
        else
        {
            if (_roundStats.refreshCost[_refresherCount] <= _player._ashAmount)
            {
                _player._ashAmount -= _roundStats.refreshCost[_refresherCount];
                ashAmount.text = string.Format(_ashAmountFormat, _player._ashAmount);
                Destroy(_CurrentShop);
                if (_refresherCount < 4)
                {
                    _refresherCount++;
                }
                InitializeRefresher();
            }
        }
    }
    private void InitializeRefresher()
    {
        _CurrentShop = container.InstantiatePrefab(shopItems);
        _CurrentShop.transform.SetParent(gameObject.transform, false);
        _shopItems = _CurrentShop.GetComponentsInChildren<Button>();
        refresherText.text = string.Format(_refresherFormat, _roundStats.refreshCost[_refresherCount]);

        foreach (Button button in _shopItems)
        {
            string json = JsonUtility.ToJson(RandomWeapon());
            button.GetComponent<WeaponIcon>().weaponData = JsonUtility.FromJson<WeaponData>(json);
            button.onClick.AddListener(delegate { BuyWeapon(button.GetComponent<WeaponIcon>()); });
        }
    }
    private void BuyWeapon(WeaponIcon weapon)
    {
        if (weapon.weaponData.TakeCurrentPrice <= _player._ashAmount)
        {
            if (_weaponLeft != null && WeaponAreSame(_weaponLeft.weaponData, weapon.weaponData))
            {
                _weaponLeft.weaponData.currentLevel++;
                _player._ashAmount -= weapon.weaponData.TakeCurrentPrice;
                ashAmount.text = string.Format(_ashAmountFormat, _player._ashAmount);
                handLeft.weaponData = _weaponLeft.weaponData;
                handLeft.InitializeSellItem(true, null);
                Destroy(weapon.gameObject);
            }
            else if (_weaponRight != null && WeaponAreSame(_weaponRight.weaponData, weapon.weaponData))
            {
                _weaponRight.weaponData.currentLevel++;
                _player._ashAmount -= weapon.weaponData.TakeCurrentPrice;
                ashAmount.text = string.Format(_ashAmountFormat, _player._ashAmount);
                handRight.weaponData = _weaponRight.weaponData;
                handRight.InitializeSellItem(true, null);
                Destroy(weapon.gameObject);
            }
            else if (_weaponLeft == null)
            {
                var weap = Resources.Load<GameObject>(weapon.weaponData.prefPath);
                var item = container.InstantiatePrefab(weap);
                string json = JsonUtility.ToJson(weapon.weaponData);
                item.GetComponent<WeaponBase>().weaponData = JsonUtility.FromJson<WeaponData>(json);
                item.transform.SetParent(_player.leftHand, false);
                InitializePlayerWeapon(true, "left");
                _player._ashAmount -= weapon.weaponData.TakeCurrentPrice;
                ashAmount.text = string.Format(_ashAmountFormat, _player._ashAmount);
                Destroy(weapon.gameObject);
            }
            else if (_weaponRight == null)
            {
                var weap = Resources.Load<GameObject>(weapon.weaponData.prefPath);
                var item = container.InstantiatePrefab(weap);
                string json = JsonUtility.ToJson(weapon.weaponData);
                item.GetComponent<WeaponBase>().weaponData = JsonUtility.FromJson<WeaponData>(json);
                item.transform.SetParent(_player.rightHand, false);
                InitializePlayerWeapon(true, "right");
                _player._ashAmount -= weapon.weaponData.TakeCurrentPrice;
                ashAmount.text = string.Format(_ashAmountFormat, _player._ashAmount);
                Destroy(weapon.gameObject);
            }
            else
            {
                var panel = Instantiate(_exceptionPanel);
                panel.SetMassage("you have both hand full!");
                panel.transform.SetParent(_ui.transform, false);
            }
        }
        else
        {
            var panel = Instantiate(_exceptionPanel);
            panel.SetMassage("not enought ash!");
            panel.transform.SetParent(_ui.transform, false);
        }
    }
    private void Sell(string hand)
    {
        if (hand == "right" && _weaponRight != null)
        {
            _player._ashAmount += _weaponRight.weaponData.TakeCurrentPrice / 2;
            ashAmount.text = string.Format(_ashAmountFormat, _player._ashAmount);
            Destroy(_weaponRight.gameObject);
            _weaponRight = null;
            handRight.InitializeSellItem(false, empty);
        }
        else if (hand == "left" && _weaponLeft != null)
        {
            _player._ashAmount += _weaponLeft.weaponData.TakeCurrentPrice / 2;
            ashAmount.text = string.Format(_ashAmountFormat, _player._ashAmount);
            Destroy(_weaponLeft.gameObject);
            _weaponLeft = null;
            handLeft.InitializeSellItem(false, empty);
        }
    }
    private void InitializePlayerWeapon(bool state, string hand)
    {
        if (hand == "left")
        {
            if (_player.leftHand.childCount != 0 && state)
            {
                _weaponLeft = _player.leftHand.GetComponentInChildren<WeaponHitter>();
                handLeft.weaponData = _weaponLeft.weaponData;
                handLeft.InitializeSellItem(true, null);
            }
            else
                handLeft.InitializeSellItem(false, empty);
        }
        else if (hand == "right")
        {
            if (_player.rightHand.childCount != 0 && state)
            {
                _weaponRight = _player.rightHand.GetComponentInChildren<WeaponHitter>();
                handRight.weaponData = _weaponRight.weaponData;
                handRight.InitializeSellItem(true, null);
            }
            else
                handRight.InitializeSellItem(false, empty);
        }
    }
    private WeaponData RandomWeapon()
    {
        var rarity = GetRandomRarity(_roundStats.roundRarity);
        var i = Random.Range(0, _weaponList.Count);

        WeaponData weapon = _weaponList[i];
        weapon.currentLevel = rarity;
        return weapon;
    }
    private int GetRandomRarity(int[] chances)
    {
        int chance = UnityEngine.Random.Range(1, 101);
        if (chance <= chances[0])
            return 0;
        else if (chances[0] < chance && chance <= chances[1])
            return 1;
        else if (chances[1] < chance && chance <= chances[2])
            return 2;
        else if (chances[2] < chance && chance <= chances[3])
            return 3;
        else
            return 4;
    }
    private bool WeaponAreSame(WeaponData one, WeaponData two)
    {
        if (one.name == two.name && one.currentLevel == two.currentLevel && one.currentLevel != 4)
        {
            return true;
        }
        return false;
    }
}
