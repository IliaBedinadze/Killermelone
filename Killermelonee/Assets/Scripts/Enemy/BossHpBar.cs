using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHpBar : MonoBehaviour
{
    public Slider hpBar;
    public Text bossName;

    private void Update()
    {
        if(hpBar.value <= 0)
        {
            Destroy(gameObject);
        }
    }
}
