using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
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
