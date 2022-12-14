using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using System.Threading;

public class QuestionManager : MonoBehaviour
{
    public List<QuestionAndAnswers> QnA; // a list of questions and their possible answers
    public GameObject[] options; // Button options of the question
    public int currentQuestion; // current question index
    public TextMeshProUGUI QuestionTxt;

    public Parameters RecordedAnswers;

    public GameObject QuestionPanel;
    public GameObject DisplayPanel;

    public UnityEngine.UI.Image imageToDisplay; // Assign in Inspector the UI Image

    private void Start()
    {
        DisplayPanel.SetActive(false);
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

    // After the user's input, display the image
    // function: disable the Question panel, enable the display panel
    
    async void DisplayImage(string ImagePath)
    {
        
        QuestionPanel.SetActive(false);
        DisplayPanel.SetActive(true);

        /*
        while (File.Exists(ImagePath) == false)
        {
            Thread.Sleep(1000);
        }
        */

        // To make sure get the image first before display
        await RetrieveGeneratedImages(RecordedAnswers, ImagePath);
        

        if (File.Exists(ImagePath))
        {
            // 1.read the bytes array
            byte[] bytes = File.ReadAllBytes(ImagePath);
            // 2.create a texture
            //The default size is not important since it will be overridden by the loading method.
            Texture2D tex = new Texture2D(1, 1);
            // 3.load inside tx the bytes and use the correct image size
            tex.LoadImage(bytes);

            // 4.create a rect using the textute dimensions
            Rect rec = new Rect(0, 0, tex.width, tex.height);
            // 5. convert the texture in sprite
            Sprite spriteToUse = Sprite.Create(tex, rec, new Vector2(0.5f, 0.5f), 100);
            // 6.load the sprite used by UI Image
            imageToDisplay.sprite = spriteToUse;
        }
        else
        {
            print("No Image File.");
        }
    }

    void generateQuestion()
    {
        if(QnA.Count > 0)
        {
            currentQuestion = UnityEngine.Random.Range(0, QnA.Count);

            QuestionTxt.text = QnA[currentQuestion].Question;
            setAnswers();
        }
        else
        {
            string BathImagePath = "Assets/Images/";
            string[] dirs = System.IO.Directory.GetFileSystemEntries(BathImagePath);
            int ImageNum = dirs.Length + 1;

            //form image path
            string ImagePath = BathImagePath + ImageNum.ToString() + ".png";

            //RetrieveGeneratedImages(RecordedAnswers, ImagePath);

            DisplayImage(ImagePath);
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

    
    string ProcessUserInputs(Parameters userInputs)
    {
        var buildString = "Guns, soul, ruins,";
        //List<string> keys = new List<string> userInputs.recordedAnswers.Keys;
        if (userInputs.recordedAnswers.ContainsKey("Color"))
        {
            buildString = buildString + " " + userInputs.recordedAnswers["Color"] + ",";
        }
        if (userInputs.recordedAnswers.ContainsKey("Style"))
        {
            buildString = buildString + " in" + userInputs.recordedAnswers["Style"];
        }
        if (userInputs.recordedAnswers.ContainsKey("Mood"))
        {
            buildString = buildString + " with " + userInputs.recordedAnswers["Mood"] + " vibe";
        }
        return buildString;
    }

    // POST
    async Task<bool> RetrieveGeneratedImages(Parameters userInputs, string savePath)
    {
        HttpClient client = new();
        // Add our Auth token
        client.DefaultRequestHeaders.Add("Authorization", "Bearer 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");
        /* Prepare request's data
         * Only generate 1 image for now
         */
        object data = new
        {
            prompt = ProcessUserInputs(userInputs),
            n = 1,
            size = "1024x1024",
            response_format = "b64_json"
        };
        var myContent = JsonConvert.SerializeObject(data);
        var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
        var byteContent = new ByteArrayContent(buffer);
        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        // Send POST request
        var response = await client.PostAsync("https://api.openai.com/v1/images/generations", byteContent);
        // Convert response to JSON
        var responseString = await response.Content.ReadAsStringAsync();
        var dynamicResponseObject = JsonConvert.DeserializeObject<dynamic>(responseString)!;
        /* b64_json.ToString() contains the base64 encoding of the generated image
         */
        string imageString = dynamicResponseObject.data[0].b64_json.ToString();
        byte[] imageBytes = Convert.FromBase64String(imageString);
        // Replace with the appropriate path
        File.WriteAllBytes(savePath, imageBytes);

        //print("Save Image function ends");
        return true;
    }

   
}
