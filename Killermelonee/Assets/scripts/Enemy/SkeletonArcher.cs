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
    private bool _delay = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        _alive = true;
    }
    protected override void Update()
    {
        base.Update();
        if (_delay)
        {
            if (_gamestate.state == GS.playing && _alive)
            {
                float step = _enemyStats.speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, _player.position, step);
                if (!_hasShooten)
                {
                    StartCoroutine(MakeShoot());
                }
            }
        }
        else
        {
            _time += Time.deltaTime;
            if (_time >= 0.3f)
            {
                _delay = true;
            }
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
    private IEnumerator MakeShoot()
    {
        animator.SetBool("Shoot", true);
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float snootAnimation = stateInfo.length;

        _hasShooten = true;
        var go = _enemyBulletFactory.Create(_enemyStats.projectileDamage);
        go.transform.position = transform.position;

        yield return new WaitForSeconds(snootAnimation);
        animator.SetBool("Shoot", false);

        yield return new WaitForSeconds(_enemyStats.shootRate);
        _hasShooten = false;
    }
}
