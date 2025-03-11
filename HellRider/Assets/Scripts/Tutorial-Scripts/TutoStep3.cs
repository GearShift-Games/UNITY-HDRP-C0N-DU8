using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutoStep3 : MonoBehaviour
{
    // Animator for scene changes
    public Animator transition;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(healthPoints);
        if (other.tag == "TutoProgress")
        {
            StartCoroutine(TutoStep3Over());
        }
    }

    private IEnumerator TutoStep3Over()
    {
        // Death sequence here
        transition.Play("uiFadeOUT");
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("Circuit1");
        yield break;
    }
}
