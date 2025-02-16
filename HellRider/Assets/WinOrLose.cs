using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinOrLose : MonoBehaviour
{

    public GameObject MainPlayer;
    public GameObject[] Participant;
    private int LeftAlive;
    public AudioSource audioSource;
    public AudioClip LoseSound;  
    public AudioClip WinSound;  
    public AudioClip BoomSound;  
    void Update()
    {
        if (!MainPlayer.activeInHierarchy) //when you lose
        {
            //SceneManager.LoadScene("Circuit01_Maquette");
            PlaySound(LoseSound, 1f);
        }

        LeftAlive = 0;
        for (int i = 0; i < Participant.Length; i++)
        {
            if (Participant[i].activeInHierarchy)
            {
                LeftAlive++;
            }
        }

        if (LeftAlive == 1) //you win here 
        {
            //SceneManager.LoadScene("Circuit01_Maquette");
            PlaySound(WinSound, 1f);
        }


    }


    void PlaySound(AudioClip clip, float volume)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }
}
