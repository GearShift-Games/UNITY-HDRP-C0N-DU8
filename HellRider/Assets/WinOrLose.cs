using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinOrLose : MonoBehaviour
{

    public GameObject MainPlayer;
    public GameObject[] Participant;
    private int LeftAlive;

    void Update()
    {
        if (!MainPlayer.activeInHierarchy) //when you lose
        {
            //SceneManager.LoadScene("Circuit01_Maquette");
        }

        LeftAlive = 0;
        for (int i = 0; i < Participant.Length; i++)
        {
            if (Participant[i].activeInHierarchy)
            {
                LeftAlive++;
            }
        }

        if (LeftAlive == 1) //you win here 
        {
            SceneManager.LoadScene("Circuit01_Maquette");
        }
    }
}
