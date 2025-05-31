using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Text title;
    [SerializeField] private Text rarity;
    [SerializeField] private Text damage;
    [SerializeField] private Text attackRate;
    [SerializeField] private Text description;
    [SerializeField] private Text pierceAmount;

    private ShowInfoPanel _showInfoPanel;
    private bool delay = false;

    private string _damageStringFormat = "damage:{0} {1}";
    private string _attackRateStringFormat = "attackrate:{0} {1}";
    private string _pierceAmountStringFormat = "pierce:{0} {1}";
    public void SetData(WeaponData data)
    {
        image.sprite = Resources.Load<Sprite>(data.spritePath);
        rarity.text = (data.currentLevel + 1).ToString();
        title.text = data.name;
        description.text = data.description;
        if(data.currentLevel != 4)
        {
            damage.text = string.Format(_damageStringFormat, data.TakeCurrentDamage, "=> " + data.TakeNextDamage);
            attackRate.text = string.Format(_attackRateStringFormat, data.TakeCurrentAR, "=> " + data.TakeNextAR); 
            if(data.TakeCurrentPierceAmount == -5)
                pierceAmount.text = string.Format(_pierceAmountStringFormat,"eternal", ""); 
            else
                pierceAmount.text = string.Format(_pierceAmountStringFormat, data.TakeCurrentPierceAmount, "=> " + data.TakeNextPierceAmount); 

        }
        else
        {
            damage.text = string.Format(_damageStringFormat, data.TakeCurrentDamage, "(max)");
            attackRate.text = string.Format(_attackRateStringFormat, data.TakeCurrentAR, "(max)");
            if (data.TakeCurrentPierceAmount == -5)
                pierceAmount.text = string.Format(_pierceAmountStringFormat, "eternal", "");
            else
                pierceAmount.text = string.Format(_pierceAmountStringFormat, data.TakeCurrentPierceAmount, "(max)");
        }
    }

    private void Update()
    {
        if (!delay)
        {
            StartCoroutine(CheckActivationState());
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
