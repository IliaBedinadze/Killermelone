using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using GS = GameState.State;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private float speed;
    private Transform _player;
    private Vector2 _direction;
    private GameState _gameState;
    [NonSerialized]public float damage;
    [SerializeField] private float lifeTime;
    private float _time;
    [Inject]
    private void Construct(Transform player,GameState gamestate)
    {
        _player = player;
        _gameState = gamestate;
    }
    private void Start()
    {
        _direction = ((Vector2)_player.position - (Vector2)transform.position).normalized;
        float lookAngle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0,0,lookAngle);
    }
    private void Update()
    {
        if(_gameState.state == GS.playing)
        {
            _time += Time.deltaTime;
            transform.position = transform.position + (Vector3)_direction * speed * Time.deltaTime;
            if (_time >= lifeTime)
            {
                Destroy(gameObject);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            collision.GetComponent<Player>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
