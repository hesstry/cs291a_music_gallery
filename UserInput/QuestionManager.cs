using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class QuestionManager : MonoBehaviour
{
    public List<QuestionAndAnswers> QnA; // a list of questions and their possible answers
    public GameObject[] options; // Button options of the question
    public int currentQuestion; // current question index
    public TextMeshProUGUI QuestionTxt;

    public Parameters RecordedAnswers;


    private void Start()
    {
        generateQuestion();
    }
    public void correct()
    {
        if (QnA.Count > 0)
        {
            QnA.RemoveAt(currentQuestion);
            generateQuestion();
        }
    }
    void generateQuestion()
    {
        if(QnA.Count > 0)
        {
            currentQuestion = Random.Range(0, QnA.Count);

            QuestionTxt.text = QnA[currentQuestion].Question;
            setAnswers();
        }
    }
    void setAnswers()
    {
        for(int i = 0; i < options.Length; i++)
        {
            options[i].GetComponent<AnswerScript>().optionText = QnA[currentQuestion].Answers[i];
            
            if (QnA[currentQuestion].Question.Contains("mood"))
            {
                options[i].GetComponent<AnswerScript>().questionText = "Mood";
            }
            if (QnA[currentQuestion].Question.Contains("color"))
            {
                options[i].GetComponent<AnswerScript>().questionText = "Color";
            }
            if (QnA[currentQuestion].Question.Contains("style"))
            {
                options[i].GetComponent<AnswerScript>().questionText = "Style";
            }

            options[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = QnA[currentQuestion].Answers[i];
        }
    }
}
