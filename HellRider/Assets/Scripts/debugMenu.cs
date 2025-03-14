using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class debugMenu : MonoBehaviour
{
    public GameObject debugText;
    private bool debugOn = false;
    // Start is called before the first frame update
    void Start()
    {
        debugOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9))
        {
            debugOn = !debugOn;
            debugText.SetActive(debugOn);
        }
        if (debugOn)
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                SceneManager.LoadScene(0);
            } else if (Input.GetKeyDown(KeyCode.F2))
            {
                SceneManager.LoadScene(1);
            } else if (Input.GetKeyDown(KeyCode.F3))
            {
                SceneManager.LoadScene(2);
            } else if (Input.GetKeyDown(KeyCode.F4))
            {
                SceneManager.LoadScene(3);
            } else if (Input.GetKeyDown(KeyCode.F5))
            {
                SceneManager.LoadScene(4);
            } else if (Input.GetKeyDown(KeyCode.F6))
            {
                SceneManager.LoadScene(5);
            } else if (Input.GetKeyDown(KeyCode.F7))
            {
                SceneManager.LoadScene(6);
            } else if (Input.GetKeyDown(KeyCode.F8))
            {
                SceneManager.LoadScene(8);
            }
        }
    }
}
