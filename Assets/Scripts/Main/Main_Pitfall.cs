using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 落とし穴
/// </summary>
public class Main_Pitfall : Main_DestroyableObject
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == Main_Constants.TAG_ENEMY && !isDestroying)
        {
            // 敵が上に乗るとダメージを与え、消滅する
            other.GetComponent<Main_EnemyBase>().Damage();
            RunDestroyAnimation();
        }
    }
}
