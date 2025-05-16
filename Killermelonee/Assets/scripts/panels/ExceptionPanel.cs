using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using GS = GameState.State;

public class ExceptionPanel : MonoBehaviour
{
    [SerializeField] private Button backButton;
    [SerializeField] private Text massage;

    private void Start()
    {
        backButton.onClick.AddListener(delegate { Destroy(gameObject); });
    }
    public void SetMassage(string Massage)
    {
        massage.text = Massage;
    }
}
