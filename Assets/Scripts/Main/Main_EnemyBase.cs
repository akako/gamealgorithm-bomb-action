using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Main_EnemyBase : Main_DestroyableObject
{
    protected Rigidbody2D rigidbodyCache;
    protected Vector2 moveVector = Vector2.zero;
    protected float speed;

    void Start()
    {
        rigidbodyCache = GetComponent<Rigidbody2D>();
    }

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
    }

    public void Damage()
    {
        rigidbodyCache.velocity = Vector3.zero;
        RunDestroyAnimation();
    }
}
