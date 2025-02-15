using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class countdown : MonoBehaviour
{
    // variables to do the countdown
    public int countdownTime = 2;
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
                getComponents(true);
                countingDown = false;
                countdownUI.text = string.Format("Pédalez !");
            }
            else
            {
                getComponents(false);
                countdownTime -= 1;
                countdownUI.text = string.Format("{0:0}", countdownTime+1);
            }
            //Debug.Log(this.gameObject + " " + TimeLeft);
        }
        else
        {
            //deactivate ui
        }
    }

    void getComponents(bool status)
    {
        // Activates / Deactivates inputs
        Navigation8JoueurGen3 playerMovement = Player.GetComponent<Navigation8JoueurGen3>();
        Timer playerTimer = Player.GetComponent<Timer>();
        playerMovement.enabled = status;
        playerTimer.enabled = status;

        foreach (GameObject entity in AI)
        {
            Navigation8 AIMovement = entity.GetComponent<Navigation8>();
            Timer AITimer = entity.GetComponent<Timer>();
            AIMovement.enabled = status;
            AITimer.enabled = status;
        }

    }
}
