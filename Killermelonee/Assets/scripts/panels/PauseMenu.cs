using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Zenject;
using Zenject.Asteroids;
using GS = GameState.State;
using System.IO;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button menuButton;

    private GS _previousState;

    private SceneManagementSc _sceneManagement;
    private GameState _gameState;
    private UI _ui;
    [Inject]
    private void Construct(GameState gameState,UI ui,SceneManagementSc CM)
    {
        _gameState = gameState;
        _ui = ui;
        _sceneManagement = CM;
    }
    private void Start()
    {
        resumeButton.onClick.AddListener(delegate { ResumeGame(); });
        menuButton.onClick.AddListener(delegate { ToMainMenu(); });
        gameObject.transform.SetParent(_ui.gameObject.transform,false);
    }
    private void ResumeGame()
    {
        _gameState.state = _previousState;
        _ui.PanelActivatedState(false);
        Destroy(gameObject);
    }
    public void SetPreviousState(GS gS)
    {
        _previousState = gS;
    }
    private void ToMainMenu()
    {
        string json = JsonUtility.ToJson(_gameState.SaveData);
        File.WriteAllText(Application.persistentDataPath + "/save.json",json);
        _sceneManagement.SceneToLoad = "MainMenu";
        SceneManager.LoadScene("LoadingScreen");
    }
}
