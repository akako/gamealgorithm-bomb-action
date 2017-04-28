using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_EnemyHoming : Main_EnemyBase
{
    void Update()
    {
        var playerPos = Main_SceneController.Instance.playerCharacter.transform.position;
        var diff = playerPos - transform.position;
        if (diff.magnitude < 3f)
        {
            // 一定距離以内にプレイヤーが居る場合は追いかける
            moveVector = diff.normalized;
        }
        else
        {
            // それ以外はSimpleと同じ動き
            if (moveVector == Vector2.zero)
            {
                switch (Random.Range(0, 4))
                {
                    case 0:
                        moveVector = new Vector2(1f, 0f);
                        break;
                    case 1:
                        moveVector = new Vector2(-1f, 0f);
                        break;
                    case 2:
                        moveVector = new Vector2(0f, 1f);
                        break;
                    default:
                        moveVector = new Vector2(0f, -1f);
                        break;
                }
            }

            var coordinate = Main_SceneController.Instance.PositionToCoordinate(transform.position + new Vector3(moveVector.x, moveVector.y) * 0.5f);
            if (!Main_SceneController.Instance.IsEmptyCell(coordinate, true))
            {
                moveVector = new Vector2((Random.Range(0, 2) == 1 ? -1 : 1) * moveVector.y, (Random.Range(0, 2) == 1 ? -1 : 1) * moveVector.x);
            }
        }
        rigidbodyCache.velocity = moveVector * speed;
    }
}
