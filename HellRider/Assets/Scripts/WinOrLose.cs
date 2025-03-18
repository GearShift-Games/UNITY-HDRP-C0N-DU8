using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class WinOrLose : MonoBehaviour
{
    [Header("Win Condition")]
    private bool onGoingRace;
    public GameObject MainPlayer;
    public GameObject[] Participant;
    private int LeftAlive;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip LoseSound;  
    public AudioClip WinSound;  
    public AudioClip BoomSound;

    [Header("UI")]
    public GameObject Lose;
    public GameObject Win;
    public GameObject EndGameMenu;
    public TMP_Text RestartCountdown;

    public float timespeed = 1f;

    public Animator transition;

    Scene scene;
    private bool Lost;
    private bool Won;

    public GameObject Osc;

    void Awake()
    {
        scene = SceneManager.GetActiveScene();
        // Disable VSync
        QualitySettings.vSyncCount = 0;
        // Set target frame rate to 60
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        onGoingRace = true;

        if (scene.name == "Outro")
        {
            StartCoroutine(SetWinAfterDelay(10f));
        }
    }
    void Update()
    {
        Time.timeScale = timespeed;

        if (scene.name != "Outro")
        {
            if (!MainPlayer.activeInHierarchy && onGoingRace == true) //when you lose
            {
                PlaySound(LoseSound, 1f);
                onGoingRace = false;
                Lost = true;
                EndGameMenu.SetActive(true);
                Lose.SetActive(true);
                StartCoroutine(Restart());
            }


            LeftAlive = 0;
            for (int i = 0; i < Participant.Length; i++)
            {
                if (Participant[i].activeInHierarchy)
                {
                    LeftAlive++;
                }
            }

            if (LeftAlive == 1 && onGoingRace == true) //you win here 
            {
                Osc.GetComponent<OscBicycle>().Leaderboard();
                PlaySound(WinSound, 0.25f);
                onGoingRace = false;
                Won = true;
                EndGameMenu.SetActive(true);
                Win.SetActive(true);
                StartCoroutine(Restart());
            }
        }

    }


    void PlaySound(AudioClip clip, float volume)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }

    IEnumerator Restart()
    {

        for (int i = 5; i >= 0; i--)
        {
            RestartCountdown.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        transition.Play("uiFadeOUT");
        yield return new WaitForSeconds(2);

        if (Won == true)
        {
            if (scene.name == "00c_tutorial_life")
            {
                SceneManager.LoadScene("Circuit1");
            }
            else if (scene.name == "Circuit1")
            {
                SceneManager.LoadScene("Circuit3");
            }
            else if (scene.name == "Circuit3")
            {
                SceneManager.LoadScene("Circuit2");
            }
            else if (scene.name == "Circuit2")
            {
                SceneManager.LoadScene("Outro");
            }
            else if (scene.name == "Outro")
            {
                SceneManager.LoadScene(4);
            }
        }
        /*for (int i = 5; i >= 0; i--)
        {
            RestartCountdown.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
        transition.Play("uiFadeOUT");
        yield return new WaitForSeconds(2);



        if (Won == true)
        {
            if (scene.name == "00c_tutorial_life")
            {
                SceneManager.LoadScene("Circuit1");
            }
            else if (scene.name == "Circuit1")
            {
                SceneManager.LoadScene("Circuit2");
            }
            else if (scene.name == "Circuit2")
            {
                SceneManager.LoadScene("Circuit3");
            }
            else if (scene.name == "Circuit3")
            {
                SceneManager.LoadScene(1);
            }
        }*/
        else if (Lost == true)
        {
             if (scene.name == "Circuit1" || scene.name == "Circuit2" || scene.name == "Circuit3")
            {
                SceneManager.LoadScene("Circuit1");
            }
             else
            {
                SceneManager.LoadScene(scene.name);
            }
        }


        

        yield break;

    }
    IEnumerator SetWinAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Won = true;
        Debug.Log("Win = " + Won);
        StartCoroutine(Restart());
        yield break;
    }
}
