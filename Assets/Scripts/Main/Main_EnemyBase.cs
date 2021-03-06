﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵キャラ基底クラス
/// </summary>
abstract public class Main_EnemyBase : Main_DestroyableObject
{
    protected Rigidbody2D rigidbodyCache;
    protected Vector2 moveVector = Vector2.zero;
    protected float speed;

    void Start()
    {
        rigidbodyCache = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// 初期化します
    /// </summary>
    /// <param name="speed">Speed.</param>
    public void Initialize(float speed)
    {
        this.speed = speed;
    }

    void OnCollisionEnter2D(Collision2D collision2D)
    {
        if (collision2D.gameObject.tag == Main_Constants.TAG_FIRE)
        {
            Damage();
        }
        else if (collision2D.gameObject.tag == Main_Constants.TAG_ENEMY && collision2D.gameObject.GetComponent<Main_EnemyBase>().speed < speed)
        {
            Turn();
        }
    }

    protected virtual void Turn()
    {
    }

    /// <summary>
    /// ダメージを与えます
    /// </summary>
    public void Damage()
    {
        rigidbodyCache.velocity = Vector3.zero;
        RunDestroyAnimation();
    }
}
