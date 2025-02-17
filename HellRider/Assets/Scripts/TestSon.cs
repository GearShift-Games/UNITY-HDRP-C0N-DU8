using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class TestSon : MonoBehaviour
{
    public int position;
    public GameObject player;
    public bool TimerOn;
    public int placement;
    public int previousPlacement;

    public AudioSource audioSource;

    public AudioClip rankUpSound;  // Son quand le joueur monte
    public AudioClip rankDownSound; // Son quand le joueur descend
    public AudioClip TimerGoDown;
    public AudioClip lastPlaceSound;

    Timer timer;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("repeatingSound", 1f, 1f);

        timer = player.GetComponent<Timer>();


    }

    // Update is called once per frame
    void Update()
    {

        placement = timer.position;
        TimerOn = timer.TimerOn;
        if(placement == 5){
                    lastplace();
            }

        if (placement != previousPlacement)
        {
            if (placement > previousPlacement)
            {
                PlaySound(rankDownSound, 1f);
                // Debug.Log(placement.position);
                //Debug.Log("w" + previousPlacement);
                //Debug.Log("Descend");
            }
            else if (placement < previousPlacement)
            {
                PlaySound(rankUpSound, 1f);
                //Debug.Log(placement.position);
                //Debug.Log("w" + previousPlacement);
                //Debug.Log("Monte");
            }
        }
        previousPlacement = placement;
        
    }
    void PlaySound(AudioClip clip, float volume)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }
    void lastplace() {
            PlaySound(lastPlaceSound, 1f);
    }
    void repeatingSound()
    {
        if(TimerOn == true)
        {
            PlaySound(TimerGoDown, 0.75f);
        }
    }
}
