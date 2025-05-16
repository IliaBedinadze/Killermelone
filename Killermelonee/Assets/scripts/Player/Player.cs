using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UnityEngine.UI;
using GS = GameState.State;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Slider hpBar;
    [SerializeField] private Text hPText;
    [SerializeField] private Slider xpBar;
    [SerializeField] private Text currencyText;
    [SerializeField] private GameObject lvlUpAnim;
    public Transform leftHand;
    public Transform rightHand;

    [NonSerialized] public float HP = 100;
    [NonSerialized] public int _ashAmount;

    private GameState _state;
    [Inject]
    private void Construct(GameState state)
    {
        _state = state;
    }

    private void Start()
    {
        hpBar.maxValue = HP;
        hpBar.value = HP;
        hPText.text = HP.ToString();
        xpBar.maxValue = 1000;
        xpBar.value = 0;
    }
    private void Update()
    {
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
                transform.Translate(moveDirection * speed * Time.deltaTime,Space.World);
            }
        }
        if (HP <= 0)
        {
            HP -= 0;
            hpBar.value = HP;
            hPText.text = HP.ToString();
            _state.state = GS.gameOver;
        }
    }
    public void TakeDamage(float amount)
    {
        HP -= amount;
        hpBar.value = HP;
        hPText.text = HP.ToString();
    }
    public void GainXP(float amount)
    {
        xpBar.value += amount;
        if(xpBar.value >= xpBar.maxValue)
        {
            StartCoroutine( LevelUp());
            xpBar.value -= xpBar.maxValue;
            xpBar.maxValue = xpBar.maxValue * 1.5f;
        }
    }
    private IEnumerator LevelUp()
    {
        hpBar.maxValue += 25;
        HP += 50;
        hpBar.value = HP;
        hPText.text = HP.ToString();
        var item = Instantiate(lvlUpAnim);
        item.transform.position = transform.position;
        item.transform.SetParent(transform);
        item.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(1.3f);
        Destroy(item);
    }
    public void AddRemoveCurency(bool state,int amount)
    {
        if (state)
        {
            _ashAmount += amount;
            currencyText.text = _ashAmount.ToString();
        }
    }
}
