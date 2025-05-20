using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using GS = GameState.State;

public class SkeletonArcher : EnemyBase
{
    [Inject]
    private EnemyBulletFactory _enemyBulletFactory;
    private bool _hasShooten = false;

    private float _time = 0;
    private bool _attackDelay = true;

    protected override void Update()
    {
        base.Update();
        if (_gamestate.state == GS.playing && _alive)
        {
            if (_attackDelay)
            {
                float step = _enemyStats.speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, _player.position, step);
                if (!_hasShooten)
                {
                    StartCoroutine(MakeShoot());
                }
            }
        }
    }
    private IEnumerator MakeShoot()
    {
        _hasShooten = true;
        _attackDelay = false;
        yield return new WaitForSeconds(0.05f);
        animator.SetBool("Shoot", true);
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float shootAnimation = stateInfo.length;

        Vector2 direction = (_player.transform.position - transform.position).normalized;
        var go = _enemyBulletFactory.Create( direction, _enemyStats.projectileDamage,_enemyStats.name);
        go.transform.position = transform.position;

        yield return new WaitForSeconds(shootAnimation);
        animator.SetBool("Shoot", false);
        _attackDelay = true;

        while(true)
        {
            _time += Time.deltaTime;
            if (_time >= _attackDelayTimer)
            {
                _hasShooten = false;
                _time = 0;
                break;
            }
            yield return null;
        }
    }
}
