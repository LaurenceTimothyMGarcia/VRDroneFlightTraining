using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateArrow : MonoBehaviour
{
    public DroneController drone;
    public GameObject obj;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0,0,drone.currentYawSpeed));
        //transform.rotation = obj.transform.rotation;
    }
}
