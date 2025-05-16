using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponIcon : WeaponBase
{
    [SerializeField] private Text Price;
    [SerializeField] private Text weaponLevelText;
    private string _weaponLevelStringFormat = "lv:{0}";
    private bool _isEmpty;
    [SerializeField] private bool forSell;
    public bool IsEmptyStatement => _isEmpty == true;
    private void Start()
    {
        if (forSell)
        {
            weaponLevelText.text = string.Format(_weaponLevelStringFormat, weaponData.currentLevel + 1);
            var image = gameObject.GetComponent<Image>();
            image.sprite = Resources.Load<Sprite>(weaponData.spritePath);
            Price.text = weaponData.price[weaponData.currentLevel].ToString();
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
