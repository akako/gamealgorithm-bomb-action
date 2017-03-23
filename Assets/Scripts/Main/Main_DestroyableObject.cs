using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Collider2D))]
abstract public class Main_DestroyableObject : MonoBehaviour
{
    protected bool isDestroying = false;

    protected void RunDestroyAnimation()
    {
        isDestroying = true;
        GetComponent<Collider2D>().enabled = false;
        transform.DOScale(Vector3.zero, 1f)
            .OnComplete(() =>
            {
                Destroy(gameObject);
            });
    }
}
