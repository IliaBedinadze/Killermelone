using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionPanel : MonoBehaviour
{
    [SerializeField] private Text descriptionText;

    private ShowInfoPanel _showInfoPanel;
    private bool delay = false;

    public void SetDescription(string text)
    {
        descriptionText.text = text.Replace("\\n","\n");
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
