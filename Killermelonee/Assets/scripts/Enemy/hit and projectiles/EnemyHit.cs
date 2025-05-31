using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemyHit : MonoBehaviour
{
    private GameState _gameState;
    private float _damage;
    private Animator _animator;
    private bool _damageDone = false;
    [Inject]
    private void Construct( GameState gamestate)
    {
        _gameState = gamestate;
    }
    private IEnumerator Start()
    {
        _animator = GetComponent<Animator>();
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length / stateInfo.speed;

        yield return new WaitForSeconds(animationLength);
        Destroy(gameObject);
    }
    public void SetStats(float damage)
    {
        _damage = damage;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && _damageDone == false)
        {
            collision.GetComponent<Player>().TakeDamage(_damage);
            _damageDone = true;
        }
    }
}
