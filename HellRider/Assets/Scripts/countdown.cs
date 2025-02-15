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

    void Start()
    {
        countingDown = true;
    }

    void Update()
    {
        if (countingDown)
        {
            if (countdownTime < 0)
            {
                countingDown = false;
            }
            else
            {
                getComponents(false);
                countdownTime -= Time.deltaTime;
                countdownUI.text = string.Format("{0:0}", countdownTime+1f);
            }
        }
        else
        {
            getComponents(true);
            countdownUI.text = string.Format("Pédalez !");
            countdownTime = 2;
            countdownTime -= Time.deltaTime;
            if (countdownTime < 0)
            {
                countdownUI.text = string.Format(" ");
            }

        }
    }

    void getComponents(bool status)
    {
        // Activates / Deactivates inputs
        Navigation8JoueurGen3 playerMovement = Player.GetComponent<Navigation8JoueurGen3>();
        Timer playerTimer = Player.GetComponent<Timer>();
        NavMeshAgent PlayerAgent = Player.GetComponent<NavMeshAgent>();
        playerMovement.enabled = status;
        playerTimer.enabled = status;
        PlayerAgent.enabled = status;

        foreach (GameObject entity in AI)
        {
            Navigation8 AIMovement = entity.GetComponent<Navigation8>();
            Timer AITimer = entity.GetComponent<Timer>();
            NavMeshAgent AIagent = entity.GetComponent<NavMeshAgent>();
            AIMovement.enabled = status;
            AITimer.enabled = status;
            AIagent.enabled = status;
        }

    }
}
