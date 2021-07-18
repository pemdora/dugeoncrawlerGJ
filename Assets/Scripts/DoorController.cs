using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    private Animator animatorController;

    private void Start()
    {
        animatorController = this.GetComponent<Animator>();
    }

    public void OpenDoor()
    {
        animatorController.enabled =true;
    }
}
