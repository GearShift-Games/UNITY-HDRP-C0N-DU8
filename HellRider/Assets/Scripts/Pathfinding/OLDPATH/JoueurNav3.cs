using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class JoueurNav3 : MonoBehaviour, IPlayerScore
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
    // Les paramètres pour la rotation lissée
    public float Sacceleration = 2.0f; // Facteur d’accélération pour la rotation
    public float SmaxSpeed = 5.0f;     // Vitesse maximale de rotation
    private float ScurretnSpeed;

    [Header("Système de collision mur")]
    public float wallDetectionDistance = 1.5f;
    public float speedReductionDuration = 5.0f;
    private float currentSpeedMultiplier = 1.0f;
    private bool isTouchingWall = false;
    private float timeTouchingWall = 0f;
    private float lastSpeedBeforeWall;

    [Header("Courbe d'accélération")]
    public float speedThreshold1 = 60f;
    public float speedThreshold2 = 80f;
    public float accelerationLow = 15f;
    public float accelerationMedium = 8f;
    public float accelerationHigh = 4f;

    [Header("Drift Settings")]
    public float Drift = 5f; // Facteur d'interpolation en mode drift

    void Start()
    {
        Bike = this.gameObject;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false; // On contrôle la rotation nous-mêmes
        agent.destination = waypoints[currentWaypointIndex].position;

        RealSpeed = Osc.GetComponent<OscBicycle>().Speed;
        XValue = Osc.GetComponent<OscBicycle>().X;
    }

    void Update()
    {
        RealSpeed = Osc.GetComponent<OscBicycle>().Speed;
        XValue = Osc.GetComponent<OscBicycle>().X;
        speedUI = Mathf.FloorToInt(currentSpeed);

        // Calculer la direction cible directement vers le waypoint courant
        Vector3 targetDirection = (waypoints[currentWaypointIndex].position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        // Lissage de la rotation avec Slerp
        float angle = Quaternion.Angle(transform.rotation, targetRotation);
        float StargetSpeed = Mathf.Clamp(angle * Sacceleration, 0.1f, SmaxSpeed);
        ScurretnSpeed = Mathf.Lerp(ScurretnSpeed, StargetSpeed, Time.deltaTime * 5f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * ScurretnSpeed);

        // Vérification des collisions avec un mur
        bool touchingWall = CheckWallCollision();
        if (touchingWall)
        {
            if (!isTouchingWall)
            {
                lastSpeedBeforeWall = currentSpeed;
                // Réduction immédiate de 40 % de la vitesse
                currentSpeed *= 0.6f;
                currentSpeedMultiplier = 1.0f;
                isTouchingWall = true;
                timeTouchingWall = 0f;
            }
            // Réduction progressive des 40 % restants sur speedReductionDuration secondes
            timeTouchingWall += Time.deltaTime;
            float reductionFactor = Mathf.Lerp(1.0f, 0.5f, timeTouchingWall / speedReductionDuration);
            currentSpeedMultiplier = Mathf.Clamp(currentSpeedMultiplier * reductionFactor, 0.2f, 1.0f);
        }
        else
        {
            isTouchingWall = false;
        }

        // Gestion de la vitesse avec une courbe d'accélération
        float verticalInput = Input.GetAxis("Vertical");
        float targetSpeed = verticalInput * maxSpeed;
        float currentAcc = GetAccelerationForSpeed(currentSpeed);
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, currentAcc * Time.deltaTime);

        // Application de la vélocité en fonction de la rotation lissée
        Vector3 targetVelocity = transform.forward * currentSpeed * currentSpeedMultiplier;
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
        float nextWaypointDistance = Vector3.Distance(waypoints[currentWaypointIndex].position,
                                                      waypoints[(currentWaypointIndex + 1) % waypoints.Length].position);
        float betweenCheckpoint = ScaleValue(distanceToWaypoint, nextWaypointDistance, 0, 0, 1);
        score = Checkpointpassed + betweenCheckpoint;

        // Passage au waypoint suivant quand on est proche
        if (distanceToWaypoint < activationRadius)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            Checkpointpassed++;
            agent.destination = waypoints[currentWaypointIndex].position;
        }
    }

    // Renvoie le taux d'accélération en fonction de la vitesse actuelle
    private float GetAccelerationForSpeed(float speed)
    {
        if (speed < speedThreshold1)
            return accelerationLow;
        else if (speed < speedThreshold2)
            return accelerationMedium;
        else
            return accelerationHigh;
    }

    // Détection de mur avec Raycast
    bool CheckWallCollision()
    {
        RaycastHit hit;
        Vector3 forward = transform.forward;
        return Physics.Raycast(transform.position, forward, out hit, wallDetectionDistance);
    }

    // Fonction utilitaire pour mettre à l'échelle une valeur d'un intervalle vers un autre
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
