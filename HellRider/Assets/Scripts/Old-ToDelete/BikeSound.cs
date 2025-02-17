using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikeSound : MonoBehaviour
{
    public GameObject player;
    public AudioClip bikeSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.clip = bikeSound;
        audioSource.loop = true; 
    }

    void Update()
    {
        Navigation8JoueurGen3 speed = player.GetComponent<Navigation8JoueurGen3>();

        if (speed.speedUI != 0) 
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}
