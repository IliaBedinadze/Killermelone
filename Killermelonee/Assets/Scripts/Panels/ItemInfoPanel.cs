using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoPanel : MonoBehaviour
{
    [SerializeField] private Color plusStatColor;
    [SerializeField] private Color minusStatColor;

    private ShowInfoPanel _showInfoPanel;
    private ItemStats _itemStats;
    private bool delay = false;

    [SerializeField] private Image itemImage;
    [SerializeField] private Text itemName;
    [SerializeField] private Text itemRarity;
    [SerializeField] private Text[] statChangeTexts;

    private void Update()
    {
        if (!delay)
        {
            StartCoroutine(CheckActivationState());
        }
    }
    public void SetStats(ItemStats stats)
    {
        _itemStats = stats;
        itemImage.sprite = Resources.Load<Sprite>(_itemStats.SpritePath);
        itemName.text = _itemStats.Name;
        itemRarity.text = _itemStats.Rarity;
        var i = 0;
        foreach(var item in _itemStats.PlusStats)
        {
            statChangeTexts[i].text = item;
            statChangeTexts[i].color = plusStatColor;
            i++;
        }
        foreach(var item in _itemStats.MinusStats)
        {
            statChangeTexts[i].text = item;
            statChangeTexts[i].color = minusStatColor;
            i++;
        }
    }
    public void SetPanel(ShowInfoPanel panel)
    {
        _showInfoPanel = panel;
    }
    private IEnumerator CheckActivationState()
    {
        delay = true;
        yield return new WaitForSeconds(0.2f);
        if (!_showInfoPanel.CheckActivated())
        {
            Destroy(gameObject);
        }
        delay = false;
    }
}
