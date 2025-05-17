using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;
using GS = GameState.State;

public class UI : MonoBehaviour
{
    [SerializeField] private PauseMenu pauseMenu;
    [SerializeField] private GameOver gameOverPanel;
    [SerializeField] private Shop shopPanel;
    [SerializeField] private Text roundText;

    private string _roundTextFormat = "round: {0}";
    private bool _panelActivated;
    private bool _gameOverActivated;
    private GameObject _CurrentPanel;
    private GS _previousState;

    [Inject]
    private DiContainer _container;

    private GameState _gameState;
    [Inject]
    private void Construct(GameState gameState)
    {
        _gameState = gameState;
    }
    private void Start()
    {
        _gameOverActivated = false;
        _panelActivated = false;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !_gameOverActivated)
        {
            if (_panelActivated)
            {
                Destroy(_CurrentPanel);
                _panelActivated = false;
                _gameState.state = _previousState;
            }
            else if (!_panelActivated)
            {
                _CurrentPanel = _container.InstantiatePrefab(pauseMenu.gameObject);
                _previousState = _gameState.state;
                _CurrentPanel.GetComponent<PauseMenu>().SetPreviousState(_previousState);
                _panelActivated = true;
                _gameState.state = GS.pause;
            }
        }
        if(_gameState.state == GS.gameOver && !_gameOverActivated)
        {
            _gameOverActivated = true;
            _CurrentPanel = _container.InstantiatePrefab(gameOverPanel.gameObject);
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
}
