using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioClip[] audios;
    public AudioSource au;

    public void ChangeAudio(int i)
    {
        au.clip = audios[i];
        au.Play();
    }

    public void CloseAudio()
    {
        au.Stop();
    }
}
