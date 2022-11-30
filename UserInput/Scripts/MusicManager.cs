using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
//using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Android;
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

    void Start()
    {
        if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        {
            Debug.Log("Storage Read permission has been granted.");
        }

        else
        {
            // We do not have permission to use the storage.
            // Ask for permission or proceed without the functionality enabled.
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
            print("Permission Asked.");
        }

        MusicPanel.SetActive(true);
        QuestionPanel.SetActive(false);
        DisplayPanel.SetActive(false);
        //AskPermission();
    }

    public void PlayMusic()
    {
        aud = this.GetComponent<AudioSource>();
        StartCoroutine(LoadAudio());
    }
    /*
    private void AskPermission()
    {
        print("Enter AskPermission Function!");
        if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        {
            Debug.Log("Storage Read permission has been granted.");
        }

        else
        {
            // We do not have permission to use the storage.
            // Ask for permission or proceed without the functionality enabled.
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
        }
    }
    */
    private IEnumerator LoadAudio()
    {
        if (File.Exists(MusicPath))
        {
            using (var uwr = UnityWebRequestMultimedia.GetAudioClip("file://" + MusicPath, AudioType.MPEG))
            {
                DownloadHandlerAudioClip dlHandler = (DownloadHandlerAudioClip)uwr.downloadHandler;
                dlHandler.streamAudio = true;
                yield return uwr.SendWebRequest();
                
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
    
