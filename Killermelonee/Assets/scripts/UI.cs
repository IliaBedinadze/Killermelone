using System.Collections;
using System.Collections.Generic;
using System.IO;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;
using static Zenject.CheatSheet;
using GS = GameState.State;

public class UI : MonoBehaviour
{
    [SerializeField] private PauseMenu pauseMenu;
    [SerializeField] private VictoryPanel gameOverPanel;
    [SerializeField] private Shop shopPanel;
    [SerializeField] private Text roundText;
    [SerializeField] private GameObject chooseWeaponPanel;

    [SerializeField] private Slider hpBar;
    [SerializeField] private Text hPText;
    [SerializeField] private Slider xpBar;
    [SerializeField] private Text currencyText;

    private string _roundTextFormat = "round: {0}";
    private bool _panelActivated;
    private bool _gameOverActivated;
    private GameObject _CurrentPanel;
    private GS _previousState;

    [Inject]
    private DiContainer _container;

    private Player _player;
    private GameState _gameState;
    private SceneAudioController _sceneAudioController;
    private UI _ui;
    private ExceptionPanel _exceptionPanel;
    [Inject]
    private void Construct(GameState gameState,Player player,SceneAudioController audioController,UI ui,ExceptionPanel exception)
    {
        _gameState = gameState;
        _player = player;
        _sceneAudioController = audioController;
        _ui = ui;
        _exceptionPanel = exception;
    }
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);
        _player.MaxHP.Subscribe(x => hpBar.maxValue = x).AddTo(this);
        _player.HP.Subscribe(x => hpBar.value = x).AddTo(this);
        _player.HP.Subscribe(x => hPText.text = x.ToString()).AddTo(this);
        _player.MaxXP.Subscribe(x => xpBar.maxValue = x).AddTo(this);
        _player.CurrentXP.Subscribe(x => xpBar.value = x).AddTo(this);
        _player.ashNum.Subscribe(x => currencyText.text = x.ToString()).AddTo(this);

        _gameOverActivated = false;
        _panelActivated = false;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_panelActivated && _gameState.state == GS.pause)
            {
                _sceneAudioController.QuietLouderSong(false);
                Destroy(_CurrentPanel);
                _panelActivated = false;
                _gameState.state = _previousState;
            }
            else if (!_panelActivated && _gameState.state == GS.playing)
            {
                _sceneAudioController.QuietLouderSong(true);
                _CurrentPanel = _container.InstantiatePrefab(pauseMenu.gameObject);
                _previousState = _gameState.state;
                _CurrentPanel.GetComponent<PauseMenu>().SetPreviousState(_previousState);
                _panelActivated = true;
                _gameState.state = GS.pause;
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape) && !(_gameState.state == GS.playing) && !(_gameState.state == GS.pause))
        {
            var panel = _container.InstantiatePrefab(_exceptionPanel.gameObject);
            panel.GetComponent<ExceptionPanel>().SetMassage("you can pause only on round playtime!");
            panel.transform.SetParent(_ui.transform, false);
        }
        if(_gameState.state == GS.gameOver && !_gameOverActivated)
        {
            _gameOverActivated = true;
            _sceneAudioController.QuietLouderSong(true);
            _CurrentPanel = _container.InstantiatePrefab(gameOverPanel.gameObject);
            _CurrentPanel.transform.SetParent(transform, false);
            string path = Application.persistentDataPath + "/save.json";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
    public void CreateShopPanel()
    {
        _CurrentPanel = _container.InstantiatePrefab(shopPanel.gameObject);
    }
    public void PanelActivatedState(bool state)
    {
        _panelActivated = state;
    }
    public void SetRound(int round)
    {
        roundText.text = string.Format(_roundTextFormat, round);
    }
    public void ChooseWeaponPanelActivation()
    {
        var panel = _container.InstantiatePrefab(chooseWeaponPanel.gameObject);
        panel.transform.SetParent(transform, false);
    }
}
