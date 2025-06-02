using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using GS = GameState.State;

public class SkeletonWarrior : EnemyBase,ICollisionHandler
{
    [SerializeField] private Transform lookPoint;
    [SerializeField] private Transform hitPoint;
    [SerializeField] private GameObject hitProjectile;

    [Inject]private DiContainer _container;
    private float _time = 0;
    private bool _attackDelay = true;
    private bool _hasHitten = false;
    private bool _inHitRange = false;
    protected override void Update()
    {
        base.Update();
        if (_gamestate.state == GS.playing && _alive)
        {
            if (_attackDelay)
            {
                float step = _enemyStats.speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, _player.position, step);

                var lookDirection = _player.position - lookPoint.position;
                float lookAngle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
                lookPoint.rotation = Quaternion.Euler(0,0,lookAngle);
                if (!_hasHitten && _inHitRange)
                {
                    StartCoroutine(MakeHit());
                }
            }
        }
    }
    private IEnumerator MakeHit()
    {
        _hasHitten = true;
        _attackDelay = false;
        yield return new WaitForSeconds(0.05f);
        animator.SetBool("Attack", true);

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float attackAnimation = stateInfo.length;

        yield return new WaitForSeconds(attackAnimation / 2);
        var hit = _container.InstantiatePrefab(hitProjectile);
        hit.GetComponent<EnemyHit>().SetStats(_enemyStats.projectileDamage);
        hit.transform.SetParent(hitPoint.transform, false);

        yield return new WaitForSeconds(attackAnimation / 2);
        animator.SetBool("Attack", false);
        _attackDelay = true;

        while (true)
        {
            _time += Time.deltaTime;
            if (_time >= _enemyStats.shootRate)
            {
                _hasHitten = false;
                _time = 0;
                break;
            }
            yield return null;
        }
    }
    public void SentCollisionInfo(bool collided)
    {
        _inHitRange = collided;
    }
}
