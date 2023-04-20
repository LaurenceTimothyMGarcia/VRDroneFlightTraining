using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneAnimation : MonoBehaviour
{
    public GameObject[] propellers; //creates an empty array variable for the propellers

    public float currentThrottlePower;
    public void Initialize(GameObject drone)
    { //creates the function for propellers array to be filled with the different propellers onces the game starts.
        propellers = new GameObject[4];
        propellers[0] = drone.transform.Find("NorthWestProp").gameObject;
        propellers[1] = drone.transform.Find("NorthEastProp").gameObject;
        propellers[2] = drone.transform.Find("SouthWestProp").gameObject;
        propellers[3] = drone.transform.Find("SouthWestProp").gameObject;
    }

    void UpdateThrottleSpeed()
    {
         foreach (GameObject propeller in propellers)
    {
        // Get the center of the propeller object
        Vector3 center = propeller.GetComponent<Renderer>().bounds.center;
        
        // Rotate the propeller around its own center
        propeller.transform.RotateAround(center, Vector3.up, currentThrottlePower * Time.deltaTime* 10);//the 10 value can be changed as needed for the speed of the propellers, or removed if default is wanted.
    }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateThrottleSpeed();
    }
}
