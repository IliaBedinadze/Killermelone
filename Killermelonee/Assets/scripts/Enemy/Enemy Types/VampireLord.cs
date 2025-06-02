using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;
using GS = GameState.State;

public class VampireLord : EnemyBase
{
    [Inject]
    private EnemyBulletFactory _enemyBulletFactory;
    private bool _hasShooten = true;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Transform shootRotationPoint;

    private float _time = 0;
    private bool _attackDelay = true;

    private Collider2D[] _colliders;

    [SerializeField] private AudioClip[] vampireLaugh;

    [SerializeField] private Transform[] TeleportPoints;
    private BossHpBar _bossHpBar;
    private UI _ui;
    [Inject]
    public void Constructor(EnemyList list,BossHpBar bossHP,UI ui)
    {
        _ui = ui;
        _bossHpBar = bossHP;
        var enemystats = GetComponent<EnemyBase>();

        string json = JsonUtility.ToJson(list.Bosses[0]);
        SetStats(JsonUtility.FromJson<Enemy>(json));
    }
    protected override void Start()
    {
        base.Start();

        var panel = Instantiate(_bossHpBar.gameObject);
        panel.transform.SetParent(_ui.transform, false);
        var bossHPBar = panel.GetComponent<BossHpBar>();
        bossHPBar.hpBar.maxValue = HP.Value;
        bossHPBar.bossName.text = _enemyStats.name;
        HP.Subscribe(x => bossHPBar.hpBar.value = x).AddTo(this);

        _colliders = GetComponents<Collider2D>();
        StartCoroutine(StartDelay());
    }
    private IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(2);
        _hasShooten = false;
    }
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
                    StartCoroutine(Vanish());
                }
            }
        }
    }
    private IEnumerator Reveal()
    {
        animator.SetInteger("Teleport", 2);
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float snootAnimation = stateInfo.length;
        yield return new WaitForSeconds(snootAnimation);
        foreach (var item in _colliders)
        {
            item.enabled = true;
        }
        StartCoroutine(MakeShoot());
    }
    private IEnumerator Vanish()
    {
        int j = Random.Range(0,vampireLaugh.Length);
        _audioSource.clip = vampireLaugh[j];
        _audioSource.Play();
        _hasShooten = true;
        _attackDelay = false;
        foreach (var item in _colliders)
        {
            item.enabled = false;
        }
        animator.SetInteger("Teleport",1);
        List<Transform> list = new List<Transform>();
        foreach (Transform t in TeleportPoints)
        {
            if(!t.GetComponent<CollisionTriggerHandler>()._isTriggered)
                list.Add(t);
        }
        Debug.Log(list.Count);
        int i = Random.Range(0, list.Count);
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float snootAnimation = stateInfo.length/stateInfo.length;
        yield return new WaitForSeconds(snootAnimation);
        transform.position = list[i].position;
        StartCoroutine(Reveal());
    }
    private IEnumerator MakeShoot()
    {
        animator.SetBool("Attack", true);
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float shootAnimation = stateInfo.length;
        yield return new WaitForSeconds(0.2f);

        Vector2 direction = (_player.transform.position - transform.position).normalized;
        float lookAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        shootRotationPoint.rotation = Quaternion.Euler(0, 0, lookAngle);
        List<float> list = new List<float>() {lookAngle,lookAngle+30,lookAngle -30 };

        foreach (var t in list)
        {
            float rad = t * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
            var go = _enemyBulletFactory.Create(dir, _enemyStats.projectileDamage, _enemyStats.name);
            go.transform.position = shootPoint.position;
        }
        yield return new WaitForSeconds(shootAnimation);
        animator.SetBool("Attack", false);
        _attackDelay = true;

        while (true)
        {
            _time += Time.deltaTime;
            if (_time >= _enemyStats.shootRate)
            {
                _hasShooten = false;
                _time = 0;
                break;
            }
            yield return null;
        }
    }
}
