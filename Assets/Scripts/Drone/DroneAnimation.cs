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
            propeller.transform.Rotate(Vector3.up * currentThrottlePower * Time.deltaTime);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //DroneController droneController = new DroneController();//creates a new instance of the DroneController class and assigns it to a variable
        //droneController.Initialize(gameObject);//calls the function to begin initlizaing the propellers and their animation 
    }

    // Update is called once per frame
    void Update()
    {
        UpdateThrottleSpeed();
    }
}
