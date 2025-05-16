using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button exitButton;

    private SceneManagementSc _sceneManagementSc;
    [Inject]
    public void Constructor(SceneManagementSc sceneManagementSc)
    {
        _sceneManagementSc = sceneManagementSc;
    }
    private void Start()
    {
        playButton.onClick.AddListener(delegate { PlayClick(); });
        exitButton.onClick.AddListener(delegate { Application.Quit(); });
        if(_sceneManagementSc == null)
        {
            Debug.Log("matata");
        }
    }
    private void PlayClick()
    {
        _sceneManagementSc.SceneToLoad = "GameScene";
        Debug.Log(_sceneManagementSc.SceneToLoad);
        SceneManager.LoadScene("LoadingScreen");
    }
}
