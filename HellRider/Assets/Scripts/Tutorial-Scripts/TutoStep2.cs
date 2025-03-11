using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutoStep2 : MonoBehaviour
{
    // Animator for scene changes
    public Animator transition;

    // To account both directions
    public int directionCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(healthPoints);
        if (other.tag == "TutoProgress")
        {
            directionCount++;
            other.gameObject.SetActive(false);
            if (directionCount >= 2)
            {
                StartCoroutine(TutoStep2Over());
            }
        }
    }

    private IEnumerator TutoStep2Over()
    {
        // Death sequence here
        transition.Play("uiFadeOUT");
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("00c_tutorial_life");
        yield break;
    }
}
