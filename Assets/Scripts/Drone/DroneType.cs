using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Template for scriptable object for drone
[CreateAssetMenu(fileName = "New Drone", menuName = "Drone")]
public class DroneType : ScriptableObject
{
    // Name of drone
    public new string name;

    // 3D model of drone
    public GameObject droneModel;

    // Movement and rotation related parameters of drone
    public float pitchSpeed;
    public float rollSpeed;
    public float yawSpeed;
    public float throttlePower;

    // Mass of drone
    public float mass;
}
