using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button continueButton;

    private SceneManagementSc _sceneManagementSc;
    [Inject]
    public void Constructor(SceneManagementSc sceneManagementSc)
    {
        _sceneManagementSc = sceneManagementSc;
    }
    private void Start()
    {
        playButton.onClick.AddListener(delegate { PlayClick(false); });
        string path = Application.persistentDataPath + "/save.json";
        if(File.Exists(path))
        {
            continueButton.onClick.AddListener(delegate { PlayClick(true); });
            continueButton.interactable = true;
        }
        exitButton.onClick.AddListener(delegate { Application.Quit(); });
    }
    private void PlayClick(bool _continue)
    {
        _sceneManagementSc.SceneToLoad = "GameScene";
        SceneManager.LoadScene("LoadingScreen");
        _sceneManagementSc.Continue = _continue;
    }
}
