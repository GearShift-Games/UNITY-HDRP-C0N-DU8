using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class countdown : MonoBehaviour
{
    // variables to do the countdown
    public float countdownTime = 2f;
    public bool countingDown;
    public TMP_Text countdownUI;

    // Gets the racers to set inactive their movement scripts
    public GameObject Player;
    public GameObject[] AI;

    private bool unlockPlayers = true;

    public AudioSource audioSource;
    public AudioClip raceStartSound;
    private bool checkedStartSound = false;
    void Start()
    {
        countingDown = true;
    }


    void PlaySound(AudioClip clip, float volume)
        {
            if (audioSource != null && clip != null)
            {
                audioSource.PlayOneShot(clip, volume);
            }
        }
    void Update()
    {
        if (countingDown)
        {
            if (countdownTime < 0f)
            {
                countingDown = false;
            }
            else
            {
                getComponents(false);
                countdownTime -= Time.deltaTime;
                countdownUI.text = string.Format("{0:0}", countdownTime+1f);
                Debug.Log(countdownUI.text);
                if(checkedStartSound == true) {
                PlaySound(raceStartSound, 0.5f);
                }           
            }
        }
        else if (unlockPlayers)
        {
            StartCoroutine("DisableCount");
        }
    }

    void getComponents(bool status)
    {
        // Activates / Desactivates inputs
        Navigation8JoueurGen3 playerMovement = Player.GetComponent<Navigation8JoueurGen3>();
        Timer playerTimer = Player.GetComponent<Timer>();
        // NavMeshAgent PlayerAgent = Player.GetComponent<NavMeshAgent>();
        playerMovement.enabled = status;
        playerTimer.enabled = status;
        // PlayerAgent.enabled = status;

        foreach (GameObject entity in AI)
        {
            Navigation8 AIMovement = entity.GetComponent<Navigation8>();
            Timer AITimer = entity.GetComponent<Timer>();
            // NavMeshAgent AIagent = entity.GetComponent<NavMeshAgent>();
            AIMovement.enabled = status;
            AITimer.enabled = status;
            // AIagent.enabled = status;
            if (status)
            {
                AIMovement.maxSpeed = 50f;
            } else
            {
                AIMovement.maxSpeed = 0f;
            }
        }

    }

    IEnumerator DisableCount()
    {
        unlockPlayers = false;
        getComponents(true);
        countdownUI.text = string.Format("PEDALEZ!");
        yield return new WaitForSeconds(2f);

        countdownUI.text = string.Format(" ");
        yield break;
    }

}
