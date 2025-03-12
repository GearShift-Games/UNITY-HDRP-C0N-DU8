using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    void Awake()
    {
        // Disable VSync
        QualitySettings.vSyncCount = 0;
        // Set target frame rate to 60
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        onGoingRace = true;
    }
    void Update()
    {
        Time.timeScale = timespeed;

        if (!MainPlayer.activeInHierarchy && onGoingRace == true) //when you lose
        {
            //SceneManager.LoadScene("Circuit01_Maquette");
            PlaySound(LoseSound, 1f);
            onGoingRace = false;

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
            //SceneManager.LoadScene("Circuit01_Maquette");
            PlaySound(WinSound, 0.5f);
            onGoingRace = false;

            EndGameMenu.SetActive(true);
            Win.SetActive(true);
            StartCoroutine(Restart());
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
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
        yield break;
    }
}
