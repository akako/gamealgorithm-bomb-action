using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Main_Bom : Main_DestroyableObject
{
    [SerializeField]
    SpriteRenderer body;

    Main_MapGenerator.Coordinate coordinate;
    int firePower;

    public void Initialize(Main_MapGenerator.Coordinate coordinate, int firePower)
    {
        this.coordinate = coordinate;
        this.firePower = firePower;
        StartCoroutine(CountdownCoroutine());
    }

    IEnumerator CountdownCoroutine()
    {
        body.transform.DOScale(Vector3.one * 0.8f, 0.3f).SetLoops(-1, LoopType.Yoyo);
        yield return new WaitForSeconds(3f);
        if (!isDestroying)
        {
            Explode();
        }
    }

    void Explode()
    {
        Main_AudioManager.Instance.explosion.Play();
        RunDestroyAnimation();
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
