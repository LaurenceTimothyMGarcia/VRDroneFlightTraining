using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoringSystem : MonoBehaviour
{
    public float overallScore = 100f;
    public float scoreDecreasePerSec = 1f;
    public float passingScore = 70f;

    [HideInInspector] public float currentScore;
    
    public DroneController droneState;

    // Start is called before the first frame update
    void Start()
    {
        currentScore = overallScore;
    }

    // Update is called once per frame
    void Update()
    {
        if (droneState.speedOver || droneState.heightOver)
        {
            DecreaseOverLimit();
        }
    }

    void DecreaseOverLimit()
    {
        float scoreDecPerFrame = scoreDecreasePerSec * Time.deltaTime;

        currentScore -= scoreDecPerFrame;
    }
}
