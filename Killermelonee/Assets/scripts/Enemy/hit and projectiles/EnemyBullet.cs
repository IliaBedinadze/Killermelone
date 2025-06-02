using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using GS = GameState.State;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private float speed;
    private GameState _gameState;

    [NonSerialized] public Vector2 direction;
    [NonSerialized] public float damage;
    [SerializeField] private float lifeTime;
    private float _time;
    [Inject]
    private void Construct(GameState gamestate)
    {
        _gameState = gamestate;
    }
    private void Start()
    {
        float lookAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0,0,lookAngle);
    }
    private void Update()
    {
        if(_gameState.state == GS.playing)
        {
            _time += Time.deltaTime;
            transform.position = transform.position + (Vector3)direction * speed * Time.deltaTime;
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
