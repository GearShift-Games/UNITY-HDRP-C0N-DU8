using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutoStep2 : MonoBehaviour
{
    // Animator for scene changes
    public Animator transition;

    // "Congratulation, you did it" animator
    public Animator checkmark;

    // To account both directions
    public int directionCount = 0;

    // The calibration that makes thing work
    private OscBicycle bigBike;
    public GameObject Osc;

    //Hey, jay here, your tutorial is great, or is it? You see, it was almost perfect until it was found out that we need to turn left
    public GameObject leftProgress;
    public GameObject shoulderCheckAnim;

    void Start()
    {
        bigBike = Osc.GetComponent<OscBicycle>();
        bigBike.Calibrator();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(healthPoints);
        if (other.tag == "TutoProgress")
        {
            directionCount++;
            other.gameObject.SetActive(false);
            if (directionCount == 1)
            {
                leftProgress.SetActive(true);
                shoulderCheckAnim.transform.rotation = Quaternion.identity;
            }
            else if (directionCount >= 2)
            {
                StartCoroutine(TutoStep2Over());
            }
        }
    }

    private IEnumerator TutoStep2Over()
    {
        checkmark.Play("TUTO_checkmark");
        yield return new WaitForSeconds(1.5f);
        // Sam wasnt here, the code works
        transition.Play("uiFadeOUT");
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("00c_tutorial_life");
        yield break;
    }
}
