using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorPerso : MonoBehaviour
{
    public Animator animator; // L'Animator du joueur
    public float speed = 0f;  // La vitesse du joueur
    JoueurNav2 navigationJoueur;
    public GameObject player;

    //private Rigidbody2D rb; // Référence au Rigidbody2D du joueur

    void Start()
    {
        navigationJoueur = player.GetComponent<JoueurNav2>();
    }

    void Update()
    {

        var speed = navigationJoueur.speedUI / 10;
        // Calcul de la vitesse en fonction de la vitesse du Rigidbody
       // speed = rb.velocity.magnitude;

        // Met à jour le paramètre "Speed" dans l'Animator
        animator.SetFloat("Speed", speed);
    }
}


