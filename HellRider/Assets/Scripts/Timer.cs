using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float TimeLeft = 30f;
    public bool TimerOn = false;
    public float seconds;
    public GameObject playerUIPortrait;
    public GameObject playerUIFrame;
    public Animator sparkAnimationUI;
    public Animator sparksPositionUI;

    // For when they die
    public GameObject explosion;

    // UI when someone dies
    private Animator uiDies;

    private bool StillAlive = true;

    public bool isTuto = false;

    public int position;

    private Animator animator;

    void Start()
    {
        //TimerOn = true; pls dont start the timer of everyone when the game start
        // >:) you can tell me what to do
        // yep i sure can =^D
        animator = playerUIFrame.GetComponent<Animator>();
        if (isTuto)
        {
            animator.Play("UIpink", 0, 26.0f);
            sparksPositionUI.Play("sparkPosition", 0, 26.0f);
        }
        else
        {
            animator.Play("UIpink");
            sparksPositionUI.Play("sparkPosition");
        }
        sparkAnimationUI.Play("sparkling");
        uiDies = playerUIPortrait.GetComponent<Animator>();
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
                uiDies.Play("ui_dying");
                StartCoroutine(DiesWithLoves());
            }
            //Debug.Log(this.gameObject + " " + TimeLeft);
             animator.speed = 1;
             sparksPositionUI.speed = 1;
            //play fuse animation
        }
        else
        {
            //stop fuse animation
            //deactivate ui
            animator.speed = 0;
            sparksPositionUI.speed = 0;
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
        explosion.SetActive(true);
        //Time.timeScale = 0.5f;
        yield return new WaitForSeconds(0.1f);
        this.gameObject.SetActive(false);
        //Time.timeScale = 1f;
    }

    public void playingWithTime(float timeAdded)
    {
        TimeLeft += timeAdded;

        if (TimeLeft > 30f)
        {
            TimeLeft = 30f;
        }

        float mathTime = 30 - TimeLeft; // imma need alex's help for this thing

        animator.Play("UIpink", 0, mathTime);
        sparksPositionUI.Play("sparkPosition", 0, mathTime);

        /*
        0 = 30
        1 = 29
        2 = 28
        3 = 27
        4 = 26
        5 = 25
        6 = 24
        7 = 23
        8 = 22
        9 = 21 
        10 = 20
        11 = 19
        12 = 18
        13 = 17
        14 = 16
        15 = 15
        16 = 14
        17 = 13
        18 = 12
        19 = 11
        20 = 10
        21 = 9
        22 = 8
        23 = 7
        24 = 6
        25 = 5
        26 = 4
        27 = 3
        28 = 2
        29 = 1
         */
    }

}