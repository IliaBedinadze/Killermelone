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

    private Player _player;
    [Inject]
    public void Constructor(Player player)
    {
        _player = player;
    }
    private void Start()
    {
        _player.MaxHP.Subscribe(x => maxHPText.text = x.ToString()).AddTo(this);
        _player.Speed.Subscribe(x => speedText.text = string.Format(percentageStringFormat, (x - 1) * 100)).AddTo(this);
        _player.Damage.Subscribe(x => damageText.text = string.Format(percentageStringFormat, (x - 1) * 100)).AddTo(this);
        _player.AttackSpeed.Subscribe(x => attackSpeedText.text = string.Format(percentageStringFormat, (x - 1) * 100)).AddTo(this);
        _player.Velocity.Subscribe(x => bulletVelocityText.text = string.Format(percentageStringFormat, (x - 1) * 100)).AddTo(this);
        _player.Pierce.Subscribe(x => pierceText.text = x.ToString()).AddTo(this);
        _player.Farm.Subscribe(x => farmText.text = string.Format(percentageStringFormat, (x - 1) * 100)).AddTo(this);
    }
}
