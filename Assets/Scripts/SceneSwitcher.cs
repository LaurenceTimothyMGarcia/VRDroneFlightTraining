using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void ChangeScene()
    {
        SceneManager.LoadScene("MasterScene");
    }

    public void ExamScene()
    {
        SceneManager.LoadScene("Exam");
    }

    public void CalPolyPomonaScene()
    {
        SceneManager.LoadScene("Cal Poly Pomona");
    }

    public void MainMenuScene()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
