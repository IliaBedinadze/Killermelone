using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Zenject.SpaceFighter;
using GS = GameState.State;

public class Zombie : EnemyBase
{
    private void Start()
    {
        animator = GetComponent<Animator>();
        _alive = true;
    }
    protected override void Update()
    {
        base.Update();
        if (_gamestate.state == GS.playing && _alive)
        {
            float step = _enemyStats.speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, _player.position, step);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _player.GetComponent<Player>().TakeDamage(_enemyStats.contactDamage);
            TakeDamage(_enemyStats.contactDamage);
        }
    }
}
