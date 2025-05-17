using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameState : MonoBehaviour
{
    private UI _ui;
    private Rarity[] _roundsData;
    private Player _player;
    [Inject]
    public void Constructor(UI ui, RoundsData data,Player player)
    {
        _ui = ui;
        _roundsData = data.roundData;
        _player = player;
    }
    public enum State
    {
        playing,
        pause,
        roundOver,
        gameOver
    }
    [NonSerialized]public State state;

    private IEnumerator Start()
    {
        state = State.pause;
        yield return new WaitUntil(() => Input.anyKeyDown);
        state = State.playing;
        _ui.SetRound(_currentRound + 1);
    }

    [SerializeField] private Text timerText;
    private int _currentRound = 0;
    public int TakeCurrentRound => _currentRound;
    public float TakeCurrentScale => _roundsData[_currentRound].enemyScale;

    private float _time;
    private int _currentTime = 1;
    private string _timerTextFormat = "{1}{0}";
    private bool _roundUp = false;
    private void Update()
    {
        if(!_roundUp && state == State.playing)
        {
            Timer(_roundsData[_currentRound].roundTimer);
            _time += Time.deltaTime;
            if (_roundsData[_currentRound].roundTimer == 0)
            {
                CleanArena();
                _roundUp = true;
                state = State.roundOver;
                _currentRound++;
                _ui.CreateShopPanel();
            }
            if (_time > _currentTime)
            {
                _currentTime++;
                _roundsData[_currentRound].roundTimer--;
            }
        }
    }
    private void Timer(int amount)
    {
        if (amount <= 9)
        {
            timerText.text = string.Format(_timerTextFormat, amount, 0);
        }
        else
        {
            timerText.text = string.Format(_timerTextFormat, amount, "");
        }
    }

    public void NextRound(bool done)
    {
        _roundUp = done;
        _ui.SetRound(_currentRound + 1);
    }
    private void CleanArena()
    {
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("bullet");
        foreach (GameObject bullet in bullets) Destroy(bullet);

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
        foreach(GameObject enemy in enemies) Destroy(enemy);

        GameObject[] enemyBullets = GameObject.FindGameObjectsWithTag("enemyBullet");
        foreach(GameObject enemyBullet in enemyBullets) Destroy(enemyBullet);

        GameObject[] currencies = GameObject.FindGameObjectsWithTag("currency");
        foreach(GameObject currency in currencies) Destroy(currency);
        _player.transform.position = Vector2.zero;
    }
}
