using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float TimeLeft = 30f;
    public bool TimerOn = false;
    public float seconds;
    public GameObject playerUIPortrait;
    public GameObject playerUIFrame;

    public int position;

    void Start()
    {
        //TimerOn = true; pls dont start the timer of everyone when the game start
        // >:) you can tell me what to do
        // yep i sure can =^D
    }

    void Update()
    {
        //Debug.Log(this.gameObject + " " + TimerOn);
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
                playerUIFrame.SetActive(false);
                playerUIPortrait.GetComponent<Image>().color = new Color32(50, 50, 50, 80);
                this.gameObject.SetActive(false);
            }
            //Debug.Log(this.gameObject + " " + TimeLeft);

            //play fuse animation
        }
        else
        {
            //stop fuse animation
            //deactivate ui
        }
    }

    void updateTimer(float currentTime)
    {
        currentTime += 1;
        
        seconds = Mathf.FloorToInt(currentTime % 60);
        //Debug.Log(this.gameObject + " " + seconds);

    }

}