using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool CanMove;
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

        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        bool canMoveForward = true;
        bool canMoveBackward = true;
        Vector3 right = transform.TransformDirection(Vector3.right);
        bool canMoveRight = true;
        bool canMoveLeft = true;

        if (Physics.Raycast(transform.position, fwd, 1))
            canMoveForward = false;
        if (Physics.Raycast(transform.position, -fwd, 1))
            canMoveBackward = false;

        if (Physics.Raycast(transform.position, right, 1))
            canMoveRight = false;
        if (Physics.Raycast(transform.position, -right, 1))
            canMoveLeft = false;

        if (CanMove)
        {
            if (movmentFowrdBackWard > 0 && canMoveForward)
            {
                moveCoroutine = Move(1, 0);
                StartCoroutine(moveCoroutine);
            }
            else if (movmentFowrdBackWard < 0 && canMoveBackward)
            {
                moveCoroutine = Move(-1, 0);
                StartCoroutine(moveCoroutine);
            }
            else if (movmentLeftRight > 0 && canMoveRight)
            {
                moveCoroutine = Move(0,1);
                StartCoroutine(moveCoroutine);
            }
            else if (movmentLeftRight < 0 && canMoveLeft)
            {
                moveCoroutine = Move(0, -1);
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

    private IEnumerator Move(int _x,int _z)
    {
        CanMove = false;
        float t = 0.0f;
        // animate the position of the game object...
       
        Vector3 startPos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 finalPos = startPos + transform.forward * _x + transform.right * _z;
        finalPos.x = (int)Mathf.Round(finalPos.x); // prevent float approx
        finalPos.z = (int)Mathf.Round(finalPos.z);
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
    private IEnumerator Rotate(int _x)
    {
        CanMove = false;
        float t = 0.0f;
        // animate the position of the game object...
        Quaternion startRot = transform.rotation;
        Quaternion finalRot = startRot * Quaternion.Euler(0, 90 * _x, 0);
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
