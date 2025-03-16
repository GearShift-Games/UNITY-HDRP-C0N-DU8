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
    public AudioClip[] rankUpSound;
    public AudioClip[] rankDownSound; // Son quand le joueur monte
    public AudioClip TimerGoDown; //son quand tu es en derniere place
    public AudioClip lastPlaceSound;
    public AudioClip[] bikeSounds;

    [Header("Acceleration Sound Settings")]
    public AudioClip[] AccelerationSounds;

    // Variables pour suivre l'accélération
    private float lastSpeed = 0f;
    private float accelStartTime = 0f;
    private float initialSpeedForAccel = 0f;
    private bool isAccelerating = false;
    private bool hasPlayedAccelSound = false;
    private bool hasPlayedRapidAccelSound = false;


    [Header("Deceleration Sound Settings")]

    public AudioClip[] decelerationClip;          // Le clip à jouer pour la décélération

    private bool isDecelerating = false;
    private float decelStartTime = 0f;
    private float initialSpeedForDecel = 0f;
    private bool hasPlayedDecelSound = false;

    [Header("Other Functions")]
    Timer timer;
    JoueurNav2 navigationJoueur;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("repeatingSound", 1f, 1f);

        timer = player.GetComponent<Timer>();
        navigationJoueur = player.GetComponent<JoueurNav2>();

       
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
               // PlaySound(rankDownSound, 0.4f);
                int randomIndex = Random.Range(0, rankDownSound.Length); // Sélection aléatoire
                audioSource.PlayOneShot(rankDownSound[randomIndex], 1f); // Joue le son
                //Debug.Log("Descend");
            }
            else if (placement < previousPlacement)
            {
                //PlaySound(rankUpSound, 0.4f);
                int randomIndex = Random.Range(0, rankUpSound.Length); // Sélection aléatoire
                audioSource.PlayOneShot(rankUpSound[randomIndex],1f); // Joue le son
                //Debug.Log("Monte");
            }
        }
        previousPlacement = placement;


        // for the bike wheel sound
        // Pour le son de la roue du vélo avec une vitesse (pitch) proportionnelle à la vitesse du joueur
        if (speed > 0)
        {
            if (!AudioSourceBikeSound.isPlaying)
            {
                // Sélection aléatoire d'un son parmi les 10
                int randomIndex = Random.Range(0, bikeSounds.Length);
                AudioSourceBikeSound.clip = bikeSounds[randomIndex];
                AudioSourceBikeSound.loop = true;
                AudioSourceBikeSound.Play();
            }
            // Modification du pitch en fonction de la vitesse (si vous souhaitez garder cette fonctionnalité)
            AudioSourceBikeSound.pitch = Mathf.Clamp((speed / 500f) * 2.5f, 0f, 4f);
        }
        else
        {
            if (AudioSourceBikeSound.isPlaying)
            {
                AudioSourceBikeSound.Stop();
            }
        }


        // Bike sound original
        /*if (speed != 0)
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
        */
        //
        if (speed < 1f)
        {
            hasPlayedAccelSound = false;
            isAccelerating = false;
            hasPlayedRapidAccelSound = false;
        }

        // Déclenchement lors du passage de 0 à 40
        if (!hasPlayedAccelSound && lastSpeed < 1f && speed >= 40f)
        {
            int randomIndex = Random.Range(0, AccelerationSounds.Length); // Sélection aléatoire
            audioSource.PlayOneShot(AccelerationSounds[randomIndex], 1f);
            hasPlayedRapidAccelSound = true;
            hasPlayedAccelSound = true;
        }

        // Détection d'une accélération rapide : dès que le vélo commence à accélérer, démarrez le suivi
        if (!isAccelerating && speed > 0f)
        {
            isAccelerating = true;
            accelStartTime = Time.time;
            initialSpeedForAccel = speed;
        }
        else if (isAccelerating)
        {
            float elapsed = Time.time - accelStartTime;
            // Si en moins de 1.5 secondes, la vitesse augmente d'au moins 150
            if (elapsed <= 1.5f && !hasPlayedRapidAccelSound && speed - initialSpeedForAccel >= 150f)
            {
                int randomIndex = Random.Range(0, AccelerationSounds.Length); // Sélection aléatoire
                audioSource.PlayOneShot(AccelerationSounds[randomIndex], 1f);
                hasPlayedRapidAccelSound = true;
            }
            else if (elapsed > 1.5f)
            {
                // Réinitialisation après 1.5 secondes
                isAccelerating = false;
                hasPlayedRapidAccelSound = false;
            }
        }
        


        if (!isDecelerating && speed < lastSpeed)
        {
            isDecelerating = true;
            decelStartTime = Time.time;
            initialSpeedForDecel = lastSpeed;
        }
        else if (isDecelerating)
        {
            float decelElapsed = Time.time - decelStartTime;
            // Si en moins de 2 secondes, la vitesse chute d'au moins 50
            if (decelElapsed <= 2.0f && !hasPlayedDecelSound && (initialSpeedForDecel - speed >= 50f))
            {
                int randomIndex = Random.Range(0, decelerationClip.Length); // Sélection aléatoire
                audioSource.PlayOneShot(decelerationClip[randomIndex], 1f);
                hasPlayedDecelSound = true;
            }
            else if (decelElapsed > 2.0f)
            {
                isDecelerating = false;
                hasPlayedDecelSound = false;
            }
        }
        lastSpeed = speed;
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
