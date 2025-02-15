using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip BoomSound;
    private bool hasExploded = false;

    void OnDisable()
    {
        if (!hasExploded && audioSource != null && BoomSound != null)
        {
            audioSource.PlayOneShot(BoomSound);
            hasExploded = true; // Assure que le son ne sera joué qu'une seule fois
        }
    }
}