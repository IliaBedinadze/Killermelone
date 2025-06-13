using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;
using GS = GameState.State;

public class StoneGolem : EnemyBase,ICollisionHandler
{
    private bool _inDashRange = false;
    private bool _dashTriggered = false;
    private bool _dashCooldawnState = false;

    [SerializeField]private float dashDalay = 1f;
    [SerializeField]private float dashCooldawn = 5f;
    [SerializeField] private float dashSpeedMultiplier = 18;
    private float _time = 0f;

    private Vector2 _direction;
    protected override void Update()
    {
        base.Update();
        if (_gamestate.state == GS.playing && _alive)
        {
            if(!_dashTriggered)
            {
                float step = _enemyStats.speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, _player.position, step);
                if(_inDashRange &&  !_dashCooldawnState)
                {
                    StartCoroutine(MakeDash());
                }
            }
        }
    }
    private IEnumerator MakeDash()
    {
        _dashTriggered = true;
        _dashCooldawnState = true;
        yield return new WaitForSeconds(dashDalay);
        animator.speed = 5;

        _direction = (_player.position - transform.position).normalized;
        while (_time <= 1.5f)
        {
            transform.position = transform.position + (Vector3)_direction * dashSpeedMultiplier * Time.deltaTime;
            _time += Time.deltaTime;
            if (!_alive)
                break;
            yield return null;
        }
        animator.speed = 1;
        _time = 0f;
        _dashTriggered = false;

        while (true)
        {
            _time += Time.deltaTime;
            if (_time >= dashCooldawn)
            {
                _dashCooldawnState = false;
                _time = 0;
                break;
            }
            yield return null;
        }
    }
    public void SentCollisionInfo(bool collided)
    {
        _inDashRange = collided;
    }
}
