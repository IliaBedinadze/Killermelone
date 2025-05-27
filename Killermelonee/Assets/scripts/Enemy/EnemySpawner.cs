using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Zenject;
using GS = GameState.State;

public class EnemySpawner : MonoBehaviour
{
    private float _minY;
    private float _maxY;
    private float _minX;
    private float _maxX;

    private float _spawnPeriod = 0f;
    private float _time = 0f;
    private bool _alreadySpawn;

    [NonSerialized] public string spawnType = "ordinary";
    private GameState _gameState;
    private string[] _enemyToSpawn;

    [Inject]
    private void Construct(GameState gamestate)
    {
        _gameState = gamestate;
    }
    [Inject] private EnemyFactory _enemyFactory;

    private void Start()
    {
        Vector3 size = GetComponent<Renderer>().bounds.size;
        _minX = transform.position.x - (size.x / 2);
        _maxX = transform.position.x + (size.x / 2);
        _minY = transform.position.y - (size.y / 2);
        _maxY = transform.position.y + (size.y / 2);
    }
    public void SetSpawnerStats(string[] enToSpawn,float spawnPeriod,string type)
    {
        spawnType = type;
        _enemyToSpawn = enToSpawn;
        _spawnPeriod = spawnPeriod;
        _time = 0;
    }
    private void Update()
    {
        if (_gameState.state == GS.playing && !_alreadySpawn)
        {
            _time += Time.deltaTime;
            if (_time >= _spawnPeriod)
            {
                if (spawnType == "ordinary")
                    SpawnOrdinarywave();
                else if (spawnType == "hoard")
                    StartCoroutine(SpawnHoardWave());
            }
        }
    }
    private Vector2 GetRandomPosition()
    {
        var randomX = UnityEngine.Random.Range(_minX, _maxX);
        var randomY = UnityEngine.Random.Range(_minY, _maxY);
        return new Vector2(randomX, randomY);
    }
    private void SpawnOrdinarywave()
    {
        var i = UnityEngine.Random.Range(0, _enemyToSpawn.Length);
        var go = _enemyFactory.Create(_enemyToSpawn[i],"ordinary");
        go.transform.position = GetRandomPosition();
        _time = 0f;
        gameObject.SetActive(false);
    }
    private IEnumerator SpawnHoardWave()
    {
        _alreadySpawn = true;
        var i = UnityEngine.Random.Range(0, _enemyToSpawn.Length);
        int j = 0;
        while (true)
        {
            j++;
            var go = _enemyFactory.Create(_enemyToSpawn[i],"ordinary");
            go.transform.position = GetRandomPosition();
            Debug.Log("here");
            yield return new WaitForSeconds(0.1f);
            if (j >= 5) break;
        }
        gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        _alreadySpawn = false;
    }
    private void OnDisable()
    {
        if(GetComponentInParent<EnemySpawnerController>() != null)
            GetComponentInParent<EnemySpawnerController>().SpawnerDisabled();
    }
}
