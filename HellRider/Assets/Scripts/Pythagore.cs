using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pythagore : MonoBehaviour
{
    public GameObject bike;
    public GameObject mass;


    void Start()
    {

    }

    void Update()
    {
        Vector3 currentEulerAngles = transform.rotation.eulerAngles;

        float angleInRadians = (currentEulerAngles.z -90 ) * Mathf.Deg2Rad;

        Debug.Log(Mathf.Cos(angleInRadians) * 0.5f);
        

        mass.transform.localPosition = new Vector3(Mathf.Cos(angleInRadians) * -0.5f, -0.5f, 0);
    }
}
