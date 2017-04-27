using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 爆弾
/// </summary>
public class Main_Bom : Main_DestroyableObject
{
    [SerializeField]
    SpriteRenderer body;

    Main_MapGenerator.Coordinate coordinate;
    int firePower;

    /// <summary>
    /// 爆弾を初期化します
    /// </summary>
    /// <param name="coordinate">Coordinate.</param>
    /// <param name="firePower">Fire power.</param>
    public void Initialize(Main_MapGenerator.Coordinate coordinate, int firePower)
    {
        this.coordinate = coordinate;
        this.firePower = firePower;
        StartCoroutine(CountdownCoroutine());
    }

    /// <summary>
    /// 爆発のカウントダウンを開始します
    /// </summary>
    /// <returns>The coroutine.</returns>
    IEnumerator CountdownCoroutine()
    {
        body.transform.DOScale(Vector3.one * 0.6f, 0.3f).SetLoops(-1, LoopType.Yoyo);
        yield return new WaitForSeconds(3f);
        if (!isDestroying)
        {
            Explode();
        }
    }

    /// <summary>
    /// 爆発します
    /// </summary>
    void Explode()
    {
        Main_AudioManager.Instance.explosion.Play();
        RunDestroyAnimation();
        // 炎を出現させる
        Main_SceneController.Instance.SpawnFire(coordinate, firePower);
    }

    void OnCollisionEnter2D(Collision2D collision2D)
    {
        if (collision2D.gameObject.tag == Main_Constants.TAG_FIRE && !isDestroying)
        {
            Explode();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == Main_Constants.TAG_FIRE && !isDestroying)
        {
            Explode();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        GetComponent<Collider2D>().isTrigger = false;
    }
}
