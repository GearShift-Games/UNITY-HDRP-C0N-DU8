/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position : MonoBehaviour
{
    void Start()
    {
        // Récupère toutes les instances de Navigation8 dans la scène
        Navigation8[] navigation8Instances = FindObjectsOfType<Navigation8>();

        // Affiche la valeur DistanceCheckpoint de chaque instance de Navigation8
        foreach (Navigation8 nav in navigation8Instances)
        {
            Debug.Log("DistanceCheckpoint de " + nav.name + " : " + nav.DistanceCheckpoint);
        }
    }

    void Update()
    {
        // À chaque frame, tu peux mettre à jour et obtenir la DistanceCheckpoint de toutes les instances de Navigation8
        Navigation8[] navigation8Instances = FindObjectsOfType<Navigation8>();

        foreach (Navigation8 nav in navigation8Instances)
        {
            // Fais quelque chose avec la DistanceCheckpoint ici, par exemple l'afficher continuellement
            Debug.Log("Mise à jour - DistanceCheckpoint de " + nav.name + " : " + nav.DistanceCheckpoint);
            if(nav.DistanceCheckpoint)
        }
    }
}
*/