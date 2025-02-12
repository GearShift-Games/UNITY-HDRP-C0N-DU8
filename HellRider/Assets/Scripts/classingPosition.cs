using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class classingPosition : MonoBehaviour
{

    /*public GameObject[] enemyArr;

    public GameObject red;
    public GameObject blue;
    public GameObject yellow;
    public GameObject green;

    private float redScore;
    private float blueScore;
    private float yellowScore;
    private float greenScore;

    private float[] ScoreArr = new float[4];


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        redScore = red.GetComponent<Navigation8>().score;
        blueScore = blue.GetComponent<Navigation8>().score;
        yellowScore = yellow.GetComponent<Navigation8>().score;
        greenScore = green.GetComponent<Navigation8>().score;

        for (int i = 0; i < enemyArr.Length; i++)
        {
            ScoreArr[i] = enemyArr[i].GetComponent<Navigation8>().score;
            Array.Sort(ScoreArr);
            Array.Reverse(ScoreArr);
            //Debug.Log("this is number " + i + " of score array : " + ScoreArr[i]);
        }
    }*/

    // Assign your player objects in the Inspector.
    public GameObject[] players;

    void Update()
    {
        // Create a list of PlayerScore objects.
        List<Navigation8> scores = new List<Navigation8>();

        foreach (GameObject player in players)
        {
            Navigation8 nav = player.GetComponent<Navigation8>();
            if (nav != null)
            {
                scores.Add(nav);
            }
        }

        // Sort the list in descending order of score.
        scores.Sort((a, b) => b.score.CompareTo(a.score));

        // Now, scores[0] is the player in first place, scores[1] second, etc.
        for (int i = 0; i < scores.Count; i++)
        {
            Debug.Log($"Place {i + 1}: {scores[i].Bike.name} with score {scores[i].score}");
        }
    }
}
