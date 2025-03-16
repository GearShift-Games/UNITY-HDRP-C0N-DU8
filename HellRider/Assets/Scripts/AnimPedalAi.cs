using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimPedalAi : MonoBehaviour
{

    public Animator animator;
    Navigation8 navigationAi;
    public GameObject Ai;
    public float PedalModif = 10f;
    // Start is called before the first frame update
    void Start()
    {
        navigationAi = Ai.GetComponent<Navigation8>();
    }

    // Update is called once per frame
    void Update()
    {
        float PedalSpeed = navigationAi.currentSpeed * PedalModif; // Ajustez ce multiplicateur selon vos besoins
        animator.SetFloat("PedalSpeed", PedalSpeed);
    }
}
