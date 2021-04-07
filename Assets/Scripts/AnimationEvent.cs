using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private GameObject objectToEnable;
    [SerializeField] private GameObject objectToDisable;

    [SerializeField] private GameObject aplhaObj;
    public void DisableGameObject()
    {
        objectToDisable.SetActive(false);
    }

    public void EnableGameObject()
    {
        objectToEnable.SetActive(true);
    }
    public void SetAlpha()
    {
        aplhaObj.GetComponent<CanvasGroup>().alpha = 1f;
    }
}
