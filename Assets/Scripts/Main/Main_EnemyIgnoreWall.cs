using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_EnemyIgnoreWall : Main_EnemyBase
{
    void Update()
    {
        var playerPos = Main_SceneController.Instance.playerCharacter.transform.position;
        var diff = playerPos - transform.position;
        if (diff.magnitude < 5f)
        {
            // 一定距離以内にプレイヤーが居る場合はそちらに向けて加速する
            rigidbodyCache.AddForce(diff.normalized * speed * 3f);
        }
        else
        {
            // ランダムにふわふわ動き回る
            rigidbodyCache.AddForce(new Vector2(Random.Range(-3f, 3f), Random.Range(-3f, 3f)) * speed);
        }
    }
}
