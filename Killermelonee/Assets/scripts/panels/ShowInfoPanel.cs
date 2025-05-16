using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class ShowInfoPanel : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    private bool _activation;
    [Inject]
    private InfoPanelFactory _infoPanelFactory;
    private InfoPanel _infoPanel;

    private UI _ui;
    private Canvas _canvas;
    [Inject]
    public void Constructor(Canvas canvas,UI ui)
    {
        _canvas = canvas;
        _ui = ui;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        StartCoroutine(ShowInfo());
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        _activation = false;
    }
    private IEnumerator ShowInfo()
    {
        _activation = true;
        yield return new WaitForSeconds(0.2f);
        if (_activation && !GetComponent<WeaponIcon>().IsEmptyStatement)
        {
            var _weapon = GetComponent<WeaponIcon>().weaponData;
            var panel = _infoPanelFactory.Create(_weapon);
            panel.transform.SetParent(_ui.transform, false);
            panel.GetComponent<RectTransform>().pivot = PanelSet();

            Vector2 localInput;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.GetComponent<RectTransform>(), Input.mousePosition, _canvas.worldCamera, out localInput);
            panel.GetComponent<RectTransform>().localPosition = localInput;
            if(_infoPanel != null)
            {
                Destroy(_infoPanel.gameObject);
            }
            _infoPanel = panel;
            panel.SetPanel(gameObject.GetComponent<ShowInfoPanel>());
        }
    }
    private Vector2 PanelSet()
    {
        var mousepos = Input.mousePosition;

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        Vector2 pos = Vector2.zero;
        if (mousepos.x >= screenWidth/2)
        {
            pos.x = 1;
        }
        if(mousepos.y >= screenHeight/2)
        {
            pos.y = 1;
        }
        return pos;
    }
    public bool CheckActivated()
    {
        return _activation;
    }
    private void OnDestroy()
    {
        if(_infoPanel != null)
            Destroy(_infoPanel.gameObject);
    }
}
