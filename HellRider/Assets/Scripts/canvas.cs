using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class canvas : MonoBehaviour
{
    // Variables for the timer to work
    public bool TimerOn = true;
    public float LiveTimer;
    public TMP_Text timerDisplay;

    // UI Position
    public GameObject player;
    public TMP_Text placementUI;
    public int previousPlacement;

    // Speed-o-meter
    public TMP_Text speedometerUI;

    // doesnt start if the countdown is happening
    public countdown startScript;

    // Start is called before the first frame update
    void Start()
    {
        startScript = GetComponent<countdown>();
    }

    // Update is called once per frame
    void Update()
    {
        // Doesnt start the timer until the countdown is done
        if (startScript.countingDown)
        {
            TimerOn = false;
        } else
        {
            TimerOn = true;
        }

        // if the timer should be on, be on
        if (TimerOn)
        {
            LiveTimer += Time.deltaTime;
            updateTimer(LiveTimer);
            Timer placement = player.GetComponent<Timer>();
            if (placement.position != previousPlacement)
            {
                if (placement.position == 1)
                {
                    placementUI.text = string.Format("{0:0}er", placement.position);
                }
                else
                {
                    placementUI.text = string.Format("{0:0}e", placement.position);
                }
            }
            previousPlacement = placement.position;
            Navigation8JoueurGen3 speed = player.GetComponent<Navigation8JoueurGen3>();
            speedometerUI.text = string.Format("{0:0} km/h", speed.speedUI);
        }
    }

    void updateTimer(float currentTime)
    {
        currentTime += 1;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);
        float milliseconds = (currentTime * 1000) % 1000;

        timerDisplay.text = string.Format("Temps {0:00} : {1:00} : {2:000}", minutes, seconds, milliseconds);
    }
}
