using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameState : MonoBehaviour
{
    private UI _ui;
    [Inject]
    public void Constructor(UI ui)
    {
        _ui = ui;
    }
    [NonSerialized]public State state;
    public enum State
    {
        playing,
        pause,
        roundOver,
        gameOver
    }
    private IEnumerator Start()
    {
        state = State.pause;
        yield return new WaitUntil(() => Input.anyKeyDown);
        state = State.playing;
    }

    [SerializeField] private Text timerText;
    private RoundsData _roundsData;
    private int _currentRound = 0;
    public int TakeCurrentRound => _currentRound;
    [Inject]
    public void Container(RoundsData data)
    {
        _roundsData = data;
    }

    private float _time;
    private int _currentTime = 1;
    private string _timerTextFormat = "{1}{0}";
    private bool _roundUp = false;
    private void Update()
    {
        if(!_roundUp && state == State.playing)
        {
            Timer(_roundsData.Rounds[_currentRound]);
            _time += Time.deltaTime;
            if (_roundsData.Rounds[_currentRound] == 0)
            {
                _roundUp = true;
                state = State.roundOver;
                _currentRound++;
                _ui.CreateShopPanel();
            }
            if (_time > _currentTime)
            {
                _currentTime++;
                _roundsData.Rounds[_currentRound]--;
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
    }
}
