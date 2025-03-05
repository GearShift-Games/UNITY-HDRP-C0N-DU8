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
}
