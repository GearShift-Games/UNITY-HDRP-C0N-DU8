using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutoStep1 : MonoBehaviour
{
    // Animator for scene changes
    public Animator transition;

    // the thing that animates the thing that it should animate
    public Animator checkmark;

    // The thing that makes calibration work
    private OscBicycle bigBike;
    public GameObject Osc;

    void Start()
    {
        bigBike = Osc.GetComponent<OscBicycle>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(healthPoints);
        if (other.tag == "TutoProgress")
        {
            StartCoroutine(TutoStep1Over());
        }
    }

    private IEnumerator TutoStep1Over()
    {
        bigBike.Calibrator();
        checkmark.Play("TUTO_checkmark");
        yield return new WaitForSeconds(1.5f);
        // Mods, turn off the lights
        transition.Play("uiFadeOUT");
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("00b_tutorial_turn");
        yield break;
    }
}
