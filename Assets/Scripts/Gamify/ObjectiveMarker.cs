using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveMarker : MonoBehaviour
{
    public float sphereDiameter = 25f;

    public Transform sphere;

    private GameObject drone;

    // Start is called before the first frame update
    void Start()
    {
        sphere.localScale = new Vector3(sphereDiameter, sphereDiameter, sphereDiameter);
        drone = GameObject.FindWithTag("Drone");
    }

    void Update()
    {
        if (drone.GetComponent<DroneController>().objectiveMet)
        {
            //Insert win screen here
            // Debug.Log("Objective Met");
        }
    }
}
