using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Navigation8: MonoBehaviour, IPlayerScore
{
    public Transform[] waypoints;  // Tableau des points de passage (circuit)
    private NavMeshAgent agent;
    private int currentWaypointIndex = 0;  // Index du waypoint actuel

    // Paramètre pour décaler la destination du waypoint (déviation aléatoire)
    public float marginOfError = 1.0f;  

    // Vitesse
    public float normalSpeed = 3.5f;            // Vitesse normale (sera modifiée toutes les 5 sec)
    public float slowedSpeed = 1.5f;            // Vitesse réduite pour les virages serrés
    public float extremeSlowedSpeed = 1.0f;     // Vitesse très réduite pour les virages extrêmes
    public float maxSpeed = 6.0f;               // Vitesse maximale que l'ennemi peut atteindre
    private float currentSpeed;               // Vitesse actuelle

    // Virage et activation
    public float turnAngleThreshold = 45.0f;  
    public float extremeTurnAngleThreshold = 90.0f;  
    public float activationRadius = 3.0f;  
    public float acceleration = 2.0f;  

    public float DistanceCheckpoint = 0f;
    public float score { get; set; }
    public GameObject Bike { get; set; }
    public int Checkpointpassed;

    // Propriétés de l'interface
    float IPlayerScore.score => score;
    GameObject IPlayerScore.Bike => Bike;

    public float baseTurnSpeed = 120f;  // Vitesse de rotation de base (en degrés/seconde)

    // Destination actuelle (avec déviation)
    private Vector3 currentDestination;

    void Start()
    {
        Bike = this.gameObject;
        agent = GetComponent<NavMeshAgent>();
        currentSpeed = normalSpeed;
        agent.speed = currentSpeed;

        // Calcule la destination initiale avec une déviation aléatoire
        currentDestination = GetWaypointDestination(currentWaypointIndex);
        agent.destination = currentDestination;

        // Désactive la rotation automatique pour la gérer manuellement
        agent.updateRotation = false;

        // Mise à jour de normalSpeed toutes les 3 secondes (random entre 35 et 50)
        InvokeRepeating("ChangeNormalSpeed", 3f, 3f);
    }

    void Update()
    {
        // Calcul de la distance jusqu'à la destination déviée
        DistanceCheckpoint = Vector3.Distance(transform.position, currentDestination);
        float distance = DistanceCheckpoint;
        float nextWaypointDistance = Vector3.Distance(waypoints[currentWaypointIndex].position,
            waypoints[(currentWaypointIndex + 1) % waypoints.Length].position);
        float betweenCheckpoint = ScaleValue(distance, nextWaypointDistance, 0, 0, 1);
        score = Checkpointpassed + betweenCheckpoint;

        // Passage au waypoint suivant si l'on est suffisamment proche de la destination déviée
        if (distance < activationRadius)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            Checkpointpassed++;
            currentDestination = GetWaypointDestination(currentWaypointIndex);
            agent.destination = currentDestination;
        }

        // --- Rotation ---
        // On utilise agent.steeringTarget pour obtenir un point plus loin sur le chemin
        Vector3 targetDirection = (agent.steeringTarget - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, targetDirection);

        // Ajustement de la vitesse selon l'angle (virage serré => décélération)
        if (angle > extremeTurnAngleThreshold)
            currentSpeed = Mathf.MoveTowards(currentSpeed, extremeSlowedSpeed, acceleration * Time.deltaTime);
        else if (angle > turnAngleThreshold)
            currentSpeed = Mathf.MoveTowards(currentSpeed, slowedSpeed, acceleration * Time.deltaTime);
        else
            currentSpeed = Mathf.MoveTowards(currentSpeed, normalSpeed, acceleration * Time.deltaTime);

        // Plafonnement de la vitesse
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

    // Méthode pour changer la destination d'un waypoint avec une déviation aléatoire dans le plan XZ
    private Vector3 GetWaypointDestination(int index)
    {
        Vector3 wp = waypoints[index].position;
        Vector2 randomOffset = Random.insideUnitCircle * marginOfError;
        return new Vector3(wp.x + randomOffset.x, wp.y, wp.z + randomOffset.y);
    }

    // Méthode appelée toutes les 5 secondes pour changer la normalSpeed
    void ChangeNormalSpeed()
    {
        normalSpeed = Random.Range(40f, 50f);
        //Debug.Log("Nouvelle normalSpeed: " + normalSpeed);
    }

    // Fonction utilitaire pour mettre à l'échelle une valeur d'un intervalle vers un autre
    public static float ScaleValue(float value, float inputMin, float inputMax, float outputMin, float outputMax)
    {
        return Mathf.Clamp(((value - inputMin) / (inputMax - inputMin) * (outputMax - outputMin) + outputMin), outputMin, outputMax);
    }

    // Visualisation dans l'éditeur de la zone d'activation et des destinations déviées
    void OnDrawGizmos()
    {
        if (waypoints != null)
        {
            foreach (var waypoint in waypoints)
            {
                // Zone d'activation
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(waypoint.position, activationRadius);

                // Visualisation de la destination déviée (approximative)
                Vector2 randomOffset = Random.insideUnitCircle * marginOfError;
                Vector3 randomDestination = waypoint.position + new Vector3(randomOffset.x, 0, randomOffset.y);
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(randomDestination, 0.5f);
            }
        }
    }
}
