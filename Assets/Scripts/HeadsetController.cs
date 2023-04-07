using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 4/2/2023
 * The commented out code was WIP for smooth movement of the headset.
 * For now, the used code translate the headset up and down instantaneously
 */

public class HeadsetController : MonoBehaviour
{
    [SerializeField] private GameObject headset;
    //[SerializeField] private float duration = 1;
    //private Vector3 currentPosition, offsetUp, offsetDown;
    private bool isLowered = false;

    public void MoveHeadset()
    {
        if (!isLowered)
        {
            transform.Translate(0f, -0.25f, 0f);
        }
        else
        {
            transform.Translate(0f, 0.25f, 0f);
        }
        isLowered = !isLowered;
    }

    /*
    void Update()
    {
        currentPosition = transform.position;
        offsetUp = new Vector3(0f, 0.0125f, 0f) + currentPosition;
        offsetDown = new Vector3(0f, -0.0125f, 0f) + currentPosition;
    }
    

    public void GrabHeadset()
    {
        StopAllCoroutines();
        if (!isLowered)
        {
            StartCoroutine(MoveHeadsetDown());
        }
        else
        {
            StartCoroutine(MoveHeadsetUp());
        }
        isLowered = !isLowered;
    }

    IEnumerator MoveHeadsetUp()
    {
        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            transform.position = Vector3.Lerp(currentPosition, offsetUp, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator MoveHeadsetDown()
    {
        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            transform.position = Vector3.Lerp(currentPosition, offsetDown, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }
    */
}
