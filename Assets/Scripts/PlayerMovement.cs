using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool CanMove;
    private Vector3 playerVelocity;
    private float playerSpeed = 2.0f;
    private IEnumerator moveCoroutine;

    private void Start()
    {
        CanMove = true;
    }

    private void FixedUpdate()
    {
        float movmentLeftRight = Input.GetAxis("Horizontal");
        float movmentFowrdBackWard = Input.GetAxis("Vertical");
        float movmentRotate = Input.GetAxis("Rotate");

        if (CanMove)
        {
            if (movmentFowrdBackWard > 0)
            {
                moveCoroutine = Move(1, 0);
                StartCoroutine(moveCoroutine);
            }
            else if (movmentFowrdBackWard < 0)
            {
                moveCoroutine = Move(-1, 0);
                StartCoroutine(moveCoroutine);
            }
            else if (movmentLeftRight > 0)
            {
                moveCoroutine = Move(0,-1);
                StartCoroutine(moveCoroutine);
            }
            else if (movmentLeftRight < 0)
            {
                moveCoroutine = Move(0, 1);
                StartCoroutine(moveCoroutine);
            }
            else if (movmentRotate > 0)
            {
                moveCoroutine = Rotate(1);
                StartCoroutine(moveCoroutine);
            }
            else if (movmentRotate < 0)
            {
                moveCoroutine = Rotate(-1);
                StartCoroutine(moveCoroutine);
            }
        }
    }

    private IEnumerator Move(float _x,float _z)
    {
        CanMove = false;
        float t = 0.0f;
        // animate the position of the game object...
       
        Vector3 startPos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 finalPos = startPos + transform.right * _x + transform.forward * _z;

        while (t <= 1.0f)
        {
            // .. and increase the t interpolater
            t += Time.deltaTime;
            float x = Mathf.Lerp(startPos.x, finalPos.x, t);
            float z = Mathf.Lerp(startPos.z, finalPos.z, t);
            transform.position = new Vector3(x, 0, z);
            yield return new WaitForEndOfFrame();
        }
        if (t > 1.0f)
        {
            transform.position = finalPos;
        }
        CanMove = true;
    }
    private IEnumerator Rotate(float _x)
    {
        CanMove = false;
        float t = 0.0f;
        // animate the position of the game object...
        Quaternion startRot = transform.rotation;// Quaternion.Euler(transform.rotation.x, transform.rotation.y, transform.rotation.z);
        Quaternion finalRot = startRot*Quaternion.Euler(0, transform.rotation.y + 90 *_x, 0);

        while (t <= 1.0f)
        {
            // .. and increase the t interpolater
            t += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(startRot, finalRot, t);
            yield return new WaitForEndOfFrame();
        }
        if (t > 1.0f)
        {
            transform.rotation = finalRot;
        }
        CanMove = true;
    }
}
