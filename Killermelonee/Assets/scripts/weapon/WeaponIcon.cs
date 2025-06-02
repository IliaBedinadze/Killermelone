using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class WeaponIcon : MonoBehaviour
{
    public enum IconType
    {
        none,
        shop,
        other
    }
    [SerializeField] private IconType iconType;
    // variable and function to set weaponData
    public WeaponData weaponData { get; private set; }
    public void SetStats(WeaponData data)
    {
        weaponData = data;
    }
    // data to set weapon stats on ui
    [SerializeField] private Text Price;
    [SerializeField] private Text weaponLevelText;
    [SerializeField] private Image currencyImage;
    private string _weaponLevelStringFormat = "lv:{0}";

    // this bool set is for only shop items(not for player items in shop)
    [SerializeField] private bool forSell;

    // change on inspector, if click on this icon should trigger sound
    [SerializeField] private bool playAudio;

    // this bool variable let shop or other sources know that there is no weapon and use empty icon
    private bool _isEmpty;
    public bool IsEmptyStatement => _isEmpty == true;

    // adding audio source for weapon icon click sound
    private SceneAudioController _sceneAudioController;
    [Inject]
    public void Constructor(SceneAudioController controller)
    {
        _sceneAudioController = controller;
    }
    private void Start()
    {
        if(playAudio) 
            GetComponent<Button>().onClick.AddListener(_sceneAudioController.PlayWeaponIconClick);
        if(iconType == IconType.other)
        {
            Destroy(Price.gameObject);
            Destroy(currencyImage.gameObject);
        }
        if (forSell && iconType == IconType.shop)
        {
            weaponLevelText.text = string.Format(_weaponLevelStringFormat, weaponData.currentLevel + 1);
            var image = gameObject.GetComponent<Image>();
            image.sprite = Resources.Load<Sprite>(weaponData.spritePath);
            Price.text = weaponData.price[weaponData.currentLevel].ToString();
        }
    }
    public void ChoosenItemInitialization(bool state,WeaponData Weapondata = null,Sprite noImage = null)
    {
        if (state)
        {
            weaponData = Weapondata;
            weaponLevelText.text = string.Format(_weaponLevelStringFormat, weaponData.currentLevel + 1);
            var image = gameObject.GetComponent<Image>();
            image.sprite = Resources.Load<Sprite>(weaponData.spritePath);
            _isEmpty = false;
        }
        else if (!state)
        {
            weaponLevelText.text = "";
            var image = gameObject.GetComponent<Image>();
            image.sprite = noImage;
            _isEmpty = true;
        }
    }
    public void InitializeSellItem(bool state, Sprite none)
    {
        if (state)
        {
            weaponLevelText.text = string.Format(_weaponLevelStringFormat, weaponData.currentLevel + 1);
            var image = gameObject.GetComponent<Image>();
            image.sprite = Resources.Load<Sprite>(weaponData.spritePath);
            Price.text = (weaponData.TakeCurrentPrice / 2).ToString();
            _isEmpty = false;
        }
        else if (!state)
        {
            weaponLevelText.text = "";
            var image = gameObject.GetComponent<Image>();
            image.sprite = none;
            Price.text = "";
            _isEmpty = true;
        }
    }
}
