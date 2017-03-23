using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Main_PlayerCharacter : Main_DestroyableObject
{
    int puttableBomCount = 3;
    int firePower = 4;
    float speed = 3f;
    Rigidbody2D rigidbodyCache;

    List<Main_Bom> putBoms = new List<Main_Bom>();

    void Start()
    {
        rigidbodyCache = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isDestroying)
        {
            return;
        }

        rigidbodyCache.velocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * speed;

        putBoms.RemoveAll(x => null == x);
        if (Input.GetButtonDown("Fire1") && putBoms.Count < puttableBomCount && Main_SceneController.Instance.IsBomPuttable(transform.position))
        {
            var bom = Main_SceneController.Instance.SpawnBom(transform.position, firePower);
            putBoms.Add(bom);
        }
    }

    void OnCollisionEnter2D(Collision2D collision2D)
    {
        if (collision2D.gameObject.tag == Main_Constants.TAG_FIRE)
        {
            rigidbodyCache.velocity = Vector3.zero;
            RunDestroyAnimation();
        }
    }
}
