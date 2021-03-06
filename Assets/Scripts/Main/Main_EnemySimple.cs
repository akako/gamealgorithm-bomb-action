﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_EnemySimple : Main_EnemyBase
{
    void Update()
    {
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
            // 障害物が目の前にあったら曲がる
            Turn();
        }
        rigidbodyCache.velocity = moveVector * speed;
    }

    protected override void Turn()
    {
        moveVector = new Vector2((Random.Range(0, 2) == 1 ? -1 : 1) * moveVector.y, (Random.Range(0, 2) == 1 ? -1 : 1) * moveVector.x);
    }
}
