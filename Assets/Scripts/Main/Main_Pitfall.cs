using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_Pitfall : Main_DestroyableObject
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == Main_Constants.TAG_ENEMY && !isDestroying)
        {
            other.GetComponent<Main_EnemyBase>().Damage();
            RunDestroyAnimation();
        }
    }
}
