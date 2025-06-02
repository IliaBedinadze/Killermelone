using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// for colliders that is not set on main object
public interface ICollisionHandler
{
    public void SentCollisionInfo(bool collided);
}
