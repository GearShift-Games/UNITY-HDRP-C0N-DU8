using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutoStep3 : MonoBehaviour
{
    // The work that calibration thing makes
    private OscBicycle bigBike;
    public GameObject Osc;

    void Start()
    {
        bigBike = Osc.GetComponent<OscBicycle>();
        bigBike.Calibrator();
    }
}
