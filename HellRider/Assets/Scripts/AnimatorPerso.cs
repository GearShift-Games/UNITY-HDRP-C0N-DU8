using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorPerso : MonoBehaviour
{
    public Animator animator; // L'Animator du joueur
    JoueurNav2 navigationJoueur;
    public GameObject player;

    void Start()
    {
        navigationJoueur = player.GetComponent<JoueurNav2>();
    }

    void Update()
    {
        // Utilise les données OSC pour ajuster la vitesse de l'animation de pédalage
        float PedalSpeed = navigationJoueur.RealSpeed * 10f; // Ajustez ce multiplicateur selon vos besoins
        animator.SetFloat("PedalSpeed", PedalSpeed);
    }
}
