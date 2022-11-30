using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueScript : MonoBehaviour
{
    public MusicManager musicManager;
    public void ClickContinueButton()
    {
        musicManager.MusicPanel.SetActive(false);
        musicManager.QuestionPanel.SetActive(true);
        musicManager.aud.Stop();
    }
}
