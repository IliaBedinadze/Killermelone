using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UnityEngine.UI;
using GS = GameState.State;
using UniRx;
using Unity.VisualScripting;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed;           // Base speed
    [SerializeField] private GameObject lvlUpAnim;  // Animated pref for level up

    //Left/right weapon spawn points
    public Transform leftHand;
    public Transform rightHand;

    private PlayerReactiveStats _playerReactiveStats = new();

    // Expose readonly reactive stats for other objects to subscribe without modifying values
    public IReadOnlyReactiveProperty<int> HP => _playerReactiveStats.HP;
    public IReadOnlyReactiveProperty<int> MaxHP => _playerReactiveStats.MaxHP;
    public IReadOnlyReactiveProperty<float> CurrentXP => _playerReactiveStats.XP;
    public IReadOnlyReactiveProperty<float> MaxXP => _playerReactiveStats.MaxXP;
    public IReadOnlyReactiveProperty<float> Speed => _playerReactiveStats.Speed;
    public IReadOnlyReactiveProperty<float> Damage => _playerReactiveStats.Damage;
    public IReadOnlyReactiveProperty<float> AttackSpeed => _playerReactiveStats.AttackSpeed;
    public IReadOnlyReactiveProperty<int> Pierce => _playerReactiveStats.Pierce;
    public IReadOnlyReactiveProperty<float> Velocity => _playerReactiveStats.Velocity;
    public IReadOnlyReactiveProperty<float> Farm => _playerReactiveStats.Farm;
    public IReadOnlyReactiveProperty<int> ashNum => _playerReactiveStats.AshAmount;

    private CompositeDisposable _disposable = new CompositeDisposable();

    // Dependencies
    private GameState _state;
    private WeaponList _weaponList;
    private SceneManagementSc _management;
    private PlayerStats _playerStats;
    [Inject] DiContainer container;
    [Inject]
    private void Construct(GameState state,WeaponList list,SceneManagementSc sceneManagement,Heroes heroes)
    {
        _state = state;
        _weaponList = list;
        _management = sceneManagement;
        if (!_management.Continue)
        {
            _playerStats = heroes.HeroTypes.First(x => x.Name == _management.ChoosenHeroName);
            _playerReactiveStats.LoadFrom(_playerStats);
        }
    }
    private void Update()
    {
        // Moving and look direction code
        if(_state.state == GS.playing)
        {
            float verticalMove = Input.GetAxis("Vertical");
            float horizontalMove = Input.GetAxis("Horizontal");
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f;
            Vector3 lookDirection = mousePosition - transform.position;
            float lookAngle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0,0,lookAngle - 90);

            Vector2 moveDirection = new Vector2(horizontalMove, verticalMove).normalized;
            if(moveDirection.magnitude > 0.1)
            {
                transform.Translate(moveDirection * (speed * _playerReactiveStats.Speed.Value) * Time.deltaTime,Space.World);
            }
        }
        // Check for player death condition
        if (_playerReactiveStats.HP.Value <= 0)
        {
            _playerReactiveStats.HP.Value = 0;
            _state.state = GS.gameOver;
        }
    }
    // Handle incoming damage
    public void TakeDamage(float amount)
    {
        _playerReactiveStats.HP.Value -= (int)amount;
        _state.VictoryStats.DamageRecieve += (int)amount;
    }
    // Gain experience (usually from enemy kill)
    public void GainXP(float amount)
    {
        _playerReactiveStats.XP.Value += amount;
        if(_playerReactiveStats.XP.Value >= _playerReactiveStats.MaxXP.Value)
        {
            StartCoroutine( LevelUp());
            _playerReactiveStats.XP.Value -= _playerReactiveStats.MaxXP.Value;
            _playerReactiveStats.MaxXP.Value *= 1.5f;
        }
    }
    // Handle level-up logic
    private IEnumerator LevelUp()
    {
        _state.VictoryStats.LevelClaimed++;
        _playerReactiveStats.MaxHP.Value += 25;
        _playerReactiveStats.HP.Value = _playerReactiveStats.HP.Value + 50 > _playerReactiveStats.MaxHP.Value ? _playerReactiveStats.MaxHP.Value : _playerReactiveStats.HP.Value + 50;
        var item = Instantiate(lvlUpAnim);
        item.transform.position = transform.position;
        item.transform.SetParent(transform);
        item.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(1.5f);
        Destroy(item);
    }
    // Add or remove ash currency
    public void AddRemoveCurency(bool fromEnemy,bool state,int amount)
    {
        if (state)
        {
            // Recieve from enemy
            if (fromEnemy)
            {
                _playerReactiveStats.AshAmount.Value += (int)(amount * _state.TakeCurrentScale);
                _state.VictoryStats.AshCollected += (int)(amount * _state.TakeCurrentScale);        //For game over panel only from kill ash scored
            }
            // Other sources
            else
                _playerReactiveStats.AshAmount.Value += (int)amount;

        }
        // Remove ash
        if (!state)
        {
            _playerReactiveStats.AshAmount.Value -= (int)amount;
        }
    }
    // Save player data (weapons and stats)
    public void SaveData()
    {
        if(leftHand.transform.childCount != 0)
        {
            WeaponData left = leftHand.GetComponentInChildren<WeaponBase>().weaponData;
            _state.SaveData.LeftHand = left;
            _state.SaveData.LeftHand.currentLevel = left.currentLevel;
        }
        else
            _state.SaveData.LeftHand = null;
        if(rightHand.transform.childCount != 0)
        {
            WeaponData right = rightHand.GetComponentInChildren<WeaponBase>().weaponData;
            _state.SaveData.RightHand = right;
            _state.SaveData.RightHand.currentLevel = right.currentLevel;
        }
        else
            _state.SaveData.RightHand = null;
        _state.SaveData.PlayerStats = _playerReactiveStats.ToStats();
    }
    // Initialize player weapons from provided WeaponData
    public void InitializeWeapon(WeaponData lefthand,WeaponData righthand)
    {
        if(lefthand != null && Resources.Load<GameObject>(lefthand.prefPath) != null)
        {
            var leftweapon = container.InstantiatePrefab(Resources.Load<GameObject>(lefthand.prefPath));
            leftweapon.transform.SetParent(leftHand.transform, false);
            leftweapon.GetComponent<WeaponBase>().weaponData.currentLevel = lefthand.currentLevel;
        }
        if(righthand != null && Resources.Load<GameObject>(righthand.prefPath) != null)
        {
            var rightweapon = container.InstantiatePrefab(Resources.Load<GameObject>(righthand.prefPath));
            rightweapon.transform.SetParent(rightHand.transform, false);
            rightweapon.GetComponent<WeaponBase>().weaponData.currentLevel = righthand.currentLevel;
        }
    }
    // Initialize player stats when continuing a saved game
    public void InitializePlayerData(PlayerStats stats)
    {
        _playerReactiveStats.LoadFrom(stats);
    }
    // Initialize player weapons by weapon names
    public void InitializeChoozenWeapon(string[] weaponNames)
    {
        var weaponLeft = _weaponList.Weapons.Find(x => x.name == weaponNames[0]);
        var weaponRight = _weaponList.Weapons.Find(x => x.name == weaponNames[1]);
        InitializeWeapon(weaponLeft, weaponRight);
    }
}
