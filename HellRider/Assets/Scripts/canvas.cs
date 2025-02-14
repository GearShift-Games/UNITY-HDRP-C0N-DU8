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
    public int placement;
    public int previousPlacement;

    // Start is called before the first frame update
    void Start()
    {
        TimerOn = true;
        placement = player.GetComponent<Timer>().position;
        previousPlacement = placement;
    }

    // Update is called once per frame
    void Update()
    {
        if (TimerOn)
        {
            LiveTimer += Time.deltaTime;
            updateTimer(LiveTimer);
            placement = player.GetComponent<Timer>().position;
            if (placement != previousPlacement)
            {
                if (placement == 1)
                {
                    placementUI.text = string.Format("{0:0}er", placement);
                }
                else
                {
                    placementUI.text = string.Format("{0:0}e", placement);
                }
            }
            previousPlacement = placement;
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
