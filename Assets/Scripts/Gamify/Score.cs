using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Score : MonoBehaviour
{
    public TMP_Text scoreText;

    public ScoringSystem score;

    // Update is called once per frame
    void Update()
    {
        // replace with player score later
        scoreText.text = score.currentScore.ToString("F0") + "/" + score.overallScore;
    }
}
