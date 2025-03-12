using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] BoomSounds; // Tableau pour stocker les clips audio

    private bool hasExploded = false;

    void OnDisable()
    {
        if (!hasExploded && audioSource != null && BoomSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, BoomSounds.Length); // Sélection aléatoire
            audioSource.PlayOneShot(BoomSounds[randomIndex]); // Joue le son
            hasExploded = true; // Empêche le son de se rejouer
        }
    }
}
