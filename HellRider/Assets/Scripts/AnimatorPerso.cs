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
        // Utilise les donn�es OSC pour ajuster la vitesse de l'animation de p�dalage
        float pedalAnimSpeed = navigationJoueur.RealSpeed * 10f; // Ajustez ce multiplicateur selon vos besoins
        animator.SetFloat("PedalSpeed", pedalAnimSpeed);

        // Si vous souhaitez aussi conserver l'ancienne m�thode bas�e sur speedUI, vous pouvez d�commenter ce qui suit :
        // float speed = navigationJoueur.speedUI / 10f;
        // animator.SetFloat("Speed", speed);
    }
}
