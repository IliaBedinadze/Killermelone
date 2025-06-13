using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ItemIcon : MonoBehaviour
{
    private ItemStats _stats = new();
    [SerializeField] private Image _icon;
    [SerializeField] private Text price;

    public int TakePrice => _stats.Price;
    public ItemStats TakeStats => _stats;

    private SceneAudioController _sceneAudioController;
    private Player _player;
    [Inject]
    public void Constructor(SceneAudioController controller,Player player)
    {
        _sceneAudioController = controller;
        _player = player;
    }
    public void SellItemInitialization(ItemStats stats)
    {
        _stats = stats;
        _icon.sprite = Resources.Load<Sprite>(_stats.SpritePath);
        price.text = _stats.Price.ToString();
    }
    public void ItemStatsAddition()
    {
        _player.PlayerStatsChange(_stats.PlayerStats);
    }
}
