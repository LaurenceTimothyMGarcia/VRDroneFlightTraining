using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuizManager : MonoBehaviour
{
    public List<QuestionAnswers> QA;
    public GameObject[] options;
    public int currentQuestion;
    public Text Qtext;
    public Text Scoretext;
    int totalquestion = 0;
    public int score;
    public GameObject Quizpanel;
    public GameObject QOPanel;
    float currentTime = 0f;
    float startTime = 300f;

    [SerializeField] Text countdownText;

    private void Start()
    {
        currentTime = startTime;
        totalquestion = QA.Count;
        QOPanel.SetActive(false);
        generateQ();
    }

    private void Update()
    {
        currentTime -= 1 * Time.deltaTime;
        countdownText.text = currentTime.ToString("0");

        if (currentTime <= 0)
        {
            currentTime = 0;
            QuizOver();
        }
    }

    public void retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuizOver()
    {
        Quizpanel.SetActive(false);
        QOPanel.SetActive(true);
        Scoretext.text = score + "/" + totalquestion;
    }

    public void correct()
    {
        score += 1;
        QA.RemoveAt(currentQuestion);
        generateQ();
    }

    public void wrong()
    {
        QA.RemoveAt(currentQuestion);
        generateQ();
    }

    void SetAnswers()
    {
        for(int i = 0; i < options.Length ; i++)
        {
            options[i].GetComponent<Answers>().isCorrect = false;
            options[i].transform.GetChild(0).GetComponent<Text>().text = QA[currentQuestion].Answers[i];

            if(QA[currentQuestion].CorrectAnswer == i +1)
            {
                options[i].GetComponent<Answers>().isCorrect = true;
            }
        }
    }

    void generateQ()
    {
        if(QA.Count > 0)
        {
            currentQuestion = Random.Range(0, QA.Count);

            Qtext.text = QA[currentQuestion].Question;
            SetAnswers();
        }
        else
        {
            Debug.Log("Out of questions");
            QuizOver();
        }
        

    }


}
