using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class JoueurNavOriginal : MonoBehaviour, IPlayerScore
{
    public Transform[] waypoints;
    private NavMeshAgent agent;
    private int currentWaypointIndex = 0;
    public float activationRadius = 3.0f;

    // Variables de vitesse
    public float maxSpeed = 6.0f;
    private float currentSpeed = 0f;
    public int speedUI;

    [Header("Osc Data")]
    public GameObject Osc;
    public float RealSpeed;
    public float XValue;

    public float DistanceCheckpoint = 0f;
    public int Checkpointpassed;

    public float score { get; set; }
    public GameObject Bike { get; set; }

    float IPlayerScore.score => score;
    GameObject IPlayerScore.Bike => Bike;

    [Header("Rotation Settings")]
    // Contr�le la fluidit� de l'interpolation en rotation
    public float rotationSmoothing = 5f;

    // Syst�me de ralentissement au mur
    public float wallDetectionDistance = 1.5f;
    public float speedReductionDuration = 5.0f;
    private float currentSpeedMultiplier = 1.0f;
    private bool isTouchingWall = false;
    private float timeTouchingWall = 0f;
    private float lastSpeedBeforeWall;

    [Header("Courbe d'acc�l�ration")]
    public float speedThreshold1 = 60f;
    public float speedThreshold2 = 80f;
    public float accelerationLow = 15f;
    public float accelerationMedium = 8f;
    public float accelerationHigh = 4f;

    //public float dampingVal = 1f;

    //public float rotationSpeed = 1.0f;


    public float lopSmoothingFactor = 5f; // Vitesse du lissage

    private Quaternion lopSmoothedRotation;


    public float Sacceleration = 2.0f; // Facteur d�acc�l�ration
    public float SmaxSpeed = 5.0f; // Vitesse maximale de rotation
    private float ScurretnSpeed;
    private Quaternion ScurrentVelocity = Quaternion.identity; // Vitesse angulaire actuelle


    [Header("Drift Settings")]
    public float Drift = 5f; // Facteur d'interpolation en mode drift

    void Start()
    {
        Bike = this.gameObject;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false; // On contr�le la rotation nous-m�mes
        agent.destination = waypoints[currentWaypointIndex].position;

        RealSpeed = Osc.GetComponent<OscBicycle>().Speed;
        XValue = Osc.GetComponent<OscBicycle>().X;
    }


    void Update()
    {
        RealSpeed = Osc.GetComponent<OscBicycle>().Speed;
        XValue = Osc.GetComponent<OscBicycle>().X;
        speedUI = Mathf.FloorToInt(currentSpeed);

        // Nouvelle gestion de la rotation : on utilise la v�locit� d�sir�e du NavMeshAgent
        Vector3 desiredVelocity = agent.desiredVelocity;
        // if (desiredVelocity.sqrMagnitude > 0.1f)
        //{
        Quaternion targetRotation = Quaternion.LookRotation(desiredVelocity);
        // Calcul d'un facteur d'interpolation exponentiel pour une transition liss�e et ind�pendante du framerate
        // float damping = dampingVal - Mathf.Exp(-rotationSmoothing * Time.deltaTime);
        // transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, damping);
        // var step = rotationSpeed * Time.deltaTime;
        // transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);
        // Filtrage passe-bas : on interpole entre la rotation actuelle et la cible
        //lopSmoothedRotation = Quaternion.Slerp(lopSmoothedRotation, targetRotation, Time.deltaTime * lopSmoothingFactor);

        // Appliquer la rotation liss�e
        //transform.rotation = lopSmoothedRotation;

        //transform.rotation = SmoothDampQuaternion(transform.rotation, targetRotation,  1f / Sacceleration, SmaxSpeed);

        // Calculer l'angle entre la rotation actuelle et la cible
        float angle = Quaternion.Angle(transform.rotation, targetRotation);

        // Calculer une vitesse dynamique en fonction de l'�cart
        float StargetSpeed = Mathf.Clamp(angle * Sacceleration, 0.1f, SmaxSpeed);

        // Lissage de la vitesse pour une transition fluide (interpolation exponentielle)
        ScurretnSpeed = Mathf.Lerp(ScurretnSpeed, StargetSpeed, Time.deltaTime * 5f);

        // Interpolation Slerp avec la vitesse ajust�e
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * ScurretnSpeed);

        

        // }

        // V�rification des collisions avec un mur
        /*
        bool touchingWall = CheckWallCollision();
        if (touchingWall)
        {
            if (!isTouchingWall)
            {
                Debug.Log("Mur");
                lastSpeedBeforeWall = currentSpeed;
                // R�duction imm�diate de 40 % de la vitesse
                currentSpeed *= 0.6f;
                currentSpeedMultiplier = 1.0f;
                isTouchingWall = true;
                timeTouchingWall = 0f;
            }
            // R�duction progressive des 40 % restants sur speedReductionDuration secondes
            timeTouchingWall += Time.deltaTime;
            float reductionFactor = Mathf.Lerp(1.0f, 0.5f, timeTouchingWall / speedReductionDuration);
            currentSpeedMultiplier = Mathf.Clamp(currentSpeedMultiplier * reductionFactor, 0.2f, 1.0f);
        }
        else
        {
            isTouchingWall = false;
        }
        */

        // Gestion de la vitesse avec une courbe d'acc�l�ration
        float verticalInput = Input.GetAxis("Vertical");
        float targetSpeed = verticalInput * maxSpeed;
        float currentAcc = GetAccelerationForSpeed(currentSpeed);
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, currentAcc * Time.deltaTime);

        // Calcul de la v�locit� cible en fonction de la direction avant (qui est d�sormais mieux orient�e)
        Vector3 targetVelocity = transform.forward * currentSpeed * currentSpeedMultiplier;

        // Application de la v�locit� avec interpolation en mode drift si la barre d'espace est enfonc�e
        if (Input.GetKey(KeyCode.Space))
        {
            agent.velocity = Vector3.Lerp(agent.velocity, targetVelocity, Drift * Time.deltaTime);
        }
        else
        {
            agent.velocity = targetVelocity;
        }

        // Gestion des waypoints pour le score
        float distanceToWaypoint = Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position);
        DistanceCheckpoint = distanceToWaypoint;
        float nextWaypointDistance = Vector3.Distance(
            waypoints[currentWaypointIndex].position,
            waypoints[(currentWaypointIndex + 1) % waypoints.Length].position);
        float betweenCheckpoint = ScaleValue(distanceToWaypoint, nextWaypointDistance, 0, 0, 1);
        score = Checkpointpassed + betweenCheckpoint;

        if (distanceToWaypoint < activationRadius)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            Checkpointpassed++;
            agent.destination = waypoints[currentWaypointIndex].position;
        }
    }

    // Renvoie le taux d'acc�l�ration en fonction de la vitesse actuelle
    private float GetAccelerationForSpeed(float speed)
    {
        if (speed < speedThreshold1)
            return accelerationLow;
        else if (speed < speedThreshold2)
            return accelerationMedium;
        else
            return accelerationHigh;
    }

    // D�tection de mur avec Raycast
    /*bool CheckWallCollision()
    {
        RaycastHit hit;
        Vector3 forward = transform.forward;
        return Physics.Raycast(transform.position, forward, out hit, wallDetectionDistance);
    }*/

    // Fonction utilitaire pour mettre � l'�chelle une valeur d'un intervalle vers un autre
    public static float ScaleValue(float value, float inputMin, float inputMax, float outputMin, float outputMax)
    {
        return Mathf.Clamp(((value - inputMin) / (inputMax - inputMin) * (outputMax - outputMin) + outputMin),
                           outputMin, outputMax);
    }

    // Visualisation des waypoints
    void OnDrawGizmos()
    {
        if (waypoints != null)
        {
            Gizmos.color = Color.green;
            foreach (var waypoint in waypoints)
            {
                Gizmos.DrawWireSphere(waypoint.position, activationRadius);
            }
        }
    }
}
