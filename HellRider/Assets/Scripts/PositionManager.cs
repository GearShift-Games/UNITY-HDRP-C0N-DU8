using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PositionManager : MonoBehaviour
{
    // Assign your player objects in the Inspector.
    public GameObject[] players;
    public GameObject MainPlayer;

    void Update()
    {
        // Create a list of IPlayerScore objects.
        List<IPlayerScore> scores = new List<IPlayerScore>();

        // Add all players that have a component implementing IPlayerScore.
        foreach (GameObject player in players)
        {
            IPlayerScore playerScore = player.GetComponent<IPlayerScore>();
            if (playerScore != null)
            {
                scores.Add(playerScore);
            }
        }

        // Add the main player.
        IPlayerScore mainScore = MainPlayer.GetComponent<IPlayerScore>();
        if (mainScore != null)
        {
            scores.Add(mainScore);
        }

        // Sort the list in descending order of score.
        scores.Sort((a, b) => b.score.CompareTo(a.score));

        // Now, scores[0] is the player in first place, scores[1] second, etc.
        for (int i = 0; i < scores.Count; i++)
        {
            Debug.Log($"Place {i + 1}: {scores[i].Bike.name} with score {scores[i].score}");
            scores[i].Bike.GetComponent<Timer>().TimerOn = false;
            if (i + 1 == 5)
            {
                scores[i].Bike.GetComponent<Timer>().TimerOn = true;
            }
        }
    }
}
