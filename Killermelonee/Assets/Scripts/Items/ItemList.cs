using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemList
{
    public ItemStats[] CommonItems;
    public ItemStats[] RareItems;
    public ItemStats[] EpicItems;
    public ItemStats[] LegendaryItems;
    public ItemStats[] UniqueItems;
}
[System.Serializable]
public class ItemStats
{
    public string Name;
    public string Rarity;
    public int Price;
    public string[] PlusStats;
    public string[] MinusStats;
    public string SpritePath;
    public PlayerStats PlayerStats;
}
