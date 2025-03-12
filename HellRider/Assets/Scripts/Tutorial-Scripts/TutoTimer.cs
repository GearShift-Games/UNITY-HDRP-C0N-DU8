using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutoTimer : MonoBehaviour
{
    public float TimeLeft = 30f;
    public bool TimerOn = false;
    public float seconds;
    public GameObject playerUIPortrait;
    public GameObject playerUIFrame;

    private bool StillAlive = true;

    public bool isAI = true;

    public int position;

    private Animator animator;

    void Start()
    {
        //TimerOn = true; pls dont start the timer of everyone when the game start
        // >:) you can tell me what to do
        // yep i sure can =^D
        animator = playerUIFrame.GetComponent<Animator>();
        if (isAI)
        {
            animator.Play("UIpink", 0, 24.0f);
        } else
        {
            animator.Play("UIpink");
        }
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
            else if (StillAlive == true)
            {
                //Debug.Log("Time is UP!");
                TimeLeft = 0;
                TimerOn = false;
                playerUIFrame.SetActive(false);
                playerUIPortrait.GetComponent<Image>().color = new Color32(50, 50, 50, 80);
                StartCoroutine(DiesWithLoves());
            }
            //Debug.Log(this.gameObject + " " + TimeLeft);

            //play fuse animation
            if (isAI)
            {
                animator.speed = 2;
            } else
            {
                animator.speed = 1;
            }
        }
        else
        {
            //stop fuse animation
            //deactivate ui
            animator.speed = 0;
        }
    }

    void updateTimer(float currentTime)
    {
        currentTime += 1;

        seconds = Mathf.FloorToInt(currentTime % 60);
        //Debug.Log(this.gameObject + " " + seconds);

    }

    private IEnumerator DiesWithLoves()
    {
        StillAlive = false; // so it only gets started once
        Time.timeScale = 0.5f;
        yield return new WaitForSeconds(0.5f);
        this.gameObject.SetActive(false);
    }
}
