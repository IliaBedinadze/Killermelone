using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using GS = GameState.State;

public class EnemySpawnerController : MonoBehaviour
{
    // if its round start and hoard type, let us know to instantly spawn first hoard wave cause it have large delay
    [NonSerialized] public bool firstHoard = true;

    // boss spawn points
    [SerializeField] private Transform[] bossSpawnPoints;

    // to do not dont let more than needes spawners be enabled
    private int _currentSpawnersEnabledCount = 0;

    // methods to get needed round data and dont garbage code
    private int _maxSpawners => _roundsData.roundData[_gameState.TakeCurrentRound].enemyByRound.spawnersAmount;
    private float[] _spawnPeriod => _roundsData.roundData[_gameState.TakeCurrentRound].enemyByRound.roundSpawnRate;
    private string[] _enemyToSpawn => _roundsData.roundData[_gameState.TakeCurrentRound].enemyByRound.baseEnemy;
    private string _spawnType => _roundsData.roundData[_gameState.TakeCurrentRound].enemyByRound.roundType;
    public void SpawnerDisabled() => _currentSpawnersEnabledCount--;

    //in start we place all spawners here for control
    private EnemySpawner[] _spawners;

    [Inject]
    private EnemyFactory _enemyFactory;
    // injecting dependencies
    private RoundsData _roundsData;
    private GameState _gameState;
    [Inject]
    public void Constructor(GameState state,RoundsData data)
    {
        _gameState = state;
        _roundsData = data;
    }
    private void Start()
    {
        _spawners = GetComponentsInChildren<EnemySpawner>(true);
    }
    private void Update()
    {
        // game is not on pause
        if(_gameState.state == GS.playing)
        {
            //code for hoard tipe first wave
            if(_spawnType == "hoard" && firstHoard == true)
            {
                _currentSpawnersEnabledCount = _maxSpawners;
                firstHoard = false;
                for(int i = 0;i < _maxSpawners; i++)
                {
                    StartCoroutine(EnableUnenabledSpawner(true));
                }
            }
            else if(_gameState.state == GS.playing && _currentSpawnersEnabledCount < _maxSpawners)
            {
                _currentSpawnersEnabledCount++;
                StartCoroutine(EnableUnenabledSpawner());
            }
        }
    }
    private IEnumerator EnableUnenabledSpawner(bool first = false)
    {
        while(true)
        {
            var i = UnityEngine.Random.Range(0, _spawners.Length);
            if(_spawners[i].gameObject.activeInHierarchy == false)
            {
                _spawners[i].gameObject.SetActive(true);
                if(!first)
                {
                    var j = UnityEngine.Random.Range(_spawnPeriod[0], _spawnPeriod[1]);
                    _spawners[i].SetSpawnerStats(_enemyToSpawn,j,_spawnType);
                    break; 
                }
                else
                {
                    _spawners[i].SetSpawnerStats(_enemyToSpawn,0,_spawnType);
                    break;
                }
            }
            yield return null;
        }
    }
    public void SpawnBoss(string name)
    {
        var i = UnityEngine.Random.Range(0,bossSpawnPoints.Length);
        var boss = _enemyFactory.Create(name,"Boss");
        boss.transform.position = bossSpawnPoints[i].position;
    }
}
