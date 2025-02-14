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

        // Add all active players that have a component implementing IPlayerScore.
        foreach (GameObject player in players)
        {
            // Check if the player is not null and active in the scene.
            if (player != null && player.activeInHierarchy)
            {
                IPlayerScore playerScore = player.GetComponent<IPlayerScore>();
                if (playerScore != null)
                {
                    scores.Add(playerScore);
                }
            }
        }

        // Optionally, add the main player if it is active.
        if (MainPlayer != null && MainPlayer.activeInHierarchy)
        {
            IPlayerScore mainScore = MainPlayer.GetComponent<IPlayerScore>();
            if (mainScore != null)
            {
                scores.Add(mainScore);
            }
        }

        // Sort the list in descending order of score.
        scores.Sort((a, b) => b.score.CompareTo(a.score));

        // Now, scores[0] is the player in first place, scores[1] second, etc.
        for (int i = 0; i < scores.Count; i++)
        {
            //Debug.Log($"Place {i + 1}: {scores[i].Bike.name} with score {scores[i].score}");
            // Disable timer for all players
            Timer timer = scores[i].Bike.GetComponent<Timer>();
            if (timer != null)
            {
                timer.TimerOn = false;
                timer.position = i + 1;
                // Enable timer for the player in 5th place
                if (i + 1 == scores.Count)
                {
                    timer.TimerOn = true;
                }
            }
        }
    }
}
