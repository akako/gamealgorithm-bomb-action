using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

/// <summary>
/// プレイヤーキャラ
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Main_PlayerCharacter : Main_DestroyableObject
{
    public static int bomAmount = 3;
    public static int pitfallAmount = 3;
    public static int instantWallAmount = 3;

    int firePower = 4;
    float speed = 3f;
    Rigidbody2D rigidbodyCache;
    float cooldown = 0f;

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

        cooldown -= Time.deltaTime;

        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10f);

        // キー操作に応じて移動
        rigidbodyCache.velocity = new Vector2(CrossPlatformInputManager.GetAxis("Horizontal"), CrossPlatformInputManager.GetAxis("Vertical")).normalized * speed;

        // 各種の罠を置く処理
        if (cooldown < 0f && CrossPlatformInputManager.GetButton("Fire1") && bomAmount > 0 && Main_SceneController.Instance.IsEmptyCell(transform.position))
        {
            bomAmount--;
            Main_SceneController.Instance.SpawnBom(transform.position, firePower);
            cooldown = 0.5f;
        }
        else if (cooldown < 0f && CrossPlatformInputManager.GetButton("Fire2") && pitfallAmount > 0 && Main_SceneController.Instance.IsEmptyCell(transform.position))
        {
            pitfallAmount--;
            Main_SceneController.Instance.SpawnPitfall(transform.position);
            cooldown = 0.5f;
        }
        else if (cooldown < 0f && CrossPlatformInputManager.GetButton("Fire3") && instantWallAmount > 0 && Main_SceneController.Instance.IsEmptyCell(transform.position))
        {
            instantWallAmount--;
            Main_SceneController.Instance.SpawnWall(transform.position);
            cooldown = 0.5f;
        }
    }

    void OnDestroy()
    {
        Main_SceneController.Instance.GameOver();
    }

    void OnCollisionEnter2D(Collision2D collision2D)
    {
        if (collision2D.gameObject.tag == Main_Constants.TAG_FIRE || collision2D.gameObject.tag == Main_Constants.TAG_ENEMY)
        {
            rigidbodyCache.velocity = Vector3.zero;
            RunDestroyAnimation();
        }
    }
}
