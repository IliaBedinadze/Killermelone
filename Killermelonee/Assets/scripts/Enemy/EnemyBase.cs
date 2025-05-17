using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Zenject.Asteroids;
using Zenject.SpaceFighter;

public class EnemyBase : MonoBehaviour
{
    [Inject]
    protected CurrencyFactory _currencyFactory;
    [SerializeField] protected string ID;

    protected bool _underLaser;
    protected float _laserDamage;
    protected bool delay = false;

    protected AudioSource _audioSource;
    protected bool _alive;
    protected Animator animator;
    protected GameState _gamestate;
    protected Enemy _enemyStats;
    protected Transform _player;
    [Inject]
    private void Construct(Transform player, GameState gamestate)
    {
        _player = player;
        _gamestate = gamestate;
    }
    private void Start()
    {
        animator = GetComponent<Animator>();
        _alive = true;
        _enemyStats.ScaleEnemy(_gamestate.TakeCurrentScale);
    }
    protected virtual void Update()
    {
        if (_underLaser && !delay)
        {
            TakeDamage(_laserDamage);
            delay = true;
            StartCoroutine(Delay());
        }
    }
    protected IEnumerator Death()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.Play();
        var colliders = GetComponents<Collider2D>();
        foreach (var item in colliders)
        {
            Destroy(item);
        }
        _player.GetComponent<Player>().GainXP(_enemyStats.XPGain);
        animator.SetBool("Death", true);
        yield return new WaitForSeconds(1);
        var ash = _currencyFactory.Create(Random.Range(_enemyStats.ashAmount[0], _enemyStats.ashAmount[1] + 1));
        ash.transform.position = transform.position;
        Destroy(gameObject);
    }
    public void SetStats(Enemy enemystats)
    {
        _enemyStats = enemystats;
    }
    public void TakeDamage(float amount)
    {
        _enemyStats.health -= amount;
        if (_enemyStats.health <= 0)
        {
            _alive = false;
            StartCoroutine(Death());
        }
    }
    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.2f);
        delay = false;
    }
    public void HittenByLaser(bool statement,float damageAmount)
    {
        _underLaser = statement;
        _laserDamage = damageAmount;
    }
}
