using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutoStep3 : MonoBehaviour
{
    // The work that calibration thing makes
    public OscBicycle bigBike;

    void Start()
    {
        bigBike = GetComponent<OscBicycle>();
        bigBike.Calibrator();
    }
}
