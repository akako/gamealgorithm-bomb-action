using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 壊せる壁
/// </summary>
public class Main_Wall : Main_DestroyableObject
{
    void OnCollisionEnter2D(Collision2D collision2D)
    {
        if (collision2D.gameObject.tag == Main_Constants.TAG_FIRE)
        {
            RunDestroyAnimation();
        }
    }
}
