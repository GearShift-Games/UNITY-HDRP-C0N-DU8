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
    public float Timer;
    public TMP_Text timerDisplay;

    // UI Position
    public TMP_Text placement;

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
            Timer += Time.deltaTime;
            updateTimer(Timer);
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
