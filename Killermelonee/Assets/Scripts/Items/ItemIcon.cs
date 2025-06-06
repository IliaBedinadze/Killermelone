using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemIcon : MonoBehaviour
{
    private ItemStats _stats;

    public void SellItemInitialization(ItemStats stats)
    {
        _stats = stats;
    }
}
