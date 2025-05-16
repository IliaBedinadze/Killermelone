using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Currency : MonoBehaviour
{
    [NonSerialized]public int CurrencyAmount;
    private Animator _animator;
    private AnimatorStateInfo _stateInfo;
    private IEnumerator Start()
    {
        _animator = GetComponent<Animator>();
        yield return new WaitForSeconds(0.5f);
        _stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_stateInfo.normalizedTime >= 1f)
        {
            if (collision.tag == "Player")
            {
                collision.GetComponent<Player>().AddRemoveCurency(true, CurrencyAmount);
                Destroy(gameObject);
            }
        }
    }
}
