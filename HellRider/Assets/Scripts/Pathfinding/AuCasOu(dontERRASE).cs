/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class JoueurNav2 : MonoBehaviour, IPlayerScore
{
    public Transform[] waypoints;
    private NavMeshAgent agent;
    private int currentWaypointIndex = 0;
    public float activationRadius = 3.0f;

    // Variables de vitesse
    public float maxSpeed = 6.0f;
    private float currentSpeed = 0f;
    public int speedUI;
    public bool isOscOn = false;

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
    public float rotationSmoothing = 5f;
    public float speedReductionDuration = 5.0f; // Durée pour récupérer le multiplicateur
    // Le multiplicateur sera appliqué à la vitesse
    private float currentSpeedMultiplier = 1.0f;

    [Header("Courbe d'accélération")]
    public float verticalInput = 0f;
    public float speedThreshold1 = 60f;
    public float speedThreshold2 = 80f;
    public float accelerationLow = 15f;
    public float accelerationMedium = 8f;
    public float accelerationHigh = 4f;

    public float lopSmoothingFactor = 5f;
    private Quaternion lopSmoothedRotation;

    public float Sacceleration = 2.0f;
    public float SmaxSpeed = 5.0f;
    private float ScurretnSpeed;
    private Quaternion ScurrentVelocity = Quaternion.identity;

    [Header("Drift Settings")]
    public float Drift = 5f;

    [Header("NavMesh Edge Slowdown Settings")]
    [Tooltip("Distance à partir de laquelle le ralentissement commence (ex: 1 mètre)")]
    public float edgeSlowdownThreshold = 1f;
    [Tooltip("Multiplicateur appliqué quand le joueur est exactement sur la bordure (ex: 0.1 pour 10% de la vitesse normale)")]
    public float slowDownFactorAtEdge = 0.1f;

    void Start()
    {
        Bike = this.gameObject;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.destination = waypoints[currentWaypointIndex].position;

        RealSpeed = Osc.GetComponent<OscBicycle>().Speed;
        XValue = Osc.GetComponent<OscBicycle>().X;
    }

    void Update()
    {
        RealSpeed = Osc.GetComponent<OscBicycle>().Speed;
        XValue = Osc.GetComponent<OscBicycle>().X;
        speedUI = Mathf.FloorToInt(currentSpeed);

        // --- Gestion de la rotation ---
        Vector3 desiredVelocity = agent.desiredVelocity;
        if (desiredVelocity.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(desiredVelocity);
            float angle = Quaternion.Angle(transform.rotation, targetRotation);
            float StargetSpeed = Mathf.Clamp(angle * Sacceleration, 0.1f, SmaxSpeed);
            ScurretnSpeed = Mathf.Lerp(ScurretnSpeed, StargetSpeed, Time.deltaTime * 5f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * ScurretnSpeed);
        }

        // --- Gestion de l'accélération ---
        // On met à jour currentSpeed selon l'entrée quand l'agent est sur la piste
        if (agent.isOnNavMesh)
        {
            if (isOscOn)
            {
                verticalInput = RealSpeed;
            }
            else
            {
                verticalInput = Input.GetAxis("Vertical");
            }
            float targetSpeed = verticalInput * maxSpeed;
            float currentAcc = GetAccelerationForSpeed(currentSpeed);
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, currentAcc * Time.deltaTime);
        }
        // Sinon, on ne modifie pas currentSpeed pour conserver la dernière vitesse obtenue

        // --- Détection de la proximité de la bordure du NavMesh ---
        // --- Détection de la proximité de la bordure du NavMesh ---
        // --- Détection de la proximité de la bordure du NavMesh ---
        NavMeshHit edgeHit;
        if (NavMesh.FindClosestEdge(transform.position, out edgeHit, NavMesh.AllAreas))
        {
            float distanceToEdge = edgeHit.distance;
            if (distanceToEdge < edgeSlowdownThreshold)
            {
                // Appliquer une réduction de vitesse en une seule fois lorsqu'on touche le bord
                float slowFactor = Mathf.Lerp(slowDownFactorAtEdge, 1f, distanceToEdge / edgeSlowdownThreshold);

                // Réduction unique de la vitesse, mais sans descendre trop bas
                currentSpeed = Mathf.Max(currentSpeed * slowFactor, 25.0f); // 2.0f = vitesse minimale après ralentissement
            }
        }



        // --- Application de la vitesse ---
        Vector3 targetVelocity = transform.forward * currentSpeed * currentSpeedMultiplier;
        if (Input.GetKey(KeyCode.Space))
        {
            agent.velocity = Vector3.Lerp(agent.velocity, targetVelocity, Drift * Time.deltaTime);
        }
        else
        {
            agent.velocity = targetVelocity;
        }

        // --- Gestion des waypoints pour le score ---
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

    private float GetAccelerationForSpeed(float speed)
    {
        if (speed < speedThreshold1)
            return accelerationLow;
        else if (speed < speedThreshold2)
            return accelerationMedium;
        else
            return accelerationHigh;
    }

    public static float ScaleValue(float value, float inputMin, float inputMax, float outputMin, float outputMax)
    {
        return Mathf.Clamp(((value - inputMin) / (inputMax - inputMin) * (outputMax - outputMin) + outputMin),
                           outputMin, outputMax);
    }

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
*/