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

    // Gets the position manager
    public GameObject posManage;

    private bool unlockPlayers = true;

    // If its the intro
    public bool isIntro = false;

    public bool isTuto = false;

    public AudioSource audioSource;
    public AudioClip raceStartSound;
    private bool checkedStartSound = false;

    private float AIMaxSpeed;
    void Start()
    {
        AIMaxSpeed = AI[0].GetComponent<Navigation8>().maxSpeed;
        //Debug.Log(AIMaxSpeed);
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
                //Debug.Log(countdownUI.text);
                if(checkedStartSound == true) 
                {
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
        if (isIntro == false)
        {
            // Activates / Desactivates inputs
            JoueurNav2 playerMovement = Player.GetComponent<JoueurNav2>();
            if (isTuto)
            {
                TutoTimer playerTutoTimer = Player.GetComponent<TutoTimer>();
                playerTutoTimer.enabled = status;
            } else
            {
                Timer playerTimer = Player.GetComponent<Timer>();
                playerTimer.enabled = status;
            }
            // NavMeshAgent PlayerAgent = Player.GetComponent<NavMeshAgent>();
            playerMovement.enabled = status;
            // PlayerAgent.enabled = status;
        }

        // Activates / Desactivates position Manager
        PositionManager placementsSwitch = posManage.GetComponent<PositionManager>();
        placementsSwitch.enabled = status;

        foreach (GameObject entity in AI)
        {
            Navigation8 AIMovement = entity.GetComponent<Navigation8>();
            if (isTuto)
            {
                TutoTimer aiTutoTimer = entity.GetComponent<TutoTimer>();
                aiTutoTimer.enabled = status;
            } else
            {
                Timer AITimer = entity.GetComponent<Timer>();
                AITimer.enabled = status;
            }
            // NavMeshAgent AIagent = entity.GetComponent<NavMeshAgent>();
            AIMovement.enabled = status;
            // AIagent.enabled = status;
            if (status)
            {
                AIMovement.maxSpeed = AIMaxSpeed;
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
        Debug.Log("You should only see this message once ever per race.");
        countdownUI.text = string.Format("PEDALEZ !");
        yield return new WaitForSeconds(2f);

        countdownUI.text = string.Format(" ");
        yield break;
    }

}
