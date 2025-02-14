using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestSon : MonoBehaviour
{
    public int position;
    public GameObject player;
    public bool TimerOn = true;
    public int previousPlacement;
    public AudioSource audioSource;
    public AudioClip rankUpSound;  // Son quand le joueur monte
    public AudioClip rankDownSound; // Son quand le joueur descend
    // Start is called before the first frame update
    void Start()
    {
        TimerOn = true;
    }

    // Update is called once per frame
    void Update()
    {
                if (TimerOn)
        {
            Timer placement = player.GetComponent<Timer>();
            if (placement.position != previousPlacement)
            {
                if (placement.position > previousPlacement)
                {
                    PlaySound(rankDownSound);
                   // Debug.Log(placement.position);
                    //Debug.Log("w" + previousPlacement);
                    //Debug.Log("Descend");
                }
                else if (placement.position < previousPlacement)
                {
                PlaySound(rankUpSound);
                //Debug.Log(placement.position);
                //Debug.Log("w" + previousPlacement);
                //Debug.Log("Monte");
                }
            }
            previousPlacement = placement.position;
        }
    }
     void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
