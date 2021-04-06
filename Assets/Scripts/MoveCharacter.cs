using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCharacter : MonoBehaviour
{
    private bool CanMove;
    private IEnumerator moveCoroutine;

    [SerializeField] private Vector2 nextMove;

    // Start is called before the first frame update
    void Start()
    {
        moveCoroutine = Move((int)nextMove.x, (int)nextMove.y);
        StartCoroutine(moveCoroutine);
    }

    private IEnumerator Move(int _x, int _z)
    {
        CanMove = false;
        float t = 0.0f;

        Vector3 startPos = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        Vector3 finalPos = startPos +new Vector3(_x, 0, _z);

        finalPos.x = (int)Mathf.Round(finalPos.x); // prevent float approx
        finalPos.z = (int)Mathf.Round(finalPos.z);

        while (t <= 1.0f)
        {
            // .. and increase the t interpolater
            t += Time.deltaTime;
            transform.Translate(Time.deltaTime* _x, 0, Time.deltaTime* _z, Space.World);
            yield return new WaitForEndOfFrame();
        }
        transform.position = finalPos;
        CanMove = true;
    }
}
