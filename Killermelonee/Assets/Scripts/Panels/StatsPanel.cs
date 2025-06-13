using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;

public class StatsPanel : MonoBehaviour
{
    [SerializeField] private Text maxHPText;
    [SerializeField] private Text speedText;
    [SerializeField] private Text damageText;
    [SerializeField] private Text attackSpeedText;
    [SerializeField] private Text bulletVelocityText;
    [SerializeField] private Text pierceText;
    [SerializeField] private Text farmText;
    private string percentageStringFormat = "{0}%";

    [SerializeField] private Color stateColor;
    [SerializeField] private Color minusColor;
    [SerializeField] private Color plusColor;

    private Player _player;
    [Inject]
    public void Constructor(Player player)
    {
        _player = player;
    }
    private void Start()
    {
        _player.MaxHP.Subscribe(x =>
        {
            maxHPText.text = x.ToString();
            if (x > 100)
                maxHPText.color = plusColor;
            else if (x == 100)
                maxHPText.color = stateColor;
            else if (x < 100)
                maxHPText.color = minusColor;
        }).AddTo(this);
        _player.Speed.Subscribe(x =>
        {
            speedText.text = string.Format(percentageStringFormat, (int)((x - 1) * 100));

            if (x > 1) 
                speedText.color = plusColor;
            else if(x == 1)
                speedText.color = stateColor;
            else if(x < 1) 
                speedText.color = minusColor;
        }).AddTo(this);
        _player.Damage.Subscribe(x =>
        {
            damageText.text = string.Format(percentageStringFormat, (int)((x - 1) * 100));
            if (x > 1)
                damageText.color = plusColor;
            else if (x == 1)
                damageText.color = stateColor;
            else if (x < 1)
                damageText.color = minusColor;
        }).AddTo(this);
        _player.AttackSpeed.Subscribe(x =>
        {
            attackSpeedText.text = string.Format(percentageStringFormat, (int)((x - 1) * 100));
            if (x > 1)
                attackSpeedText.color = plusColor;
            else if (x == 1)
                attackSpeedText.color = stateColor;
            else if (x < 1)
                attackSpeedText.color = minusColor;
        }).AddTo(this);
        _player.Velocity.Subscribe(x => { 
            bulletVelocityText.text = string.Format(percentageStringFormat, (int)((x - 1) * 100));
            if (x > 1)
                bulletVelocityText.color = plusColor;
            else if (x == 1)
                bulletVelocityText.color = stateColor;
            else if (x < 1)
                bulletVelocityText.color = minusColor;
        }).AddTo(this);
        _player.Pierce.Subscribe(x => { 
            pierceText.text = x.ToString();
            if (x > 1)
                pierceText.color = plusColor;
            else if (x == 0)
                pierceText.color = stateColor;
        }).AddTo(this);
        _player.Farm.Subscribe(x =>
        {
            farmText.text = x.ToString();
            if (x > 0)
                farmText.color = plusColor;
            else if (x == 0)
                farmText.color = stateColor;
            else if (x < 0)
                farmText.color = minusColor;
        }).AddTo(this);
    }
}
