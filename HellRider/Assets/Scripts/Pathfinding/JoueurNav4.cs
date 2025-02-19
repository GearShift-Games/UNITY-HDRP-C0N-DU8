using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class JoueurNav4 : MonoBehaviour, IPlayerScore
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

    public float rotationSmoothing = 5f;

    // Système de ralentissement au mur
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
    public float driftFactor = 5f; // Facteur d'interpolation en mode drift

    [Header("Audio Settings")]
    public AudioSource bikeAudioSource;
    public AudioClip bikeSound;

    void Start()
    {
        Bike = this.gameObject;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.destination = waypoints[currentWaypointIndex].position;

        RealSpeed = Osc.GetComponent<OscBicycle>().Speed;
        XValue = Osc.GetComponent<OscBicycle>().X;

        // Configuration de l'audio
        bikeAudioSource.clip = bikeSound;
        bikeAudioSource.loop = true;
    }

    void Update()
    {
        RealSpeed = Osc.GetComponent<OscBicycle>().Speed;
        XValue = Osc.GetComponent<OscBicycle>().X;
        speedUI = Mathf.FloorToInt(currentSpeed);

        // Rotation fluide améliorée
        Vector3 targetDirection = (agent.steeringTarget - transform.position).normalized;
        if (targetDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothing * Time.deltaTime);
        }

        // Vérification des collisions avec un mur
        bool touchingWall = CheckWallCollision();
        if (touchingWall)
        {
            if (!isTouchingWall)
            {
                lastSpeedBeforeWall = currentSpeed;
                // Perte immédiate de 40% de la vitesse
                currentSpeed *= 0.6f;
                currentSpeedMultiplier = 1.0f;
                isTouchingWall = true;
                timeTouchingWall = 0f;
            }
            // Réduction progressive des 40% restants sur speedReductionDuration secondes
            timeTouchingWall += Time.deltaTime;
            float reductionFactor = Mathf.Lerp(1.0f, 0.5f, timeTouchingWall / speedReductionDuration);
            currentSpeedMultiplier = Mathf.Clamp(currentSpeedMultiplier * reductionFactor, 0.2f, 1.0f);
        }
        else
        {
            isTouchingWall = false;
        }

        // Gestion de la vitesse avec courbe d'accélération
        float verticalInput = Input.GetAxis("Vertical");
        float targetSpeed = verticalInput * maxSpeed;
        float currentAcc = GetAccelerationForSpeed(currentSpeed);
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, currentAcc * Time.deltaTime);

        // Calcul de la vélocité cible
        Vector3 targetVelocity = transform.forward * currentSpeed * currentSpeedMultiplier;

        // Utilisation conditionnelle de l'interpolation
        // Par défaut, on utilise l'assignation directe, sauf si la barre d'espace est pressée (mode drift)
        if (Input.GetKey(KeyCode.Space))
        {
            agent.velocity = Vector3.Lerp(agent.velocity, targetVelocity, driftFactor * Time.deltaTime);
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

        // Gestion du son du vélo
        if (currentSpeed != 0)
        {
            if (!bikeAudioSource.isPlaying)
            {
                bikeAudioSource.Play();
            }
        }
        else
        {
            if (bikeAudioSource.isPlaying)
            {
                bikeAudioSource.Stop();
            }
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
