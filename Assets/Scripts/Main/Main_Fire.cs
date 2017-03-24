using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 炎クラス
/// </summary>
public class Main_Fire : MonoBehaviour
{
    void Start()
    {
        transform.localScale = Vector3.zero;
        // 出現後自動的に消滅させる
        transform.DOScale(Vector3.one, 0.2f).OnComplete(() =>
            {
                transform.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
                    {
                        Destroy(gameObject);
                    });
            });
    }
}
