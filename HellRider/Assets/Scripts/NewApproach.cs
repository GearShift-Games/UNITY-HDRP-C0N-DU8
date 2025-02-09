using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewApproach : MonoBehaviour
{
    /*public GameObject bike;
    public Vector3 suspensionDistance;
    public Vector3 tireTransform = transform.position;
    public float springStrenght;
    public float springDamper;

    void Start()
    {
        Rigidbody bikeRB = bike.GetComponent<Rigidbody>();
    }

    void RayCheck()
    {
        Ray ray = new Ray(transform.position, transform.up);

        if (Physics.Raycast(ray, out hit))
        {
            // Sets the direction of the force to up (Y)
            Vector3 springDir = tireTransform.up;

            // Velocity of the tire
            Vector3 tireVelocity = bikeRB.GetPointVelocity(tireTransform.position);

            // Off-set of the tire calculated
            float offset = suspensionDistance - ray.distance;

            // Calculation of the velocity along the spring direction
            float vel = Vector3.Dot(springDir, tireVelocity);

            // Dampening of the tire
            float force = (offset * springStrenght) - (vel * springDamper);

            // force applied where this tire is
            bikeRB.AddForceAtPosition(springDir*force, tireTransform.position);
        }
    }*/
}
