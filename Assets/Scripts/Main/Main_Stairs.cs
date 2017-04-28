using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 階段
/// </summary>
public class Main_Stairs : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (null != other.GetComponent<Main_PlayerCharacter>())
        {
            // プレイヤーが上に乗ったら次のレベルへ
            Main_SceneController.Instance.NextLevel();
        }
    }
}
