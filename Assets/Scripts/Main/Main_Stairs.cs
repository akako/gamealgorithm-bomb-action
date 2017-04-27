using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_Stairs : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (null != other.GetComponent<Main_PlayerCharacter>())
        {
            Main_SceneController.Instance.NextLevel();
        }
    }
}
