using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Main_Fire : MonoBehaviour
{
    void Start()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 0.2f).OnComplete(() =>
            {
                transform.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
                    {
                        Destroy(gameObject);
                    });
            });
    }
}
