/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position : MonoBehaviour
{
    void Start()
    {
        // R�cup�re toutes les instances de Navigation8 dans la sc�ne
        Navigation8[] navigation8Instances = FindObjectsOfType<Navigation8>();

        // Affiche la valeur DistanceCheckpoint de chaque instance de Navigation8
        foreach (Navigation8 nav in navigation8Instances)
        {
            Debug.Log("DistanceCheckpoint de " + nav.name + " : " + nav.DistanceCheckpoint);
        }
    }

    void Update()
    {
        // � chaque frame, tu peux mettre � jour et obtenir la DistanceCheckpoint de toutes les instances de Navigation8
        Navigation8[] navigation8Instances = FindObjectsOfType<Navigation8>();

        foreach (Navigation8 nav in navigation8Instances)
        {
            // Fais quelque chose avec la DistanceCheckpoint ici, par exemple l'afficher continuellement
            Debug.Log("Mise � jour - DistanceCheckpoint de " + nav.name + " : " + nav.DistanceCheckpoint);
            if(nav.DistanceCheckpoint)
        }
    }
}
*/