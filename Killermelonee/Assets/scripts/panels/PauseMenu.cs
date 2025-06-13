using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Zenject;
using Zenject.Asteroids;
using GS = GameState.State;
using System.IO;
using UniRx;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private WeaponIcon leftHand;
    [SerializeField] private WeaponIcon rightHand;

    [SerializeField] private Button resumeButton;
    [SerializeField] private Button menuButton;

    private GS _previousState;

    // dependencies
    private SceneManagementSc _sceneManagement;
    private GameState _gameState;
    private UI _ui;
    private SceneAudioController _audioController;
    private Player _player;
    [Inject]
    private void Construct(GameState gameState,UI ui,SceneManagementSc CM,SceneAudioController audioController,Player player)
    {
        _gameState = gameState;
        _ui = ui;
        _sceneManagement = CM;
        _audioController = audioController;
        _player = player;
    }
    private void Start()
    {
        var left = _player.leftHand.GetComponentInChildren<WeaponHitter>().weaponData;
        var right = _player.rightHand.GetComponentInChildren<WeaponHitter>().weaponData;
        leftHand.ChoosenItemInitialization(true, left);
        rightHand.ChoosenItemInitialization(true, Weapondata:right);

        resumeButton.onClick.AddListener(delegate { ResumeGame(); });
        menuButton.onClick.AddListener(delegate { StartCoroutine( ToMainMenu()); });
        gameObject.transform.SetParent(_ui.gameObject.transform,false);
    }
    private void ResumeGame()
    {
        _audioController.QuietLouderSong(false);
        _audioController.PlayClick();
        _gameState.state = _previousState;
        _ui.PanelActivatedState(false);
        Destroy(gameObject);
    }
    public void SetPreviousState(GS gS)
    {
        _previousState = gS;
    }
    private IEnumerator ToMainMenu()
    {
        _audioController.PlayClick();
        yield return new WaitForSeconds(0.3f);
        _sceneManagement.SceneToLoad = "MainMenu";
        SceneManager.LoadScene("LoadingScreen");
    }
}
