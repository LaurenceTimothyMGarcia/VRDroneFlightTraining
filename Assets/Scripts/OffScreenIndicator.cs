using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OffScreenIndicator : MonoBehaviour
{
    public GameObject target;
    public Image arrowPoint;

    private Camera mainCamera;
    private Vector3 targetViewportPoint;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        targetViewportPoint = mainCamera.WorldToViewportPoint(target.transform.position);

        if (targetViewportPoint.x < 0 || targetViewportPoint.x > 1 || targetViewportPoint.y < 0 || targetViewportPoint.y > 1)
        {
            // Object is off-screen
            arrowPoint.gameObject.SetActive(true);

            // Move the indicator along the edge of the screen
            Vector2 screenPos = mainCamera.WorldToScreenPoint(target.transform.position);
            Vector2 indicatorPos = screenPos;

            if (targetViewportPoint.x > 0)
            {
                // Object is off-screen to the left
                indicatorPos.x = Screen.width/4;
            }
            else if (targetViewportPoint.x < 1)
            {
                // Object is off-screen to the right
                indicatorPos.x = Screen.width*3/4;
            }

            if (targetViewportPoint.y < 0)
            {
                // Object is off-screen below
                indicatorPos.y = Screen.height/2;
            }
            else if (targetViewportPoint.y > 1)
            {
                // Object is off-screen above
                indicatorPos.y = Screen.height/2;
            }

            // Set the position of the indicator
            arrowPoint.rectTransform.position = indicatorPos;

            // Rotate the indicator to point towards the object
            Vector2 indicatorDirection = (target.transform.position - mainCamera.transform.position).normalized;
            float angle = Mathf.Atan2(indicatorDirection.y, indicatorDirection.x) * Mathf.Rad2Deg - 90f;
            arrowPoint.rectTransform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
        else
        {
            // Object is on-screen
            arrowPoint.gameObject.SetActive(false);
        }
    }
}
