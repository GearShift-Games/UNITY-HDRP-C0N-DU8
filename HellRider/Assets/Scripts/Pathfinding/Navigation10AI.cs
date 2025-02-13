using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Navigation10AI : MonoBehaviour, IPlayerScore
{
    public Transform[] waypoints;  // Tableau des points de passage (circuit)
    private NavMeshAgent agent;
    private int currentWaypointIndex = 0;  // Index du point de passage actuel
    public float marginOfError = 1.0f;  // Marge d'erreur pour considérer que l'ennemi a atteint un point
    public float normalSpeed = 3.5f;  // Vitesse normale de l'ennemi
    public float slowedSpeed = 1.5f;  // Vitesse réduite pour les virages serrés
    public float extremeSlowedSpeed = 1.0f;  // Vitesse très réduite pour les virages extrêmes
    public float turnAngleThreshold = 45.0f;  // Seuil d'angle pour les virages serrés
    public float extremeTurnAngleThreshold = 90.0f;  // Seuil d'angle pour les virages extrêmes
    public float activationRadius = 3.0f;  // Rayon d'activation autour du point de passage
    public float acceleration = 2.0f;  // Accélération de l'ennemi
    public float maxSpeed = 6.0f;  // Vitesse maximale que l'ennemi peut atteindre

    private float currentSpeed;  // Vitesse actuelle de l'ennemi
    public float DistanceCheckpoint = 0f;
    public float score { get; set; }
    public GameObject Bike { get; set; }

    // Propriétés de l'interface
    float IPlayerScore.score => score;
    GameObject IPlayerScore.Bike => Bike;

    public int Checkpointpassed;

    // Paramètre de base pour la rotation (en degrés par seconde)
    public float baseTurnSpeed = 120f;  

    void Start()
    {
        Bike = this.gameObject;
        agent = GetComponent<NavMeshAgent>();
        currentSpeed = normalSpeed;
        agent.speed = currentSpeed;
        agent.destination = waypoints[currentWaypointIndex].position;

        // On désactive la rotation automatique pour gérer manuellement
        agent.updateRotation = false;
    }

    void Update()
    {
        // Calcul de la distance au waypoint actuel
        DistanceCheckpoint = Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position);
        float distance = DistanceCheckpoint;
        float nextWaypointDistance = Vector3.Distance(waypoints[currentWaypointIndex].position,
            waypoints[(currentWaypointIndex + 1) % waypoints.Length].position);
        float betweenCheckpoint = ScaleValue(distance, nextWaypointDistance, 0, 0, 1);
        score = Checkpointpassed + betweenCheckpoint;

        // Passage au waypoint suivant si on est dans la zone d'activation
        if (distance < activationRadius)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            Checkpointpassed++;
            agent.destination = waypoints[currentWaypointIndex].position;
        }

        // --- Rotation ---
        // Au lieu d'utiliser directement le waypoint cible, on se base sur agent.steeringTarget
        // qui représente le point "intermédiaire" du chemin calculé.
        Vector3 targetDirection = (agent.steeringTarget - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, targetDirection);

        // Ajustement de la vitesse en fonction de l'angle (virage serré -> décélération)
        if (angle > extremeTurnAngleThreshold)
            currentSpeed = Mathf.MoveTowards(currentSpeed, extremeSlowedSpeed, acceleration * Time.deltaTime);
        else if (angle > turnAngleThreshold)
            currentSpeed = Mathf.MoveTowards(currentSpeed, slowedSpeed, acceleration * Time.deltaTime);
        else
            currentSpeed = Mathf.MoveTowards(currentSpeed, normalSpeed, acceleration * Time.deltaTime);

        currentSpeed = Mathf.Min(currentSpeed, maxSpeed);
        agent.speed = currentSpeed;

        // Calcul d'un facteur dynamique pour la rotation
        float speedFactor = currentSpeed / normalSpeed;
        if (speedFactor < 1f) speedFactor = 1f;
        float dynamicTurnSpeed = baseTurnSpeed * speedFactor;

        // Pour éviter de tourner trop tôt vers un waypoint (souvent proche d'un mur), 
        // on utilise agent.steeringTarget qui "regarde plus loin" sur le chemin.
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, dynamicTurnSpeed * Time.deltaTime);

        // Application de la vélocité dans la direction actuelle du transform
        agent.velocity = transform.forward * currentSpeed;
    }

    // Fonction utilitaire pour mettre à l'échelle une valeur d'un intervalle vers un autre
    public static float ScaleValue(float value, float inputMin, float inputMax, float outputMin, float outputMax)
    {
        return Mathf.Clamp(((value - inputMin) / (inputMax - inputMin) * (outputMax - outputMin) + outputMin), outputMin, outputMax);
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
