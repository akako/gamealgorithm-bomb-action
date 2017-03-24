using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Audioマネージャ
/// </summary>
public class Main_AudioManager : MonoBehaviour
{
    static Main_AudioManager instance;

    public static Main_AudioManager Instance
    {
        get { return instance; }
    }

    public AudioSource put;
    public AudioSource explosion;

    void Awake()
    {
        if (null != instance)
        {
            Destroy(instance.gameObject);
        }
        instance = this;
    }
}
