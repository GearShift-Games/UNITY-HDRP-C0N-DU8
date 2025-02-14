using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    private float TimeLeft = 30f;
    public bool TimerOn = false;

    public int position;

    void Start()
    {
        //TimerOn = true; pls dont start the timer of everyone when the game start
        // >:) you can tell me what to do
    }

    void Update()
    {
        if (TimerOn)
        {
            if (TimeLeft > 0)
            {
                TimeLeft -= Time.deltaTime;
                updateTimer(TimeLeft);
            }
            else
            {
                //Debug.Log("Time is UP!");
                TimeLeft = 0;
                TimerOn = false;
                this.gameObject.SetActive(false);
            }
            //Debug.Log(this.gameObject + " " + TimeLeft);
        }
        else
        {
            //deactivate ui
        }
    }

    void updateTimer(float currentTime)
    {
        currentTime += 1;

        float seconds = Mathf.FloorToInt(currentTime % 60);
        Debug.Log(this.gameObject + " " + seconds);
    }

}