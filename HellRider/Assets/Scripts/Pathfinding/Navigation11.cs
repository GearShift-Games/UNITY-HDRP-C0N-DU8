using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Navigation11 : MonoBehaviour, IPlayerScore
{
    public Transform[] waypoints;  // Tableau des points de passage (circuit)
    private NavMeshAgent agent;
    private int currentWaypointIndex = 0;  // Index du point de passage actuel
    public float marginOfError = 1.0f;  // Marge d'erreur pour considérer que l'ennemi a atteint un point

    // Vitesse
    public float normalSpeed = 3.5f;  // Vitesse normale (sera modifiée toutes les 5 secondes)
    public float slowedSpeed = 1.5f;  // Vitesse réduite pour les virages serrés
    public float extremeSlowedSpeed = 1.0f;  // Vitesse très réduite pour les virages extrêmes
    public float maxSpeed = 6.0f;  // Vitesse maximale que l'ennemi peut atteindre
    private float currentSpeed;  // Vitesse actuelle de l'ennemi

    // Virage
    public float turnAngleThreshold = 45.0f;  // Seuil d'angle pour les virages serrés
    public float extremeTurnAngleThreshold = 90.0f;  // Seuil d'angle pour les virages extrêmes
    public float activationRadius = 3.0f;  // Rayon d'activation autour du point de passage
    public float acceleration = 2.0f;  // Accélération de l'ennemi

    public float DistanceCheckpoint = 0f;
    public float score { get; set; }
    public GameObject Bike { get; set; }
    public int Checkpointpassed;

    // Propriétés de l'interface
    float IPlayerScore.score => score;
    GameObject IPlayerScore.Bike => Bike;

    // Paramètre de base pour la rotation (en degrés par seconde)
    public float baseTurnSpeed = 120f;  

    void Start()
    {
        Bike = this.gameObject;
        agent = GetComponent<NavMeshAgent>();
        currentSpeed = normalSpeed;
        agent.speed = currentSpeed;
        agent.destination = waypoints[currentWaypointIndex].position;

        // On désactive la rotation automatique pour la gérer manuellement
        agent.updateRotation = false;

        // Appelle la méthode ChangeNormalSpeed toutes les 5 secondes
        InvokeRepeating("ChangeNormalSpeed", 3f, 3f);
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

        // Passage au waypoint suivant si dans la zone d'activation
        if (distance < activationRadius)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            Checkpointpassed++;
            agent.destination = waypoints[currentWaypointIndex].position;
        }

        // --- Rotation ---
        // Utilisation de agent.steeringTarget pour "regarder plus loin" sur le chemin
        Vector3 targetDirection = (agent.steeringTarget - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, targetDirection);

        // Ajustement de la vitesse en fonction de l'angle
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

        // Rotation manuelle vers la direction cible
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, dynamicTurnSpeed * Time.deltaTime);

        // Application de la vélocité dans la direction actuelle du transform
        agent.velocity = transform.forward * currentSpeed;
    }

    // Méthode appelée toutes les 5 secondes pour changer la vitesse normale de manière aléatoire
    void ChangeNormalSpeed()
    {
        normalSpeed = Random.Range(35f, 50f);
        // Vous pouvez également ajuster slowedSpeed et extremeSlowedSpeed si nécessaire,
        // par exemple en fonction de normalSpeed.
        Debug.Log("Nouvelle normalSpeed: " + normalSpeed);
    }

    // Fonction utilitaire pour mettre à l'échelle une valeur d'un intervalle vers un autre
    public static float ScaleValue(float value, float inputMin, float inputMax, float outputMin, float outputMax)
    {
        return Mathf.Clamp(((value - inputMin) / (inputMax - inputMin) * (outputMax - outputMin) + outputMin), outputMin, outputMax);
    }

    // Visualisation dans l'éditeur de la zone d'activation des waypoints
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
