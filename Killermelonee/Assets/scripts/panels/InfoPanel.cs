using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

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
    private string _attackRateStringFormat = "attackrate:{0:F2} {1}";
    private string _pierceAmountStringFormat = "pierce:{0} {1}";

    private Player _player;
    [Inject]
    public void Constructor(Player player)
    {
        _player = player;
    }
    public void SetData(WeaponData data)
    {
        image.sprite = Resources.Load<Sprite>(data.spritePath);
        rarity.text = (data.currentLevel + 1).ToString();
        title.text = data.name;
        description.text = data.description;
        if(data.currentLevel != 4)
        {
            damage.text = string.Format(_damageStringFormat,
                (int)(data.TakeCurrentDamage * _player.CurrentDamageMultiplier),
                "=> " + (int)(data.TakeNextDamage * _player.CurrentDamageMultiplier));
            attackRate.text = string.Format(_attackRateStringFormat,
                (AttackRate(true,data)),
                "=> " + (AttackRate(false, data).ToString("F2"))); 
            if(data.TakeCurrentPierceAmount == -5)
                pierceAmount.text = string.Format(_pierceAmountStringFormat,"eternal", "");
            else
            {
                int i = data.TakeNextPierceAmount + _player.CurrentPierceAddition;
                pierceAmount.text = string.Format(_pierceAmountStringFormat,
                    data.TakeCurrentPierceAmount + _player.CurrentPierceAddition,
                    "=> " + i);

            }
        }
        else
        {
            damage.text = string.Format(_damageStringFormat,
                (int)(data.TakeCurrentDamage * _player.CurrentDamageMultiplier),
                "(max)");
            attackRate.text = string.Format(_attackRateStringFormat,
                (AttackRate(true, data)),
                "(max)");
            if (data.TakeCurrentPierceAmount == -5)
                pierceAmount.text = string.Format(_pierceAmountStringFormat, "eternal", "");
            else
                pierceAmount.text = string.Format(_pierceAmountStringFormat,
                    data.TakeCurrentPierceAmount + _player.CurrentPierceAddition,
                    "(max)");
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

    private float AttackRate(bool current,WeaponData data)
    {
        if (_player.CurrentAttackSpeedMultiplier >= 1)
        {
            if(current) 
                return data.TakeCurrentAR / _player.CurrentAttackSpeedMultiplier;
            else
                return data.TakeNextAR / _player.CurrentAttackSpeedMultiplier;

        }
        else if(_player.CurrentAttackSpeedMultiplier < 1 && _player.CurrentAttackSpeedMultiplier >= 0)
        {
            if(current)
                return data.TakeCurrentAR * (1 + (1 - _player.CurrentAttackSpeedMultiplier));
            else
                return data.TakeNextAR * (1 + (1 - _player.CurrentAttackSpeedMultiplier));

        }
        else
        {
            if(current)
                return data.TakeCurrentAR * (1 + Mathf.Abs(_player.CurrentAttackSpeedMultiplier));
            else
                return data.TakeNextAR * (1 + Mathf.Abs(_player.CurrentAttackSpeedMultiplier));
        }
    }
}
