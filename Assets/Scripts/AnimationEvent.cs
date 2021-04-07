using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private GameObject objectToEnable;
    [SerializeField] private GameObject objectToDisable;

    public void DisableGameObject()
    {
        objectToDisable.SetActive(false);
    }

    public void EnableGameObject()
    {
        objectToEnable.SetActive(true);
    }
}
