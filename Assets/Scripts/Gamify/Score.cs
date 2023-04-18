using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Score : MonoBehaviour
{
    public TMP_Text scoreText;

    public DroneController drone;

    // Update is called once per frame
    void Update()
    {
        // replace with player score later
        scoreText.text = "100";
    }
}
