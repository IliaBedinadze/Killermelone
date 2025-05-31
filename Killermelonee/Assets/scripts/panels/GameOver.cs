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
    private SceneAudioController _audioController;
    [Inject]
    private void Constructor(UI ui,SceneManagementSc SM,SceneAudioController audioController)
    {
        _ui = ui;
        _sceneManagement = SM;
        _audioController = audioController;
    }
    private void Start()
    {
        retryButton.onClick.AddListener(delegate { StartCoroutine(Retry()); });
        toMenuButton.onClick.AddListener(delegate { StartCoroutine(ToMainMenu()); });
        gameObject.transform.SetParent(_ui.gameObject.transform,false);
    }

    private IEnumerator ToMainMenu()
    {
        _audioController.PlayClick();
        yield return new WaitForSeconds(0.3f);
        _sceneManagement.SceneToLoad = "MainMenu";
        SceneManager.LoadScene("LoadingScreen");
    }
    private IEnumerator Retry()
    {
        _audioController.PlayClick();
        yield return new WaitForSeconds(0.3f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
