using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;
using Zenject.Asteroids;
using Zenject.SpaceFighter;

// every enemy base stats and methods
public class EnemyBase : MonoBehaviour
{
    //under effect of laser type weapon,damage and delay state
    protected bool _underLaser;         
    protected float _laserDamage;
    protected bool laserDelay = false;

    [SerializeField] protected AudioClip deathClip;
    protected bool _alive;
    protected Enemy _enemyStats;
    protected Animator animator;
    protected AudioSource _audioSource;

    private readonly ReactiveProperty<float> hp = new ReactiveProperty<float>(0);
    public IReadOnlyReactiveProperty<float> HP => hp;

    //dependencies
    [Inject]
    protected CurrencyFactory _currencyFactory; 
    protected GameState _gamestate;
    protected Transform _player;
    protected RoundsData _roundsData;
    [Inject]
    private void Construct(Transform player, GameState gamestate,RoundsData roundData)
    {
        _player = player;
        _gamestate = gamestate;
        _roundsData = roundData;
    }
    protected virtual void Start()
    {
        hp.Value = (int)_enemyStats.health;
        _audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        _alive = true;
        _enemyStats.ScaleEnemy(_gamestate.TakeCurrentScale);
    }
    protected virtual void Update()
    {
        if (_underLaser && !laserDelay)
        {
            TakeDamage(_laserDamage);
            laserDelay = true;
            StartCoroutine(Delay());
        }
    }
    protected IEnumerator Death()
    {
        _gamestate.VictoryStats.EnemyKilled++;
        _audioSource.clip = deathClip;
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
        if (_enemyStats.name == "VampireLord")
            _gamestate.BossDefeated();
        Destroy(gameObject);
    }
    public void SetStats(Enemy enemystats)
    {
        _enemyStats = enemystats;
    }
    public void TakeDamage(float amount)
    {
        hp.Value -= amount;
        _gamestate.VictoryStats.DamageDone += (int)amount;
        if (hp.Value <= 0)
        {
            _alive = false;
            StartCoroutine(Death());
        }
    }
    // delay for laser effect below
    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.2f);
        laserDelay = false;
    }
    // enemy undel laser effect
    public void HittenByLaser(bool statement,float damageAmount)
    {
        _underLaser = statement;
        _laserDamage = damageAmount;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _player.GetComponent<Player>().TakeDamage(_enemyStats.contactDamage);
            _gamestate.VictoryStats.DamageRecieve += (int)_enemyStats.contactDamage;
            TakeDamage(_enemyStats.contactDamage);
        }
    }
}
