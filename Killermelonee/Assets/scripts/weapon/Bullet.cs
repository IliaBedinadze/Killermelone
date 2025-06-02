using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using GS = GameState.State;

public class Bullet : MonoBehaviour
{
    private float _speed;
    private float _damage;
    private float _lifeTime;
    private float _pierceAmount;
    private bool delay;

    private Vector2 _direction;
    private Transform _player;
    private UI _uI;
    private GameState _gameState;
    private float _time;
    [Inject]
    private void Construct(UI ui,GameState gamestate, Transform player)
    {
        _gameState = gamestate;
        _uI = ui;
        _player = player;
    }
    private void Start()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _direction = (mousePosition - (Vector2)_player.position).normalized;
        float lookAngle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, lookAngle);
        delay = false;
    }
    private void Update()
    {
        if(_gameState.state == GS.playing)
        {
            _time += Time.deltaTime;
            transform.position = transform.position + (Vector3)_direction * _speed * Time.deltaTime;
            if(_time >= _lifeTime)
            {
                Destroy(gameObject);
            }
            if (_pierceAmount <= -5 && !delay)
            {
                delay = true;
                StartCoroutine(Delay());
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "enemy")
        {
            var enemy = collision.GetComponent<EnemyBase>();
            if(_pierceAmount > 0)
            {
                enemy.TakeDamage(_damage);
                _pierceAmount--;
                if(_pierceAmount == 0)
                {
                    Destroy(gameObject);
                }
            }
            if(_pierceAmount <= -5)
            {
                enemy.HittenByLaser(true,_damage);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        var enemy = collision.GetComponent<EnemyBase>();
        if (collision.gameObject.tag == "enemy" && _pierceAmount <= -5)
        {
            enemy.HittenByLaser(false,_damage);
        }
    }
    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.1f);
        delay = false;
    }
    public void SetStats(int pierce,float damage,float lifetime ,float speed)
    {
        _pierceAmount = pierce;
        _damage = damage;
        _lifeTime = lifetime;
        _speed = speed;
    }
}
