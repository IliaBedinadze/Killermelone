using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Currency : MonoBehaviour
{
    [NonSerialized]public int CurrencyAmount;
    private Animator _animator;
    private AnimatorStateInfo _stateInfo;
    private AudioSource _audioSource;

    private GameState _gameState;
    [Inject]
    public void Constructor(GameState gameState)
    {
        _gameState = gameState;
    }
    private IEnumerator Start()
    {
        _animator = GetComponent<Animator>();
        yield return new WaitForSeconds(0.5f);
        _stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        _audioSource = GetComponent<AudioSource>();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_stateInfo.normalizedTime >= 1f)
        {
            if (collision.tag == "Player")
            {
                collision.GetComponent<Player>().AddRemoveCurency(true, CurrencyAmount);
                PlaySound();
                Destroy(gameObject);
            }
        }
    }
    private void PlaySound()
    {
        GameObject source = new GameObject("pickUpSound");
        source.transform.position = transform.position;

        AudioSource audio = source.AddComponent<AudioSource>();
        audio.clip = _audioSource.clip;
        audio.volume = _audioSource.volume;
        audio.Play();
        Destroy(audio.gameObject,audio.clip.length);
    }
}
