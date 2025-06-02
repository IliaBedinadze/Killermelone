using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class VictoryPanel : MonoBehaviour
{
    [SerializeField] private Text titleText;
    [SerializeField] private Text enemyKilledAmount;
    [SerializeField] private Text damageDoneAmount;
    [SerializeField] private Text damageRecieveAmount;
    [SerializeField] private Text levelsClaimedAmount;
    [SerializeField] private Text ashCollectedAmount;

    [SerializeField] private Button againButton;
    [SerializeField] private Button mainMenuButton;

    private GameState _gameState;
    private SceneAudioController _audioController;
    private SceneManagementSc _sceneManagement;
    [Inject]
    public void Constructor(GameState gamestate,SceneAudioController controller,SceneManagementSc management)
    {
        _gameState = gamestate;
        _audioController = controller;
        _sceneManagement = management;
    }
    private void Start()
    {
        againButton.onClick.AddListener(delegate { StartCoroutine(Retry()); });
        mainMenuButton.onClick.AddListener(delegate {StartCoroutine(ToMainMenu());});
        titleText.text = _gameState.VictoryStats.VictoryState ? "Victory" : "Defeat";
        enemyKilledAmount.text = _gameState.VictoryStats.EnemyKilled.ToString();
        damageDoneAmount.text = _gameState.VictoryStats.DamageDone.ToString();
        damageRecieveAmount.text = _gameState.VictoryStats.LevelClaimed.ToString();
        levelsClaimedAmount.text = _gameState.VictoryStats.LevelClaimed.ToString();
        ashCollectedAmount.text = _gameState.VictoryStats.AshCollected.ToString();
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
