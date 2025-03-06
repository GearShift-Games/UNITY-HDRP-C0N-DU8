using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUps : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (this.gameObject.tag == "AI")
        {
            Debug.Log("AI's will take over the world!");
        }
        else if (this.gameObject.tag == "Player")
        {
            Debug.Log("They'll never take us down! Humanity shall thrive!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ItemBox") && this.gameObject.CompareTag("Player"))
        {
            Debug.Log(this.gameObject.name + " boxed Player");
            StartCoroutine(ActivateTurbo());

        }
        else if (other.CompareTag("ItemBox") && this.gameObject.CompareTag("AI"))
        {
            Debug.Log(this.gameObject.name + " boxed AI");
        }
    }

    // we'll need to activate it here most likely, dunno how to do it for the ai yet tho
    private IEnumerator ActivateTurbo()
    {
        this.gameObject.GetComponent<JoueurNav2>().currentSpeedMultiplier = 2;
        Debug.Log(this.gameObject.name + " Boostah ON!");
        yield return new WaitForSeconds(3f);
        this.gameObject.GetComponent<JoueurNav2>().currentSpeedMultiplier = 1;
        Debug.Log(this.gameObject.name + " Boostah OFF!");
    }
}
