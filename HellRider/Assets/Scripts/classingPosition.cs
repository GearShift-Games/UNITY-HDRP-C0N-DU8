using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ClassingPosition : MonoBehaviour
{

    // Assign your player objects in the Inspector.
    public GameObject[] players;
    public GameObject MainPlayer;

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
