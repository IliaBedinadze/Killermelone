using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;
using static Zenject.CheatSheet;

public class GameOver : MonoBehaviour
{
    [SerializeField] private Button retryButton;
    [SerializeField] private Button toMenuButton;

    private SceneManagementSc _sceneManagement;
    private UI _ui;
    [Inject]
    private void Constructor(UI ui,SceneManagementSc SM)
    {
        _ui = ui;
        _sceneManagement = SM;
    }
    private void Start()
    {
        retryButton.onClick.AddListener(delegate { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); });
        toMenuButton.onClick.AddListener(delegate { ToMainMenu(); });
        gameObject.transform.SetParent(_ui.gameObject.transform,false);
    }

    private void ToMainMenu()
    {
        _sceneManagement.SceneToLoad = "MainMenu";
        SceneManager.LoadScene("LoadingScreen");
    }
}
