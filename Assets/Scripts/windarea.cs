using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class windarea : MonoBehaviour
{
    public float WindStrengthMin = 0;
    public float WindStrengthMax = 5;
    public float radius = 100;

    float windStrength;
    RaycastHit hit;
    Collider[] hitColliders;
   // Rigidbody wz;

    public Vector3 windDirection = new Vector3(0, 0, -1);

  

}
