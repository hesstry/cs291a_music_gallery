using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UploadScript : MonoBehaviour
{
    public MusicManager musicManager;
    public void EnterPath(string Path)
    {
        musicManager.MusicPath = Path;
        print("Music Path recorded: " + musicManager.MusicPath);
        musicManager.PlayMusic();
    }
}
