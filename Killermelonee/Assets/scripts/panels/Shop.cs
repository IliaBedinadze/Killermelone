using System.Collections;
using System.Collections.Generic;
using UniRx;
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

    [SerializeField] private Slider hpBar;
    [SerializeField] private Text hpText;
    // text format for shop
    private string _refresherFormat = "refresh({0})";
    private string _ashAmountFormat = ":{0}";

    private int _refresherCount = 0;    // counts refresher click

    // player weapons
    private WeaponBase _weaponLeft;  
    private WeaponBase _weaponRight;   

    private GameObject _CurrentShop;   // current item panel
    private WeaponIcon[] _weaponIcons; //weapon icons on shop
    private ItemIcon[] _itemIcons;     //item icons on shop

    //dependencies
    private RoundStats _roundStats;
    private GameState _state;
    private UI _ui;
    private Player _player;
    private ExceptionPanel _exceptionPanel;
    private List<WeaponData> _weaponList;
    private ItemList _itemList;
    private SceneAudioController _sceneAudioController;
    [Inject]
    public DiContainer container;
    [Inject]
    public void Constructor(GameState state, UI ui, Player player, ExceptionPanel exceptionPanel, RoundsData roundData, WeaponList weaponList,SceneAudioController audioController,ItemList itemList)
    {
        _weaponList = weaponList.Weapons;
        _itemList = itemList;
        _state = state;
        _ui = ui;
        _player = player;
        _exceptionPanel = exceptionPanel;
        _roundStats = roundData.roundData[_state.TakeCurrentRound - 1];
        _sceneAudioController = audioController;
    }
    private void Start()
    {
        _player.MaxHP.Subscribe(x => hpBar.maxValue = x).AddTo(this);
        _player.HP.Subscribe(x => hpBar.value = x).AddTo(this);
        _player.HP.Subscribe(x => hpText.text = x.ToString()).AddTo(this);
        readyButton.onClick.AddListener(delegate { Ready(); });
        refreshButton.onClick.AddListener(delegate { Refresher(false); });
        sellButtonLeft.onClick.AddListener(delegate { Sell("left"); });
        sellButtonRight.onClick.AddListener(delegate { Sell("right"); });
        gameObject.transform.SetParent(_ui.gameObject.transform, false);
        _player.ashNum.Subscribe(x => ashAmount.text = string.Format(_ashAmountFormat,x)).AddTo(this);

        Refresher(true);

        InitializePlayerWeapon(true, "right");
        InitializePlayerWeapon(true, "left");
    }
    // ready button click code
    private void Ready()
    {
        _sceneAudioController.PlayClick();
        _state.state = GS.playing;
        _state.NextRound(false);
        Destroy(gameObject);
    }
    // initialize shop on start and on refresh button click
    private void Refresher(bool start)
    {
        if (start)
        {
            InitializeRefresher();
        }
        else
        {
            if (_roundStats.refreshCost[_refresherCount] <= _player.ashNum.Value)
            {
                _player.AddRemoveCurency(false,false, _roundStats.refreshCost[_refresherCount]);
                Destroy(_CurrentShop);
                if (_refresherCount < 4)
                {
                    _refresherCount++;
                }
                InitializeRefresher();
                _sceneAudioController.PlayClick();
            }
        }
    }
    // shop items initialization
    private void InitializeRefresher()
    {
        _CurrentShop = container.InstantiatePrefab(shopItems);
        _CurrentShop.transform.SetParent(gameObject.transform, false);
        _weaponIcons = _CurrentShop.GetComponentsInChildren<WeaponIcon>();
        _itemIcons = _CurrentShop.GetComponentsInChildren<ItemIcon>();
        refresherText.text = string.Format(_refresherFormat, _roundStats.refreshCost[_refresherCount]);


        foreach(var item in _itemIcons)
        {
            string json = JsonUtility.ToJson(RandomItem());
            item.SellItemInitialization(JsonUtility.FromJson<ItemStats>(json));
            item.GetComponent<Button>().onClick.AddListener(delegate { BuyItem(item); });
        }
        foreach(var item in _weaponIcons)
        {
            string json = JsonUtility.ToJson(RandomWeapon());
            item.SetStats(JsonUtility.FromJson<WeaponData>(json));
            item.GetComponent<Button>().onClick.AddListener(delegate { BuyWeapon(item); });
        }
    }
    // weapon buy code
    private void BuyWeapon(WeaponIcon weapon)
    {
        if (weapon.weaponData.TakeCurrentPrice <= _player.ashNum.Value)
        {
            if (_weaponLeft == null)
            {
                var weap = Resources.Load<GameObject>(weapon.weaponData.prefPath);
                var item = container.InstantiatePrefab(weap);
                string json = JsonUtility.ToJson(weapon.weaponData);
                item.GetComponent<WeaponBase>().weaponData = JsonUtility.FromJson<WeaponData>(json);
                item.transform.SetParent(_player.leftHand, false);
                InitializePlayerWeapon(true, "left");
                _player.AddRemoveCurency(false,false, weapon.weaponData.TakeCurrentPrice);
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
                _player.AddRemoveCurency(false,false, weapon.weaponData.TakeCurrentPrice);
                Destroy(weapon.gameObject);
            }
            else if (_weaponLeft != null && WeaponAreSame(_weaponLeft.weaponData, weapon.weaponData))
            {
                _weaponLeft.weaponData.currentLevel++;
                _player.AddRemoveCurency(false,false, weapon.weaponData.TakeCurrentPrice);
                handLeft.SetStats(_weaponLeft.weaponData);
                handLeft.InitializeSellItem(true, null);
                Destroy(weapon.gameObject);
            }
            else if (_weaponRight != null && WeaponAreSame(_weaponRight.weaponData, weapon.weaponData))
            {
                _weaponRight.weaponData.currentLevel++;
                _player.AddRemoveCurency(false,false, weapon.weaponData.TakeCurrentPrice);
                handRight.SetStats(_weaponRight.weaponData);
                handRight.InitializeSellItem(true, null);
                Destroy(weapon.gameObject);
            }
            else
            {
                var panel = container.InstantiatePrefab(_exceptionPanel.gameObject);
                panel.GetComponent<ExceptionPanel>().SetMassage("you have both hand full!");
                panel.transform.SetParent(_ui.transform, false);
            }
        }
        else
        {
            var panel = container.InstantiatePrefab(_exceptionPanel.gameObject);
            panel.GetComponent<ExceptionPanel>().SetMassage("not enought ash!");
            panel.transform.SetParent(_ui.transform, false);
        }
    }
    // sell item code
    private void Sell(string hand)
    {
        if (hand == "right" && _weaponRight != null)
        {
            _player.AddRemoveCurency(false,true, _weaponRight.weaponData.TakeCurrentPrice / 2);
            Destroy(_weaponRight.gameObject);
            _weaponRight = null;
            handRight.InitializeSellItem(false, empty);
            _sceneAudioController.PlayClick();
        }
        else if (hand == "left" && _weaponLeft != null)
        {
            _player.AddRemoveCurency(false,true, _weaponLeft.weaponData.TakeCurrentPrice / 2);
            Destroy(_weaponLeft.gameObject);
            _weaponLeft = null;
            handLeft.InitializeSellItem(false, empty);
            _sceneAudioController.PlayClick();
        }
    }
    // initialize player weapon on shop
    private void InitializePlayerWeapon(bool state, string hand)
    {
        if (hand == "left")
        {
            if (_player.leftHand.childCount != 0 && state)
            {
                _weaponLeft = _player.leftHand.GetComponentInChildren<WeaponHitter>();
                handLeft.SetStats(_weaponLeft.weaponData);
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
                handRight.SetStats(_weaponRight.weaponData);
                handRight.InitializeSellItem(true, null);
            }
            else
                handRight.InitializeSellItem(false, empty);
        }
    }
    // return random weapon
    private WeaponData RandomWeapon()
    {
        var rarity = GetRandomRarity(_roundStats.roundRarity);
        var i = Random.Range(0, _weaponList.Count);

        WeaponData weapon = _weaponList[i];
        weapon.currentLevel = rarity;
        return weapon;
    }
    // return random rarity, depends on round
    private int GetRandomRarity(int[] chances)
    {
        int chance = Random.Range(1, 101);
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
    // checks if weapons are same
    private bool WeaponAreSame(WeaponData one, WeaponData two)
    {
        if (one.name == two.name && one.currentLevel == two.currentLevel && one.currentLevel != 4)
        {
            return true;
        }
        return false;
    }
    // returns random item
    private ItemStats RandomItem()
    {
        var rarity = GetRandomRarity(_roundStats.roundRarity);
        if(rarity == 0)
            return _itemList.CommonItems[Random.Range(0, _itemList.CommonItems.Length)];
        else if(rarity == 1)
            return _itemList.RareItems[Random.Range(0, _itemList.RareItems.Length)];
        else if(rarity == 2)
            return _itemList.EpicItems[Random.Range(0,_itemList.EpicItems.Length)];
        else if(rarity == 3)
            return _itemList.LegendaryItems[Random.Range(0,_itemList.LegendaryItems.Length)];
        else
            return _itemList.UniqueItems[Random.Range(0,_itemList.UniqueItems.Length)];
    }
    // buy item code
    private void BuyItem(ItemIcon item)
    {
        if(item.TakePrice <= _player.ashNum.Value)
        {
            _player.AddRemoveCurency(false,false,item.TakePrice);
            item.ItemStatsAddition();
            Destroy(item.gameObject);
        }
        else
        {
            var panel = container.InstantiatePrefab(_exceptionPanel.gameObject);
            panel.GetComponent<ExceptionPanel>().SetMassage("not enought ash!");
            panel.transform.SetParent(_ui.transform, false);
        }
    }
}
