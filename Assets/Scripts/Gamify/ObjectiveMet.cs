using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectiveMet : MonoBehaviour
{
    public DroneController drone;
    public ScoringSystem score;

    public GameObject uiPopUp;
    public Image backgroundCol;
    public TMP_Text passFailPrompt;
    public TMP_Text finalScore;

    // Start is called before the first frame update
    void Start()
    {
        uiPopUp.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        ShowScreen();
    }

    void ShowScreen()
    {
        if (drone.objectiveMet)
        {
            // If player passes 
            if (score.currentScore >= score.passingScore)
            {
                backgroundCol.color = new Color(0, 255, 0, 0.1f);
                passFailPrompt.text = "Pass";
            }
            else
            {
                backgroundCol.color = new Color(255, 0, 0, 0.1f);
                passFailPrompt.text = "Fail";
            }

            finalScore.text = "Score: " + score.currentScore.ToString("F0") + "/" + score.overallScore;

            uiPopUp.SetActive(true);

        }
    }
}
