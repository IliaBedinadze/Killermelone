using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using GS = GameState.State;

public class EnemySpawner : MonoBehaviour
{
    private float _minY;
    private float _maxY;
    private float _minX;
    private float _maxX;
    private float _spawnPeriod = 1f;
    private float _time = 0f;
    private bool _spawnCondition = true;
    private int _collidedAmount = 0;

    [Inject] DiContainer _container;

    private GameState _gameState;

    [Inject]
    private void Construct(GameState gamestate)
    {
        _gameState = gamestate;
    }
    [Inject] private EnemyFactory _enemyFactory;

    private void Update()
    {
        if (_gameState.state == GS.playing && _spawnCondition)
        {
            Vector3 size = GetComponent<Renderer>().bounds.size;
            _minX = transform.position.x - (size.x / 2);
            _maxX = transform.position.x + (size.x / 2);
            _minY = transform.position.y - (size.y / 2);
            _maxY = transform.position.y + (size.y / 2);
            _time += Time.deltaTime;
            if (_time >= _spawnPeriod)
            {
                var go = _enemyFactory.Create();
                go.transform.position = GetRandomPosition();
                _time = 0f;
            }
        }
    }
    private Vector2 GetRandomPosition()
    {
        var randomX = Random.Range(_minX, _maxX);
        var randomY = Random.Range(_minY, _maxY);
        return new Vector2(randomX, randomY);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "wall")
        {
            _spawnCondition = false;
            _collidedAmount++;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "wall")
        {
            _collidedAmount--;
            if (_collidedAmount == 0)
            {
                _spawnCondition = true;
            }
        }
    }
}
