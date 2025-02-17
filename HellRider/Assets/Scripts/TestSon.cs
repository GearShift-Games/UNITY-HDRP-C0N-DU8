using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class TestSon : MonoBehaviour
{
    public int position; // doesnt seem to do anything

    public GameObject player; // player gameobject

    [Header("Boolean")]
    public bool TimerOn; // turn to true when the player is last place
    private bool hasPlayedLastPlaceSound = false;

    [Header("Player's Position")] //not sure why its public tho
    public int placement;
    public int previousPlacement;

    [Header("AudioSourses")]
    public AudioSource audioSource; // le audioSource
    public AudioSource AudioSourceBikeSound; // le audioSource 2

    [Header("Sounds")]
    public AudioClip rankUpSound;  // Son quand le joueur monte
    public AudioClip rankDownSound; // Son quand le joueur descend
    public AudioClip TimerGoDown; //son quand tu es en derniere place
    public AudioClip lastPlaceSound;
    public AudioClip bikeSound;

    [Header("Other Functions")]
    Timer timer;
    Navigation8JoueurGen3 navigationJoueur;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("repeatingSound", 1f, 1f);

        timer = player.GetComponent<Timer>();
        navigationJoueur = player.GetComponent<Navigation8JoueurGen3>();

        AudioSourceBikeSound.clip = bikeSound;
        AudioSourceBikeSound.loop = true;
    }

    // Update is called once per frame
    void Update()
    {
        var speed = navigationJoueur.speedUI;
        placement = timer.position;
        TimerOn = timer.TimerOn;


        if(placement == 5 && !hasPlayedLastPlaceSound){
            PlaySound(lastPlaceSound, 1f);
            hasPlayedLastPlaceSound = true; 
        } 
        else if (placement != 5)
        {
            //hasPlayedLastPlaceSound = false;  // Réinitialiser quand on n'est plus en dernière place
        } // lets make it so it only plays once or something similar - jay


        // for when your position changes
        if (placement != previousPlacement)
        {
            if (placement > previousPlacement)
            {
                PlaySound(rankDownSound, 1f);
                //Debug.Log("Descend");
            }
            else if (placement < previousPlacement)
            {
                PlaySound(rankUpSound, 1f);
                //Debug.Log("Monte");
            }
        }
        previousPlacement = placement;


        // for the bike wheel sound
        if (speed != 0)
        {
            if (!AudioSourceBikeSound.isPlaying)
            {
                AudioSourceBikeSound.Play();
            }
        }
        else
        {
            if (AudioSourceBikeSound.isPlaying)
            {
                AudioSourceBikeSound.Stop();
            }
        }

    }

    void PlaySound(AudioClip clip, float volume)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }

    void repeatingSound()
    {
        if(TimerOn == true && this.gameObject.activeInHierarchy)
        {
            PlaySound(TimerGoDown, 0.75f);
        }
    }
}
