using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class canvas : MonoBehaviour
{
    public bool TimerOn = true;
    public float Timer;
    public TextMeshProUGUI timerDisplay;
    // Start is called before the first frame update
    void Start()
    {
        TimerOn = true;
        timerDisplay = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime;
        updateTimer(Timer);
        timerDisplay.text = Timer.ToString();
    }

    void updateTimer(float currentTime)
    {
        currentTime += 1;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);
        float miliseconds = Mathf.FloorToInt(currentTime % 1000);

        timerDisplay.text = string.Format("Temps {0:00} : {0:00} : {0:000}", minutes, seconds, miliseconds);
    }
}
