using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponIcon : MonoBehaviour
{
    [NonSerialized] public WeaponData weaponData;
    public enum IconType
    {
        none,
        shop,
        other
    }
    [SerializeField] private IconType iconType;
    public void SetStats(WeaponData data)
    {
        weaponData = data;
    }
    [SerializeField] private Text Price;
    [SerializeField] private Text weaponLevelText;
    [SerializeField] private Image currencyImage;
    private string _weaponLevelStringFormat = "lv:{0}";
    private bool _isEmpty;
    [SerializeField] private bool forSell;
    public bool IsEmptyStatement => _isEmpty == true;
    private void Start()
    {
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
