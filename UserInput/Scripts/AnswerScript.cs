using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using TMPro;
using UnityEngine;

public class AnswerScript : MonoBehaviour
{

    public QuestionManager questionManager;
    public string optionText;
    public string questionText;

    public void answer()
    {   
        // print(questionText + ": " + optionText);
        questionManager.RecordedAnswers.recordedAnswers.Add(questionText, optionText);

        /*
        foreach (string key in questionManager.RecordedAnswers.recordedAnswers.Keys)
        {
            print(key + questionManager.RecordedAnswers.recordedAnswers[key]);
        }
        */
        

        questionManager.correct();
    }

}
