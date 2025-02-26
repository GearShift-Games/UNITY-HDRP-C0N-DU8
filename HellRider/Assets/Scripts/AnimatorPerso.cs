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
        float pedalAnimSpeed = navigationJoueur.RealSpeed * 10f; // Ajustez ce multiplicateur selon vos besoins
        animator.SetFloat("PedalSpeed", pedalAnimSpeed);

        // Si vous souhaitez aussi conserver l'ancienne méthode basée sur speedUI, vous pouvez décommenter ce qui suit :
        // float speed = navigationJoueur.speedUI / 10f;
        // animator.SetFloat("Speed", speed);
    }
}
