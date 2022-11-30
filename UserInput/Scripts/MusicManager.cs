using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
//using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    // Music data
    public AudioSource aud = null;

    public AudioClip audioClip;

    public string MusicPath;

    public GameObject MusicPanel;
    public GameObject QuestionPanel;
    public GameObject DisplayPanel;

    public GameObject EnterPath;
    public GameObject ContinueButton;

    private void Start()
    {
        MusicPanel.SetActive(true);
        QuestionPanel.SetActive(false);
        DisplayPanel.SetActive(false);
    }

    public void PlayMusic()
    {
        aud = this.GetComponent<AudioSource>();
        StartCoroutine(LoadAudio());
    }

    private IEnumerator LoadAudio()
    {
        /*
        if (MusicPath.Length != 0)
        {
            // Path for Android
            string fullpath = "file://" + MusicPath;
            print("Full Path on Andriod: " + fullpath);

            WWW url = new WWW(fullpath);
            yield return url;
            aud.clip = url.GetAudioClip(false, true, AudioType.MPEG);
            aud.Play();
        */
            
            if (File.Exists(MusicPath))
            {
                using (var uwr = UnityWebRequestMultimedia.GetAudioClip("file://" + MusicPath, AudioType.MPEG))
                {
                    ((DownloadHandlerAudioClip)uwr.downloadHandler).streamAudio = true;
                    yield return uwr.SendWebRequest();
                    DownloadHandlerAudioClip dlHandler = (DownloadHandlerAudioClip)uwr.downloadHandler;
                    if (dlHandler.isDone)
                    {
                        aud.clip = dlHandler.audioClip;
                        if (aud.clip != null)
                        {
                            aud.clip = DownloadHandlerAudioClip.GetContent(uwr);
                            aud.Play();
                            Debug.Log("Playing song using Audio Source!");
                        }
                        else
                        {
                            Debug.Log("No valid AudioClip! ");
                        }
                    }
                    else
                    {
                        Debug.Log("dlHandler is not done! ");
                    }

                }
            }
            else
            {
                print("The music file does not exist! ");
            }
            
        }
    }
}
    
