using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTriggerHandler : MonoBehaviour
{
    [SerializeField] private GameObject handle;
    [SerializeField] private string target;
    public bool _isTriggered { get; private set; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "wall" || collision.tag == "Player")
            _isTriggered = true;
        if (collision.tag == target)
        {
            handle.GetComponent<ICollisionHandler>().SentCollisionInfo(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "wall" || collision.tag == "Player")
            _isTriggered = false;
        if (collision.tag == target)
        {
            handle.GetComponent<ICollisionHandler>().SentCollisionInfo(false);
        }
    }
}
