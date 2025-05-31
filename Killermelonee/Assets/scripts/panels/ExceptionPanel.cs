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

    private SceneAudioController _sceneAudioController;
    [Inject]
    public void Constructor(SceneAudioController audioController)
    {
        _sceneAudioController = audioController;
    }
    private void Start()
    {
        backButton.onClick.AddListener(Back);
    }
    public void SetMassage(string Massage)
    {
        massage.text = Massage;
    }
    private void Back()
    {
        _sceneAudioController.PlayClick();
        Destroy(gameObject);
    }
}
