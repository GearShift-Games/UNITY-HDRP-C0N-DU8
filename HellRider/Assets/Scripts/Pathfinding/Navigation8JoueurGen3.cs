using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Navigation8JoueurGen3 : MonoBehaviour, IPlayerScore
{
    public Transform[] waypoints;  // Tableau des points de passage (circuit)
    private NavMeshAgent agent;
    private int currentWaypointIndex = 0;  // Index du waypoint actuel
    public float activationRadius = 3.0f;  // Rayon d'activation pour valider le passage d'un waypoint

    public float acceleration = 2.0f;  // Accélération du joueur
    public float maxSpeed = 6.0f;  // Vitesse maximale atteignable par le joueur
    private float currentSpeed = 0f;  // Vitesse actuelle
    public int speedUI; // Vitesse pour le UI

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

    // Paramètre de contrôle de la rotation par le joueur
    public float turnSpeed = 100f;  // Vitesse de rotation

    void Start()
    {
        Bike = this.gameObject;
        agent = GetComponent<NavMeshAgent>();

        // On désactive la rotation automatique du NavMeshAgent
        agent.updateRotation = false;

        // Initialise la destination pour le suivi des waypoints
        agent.destination = waypoints[currentWaypointIndex].position;

        RealSpeed = Osc.GetComponent<OscBicycle>().Speed;
        XValue = Osc.GetComponent<OscBicycle>().X;
    }

    void Update()
    {
        RealSpeed = Osc.GetComponent<OscBicycle>().Speed;
        XValue = Osc.GetComponent<OscBicycle>().X;

        // Prend la valeur du private currentSpeed pour l'envoyer au UI
        speedUI = Mathf.FloorToInt(currentSpeed);

        // --- Contrôle de la rotation ---
        // Le joueur tourne à gauche ou à droite via l'input horizontal (touches A/D ou flèches gauche/droite)
        float horizontalInput = XValue; // Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up, horizontalInput * turnSpeed * Time.deltaTime);

        // --- Contrôle de la vitesse ---
        // La vitesse est désormais contrôlée par l'input vertical (touches Z/S ou flèches haut/bas)
        // La valeur obtenue va de -1 (ralenti/frein ou marche arrière) à 1 (accélération)
        float verticalInput = RealSpeed; //Input.GetAxis("Vertical");
        float targetSpeed = verticalInput * maxSpeed;
        // On fait évoluer progressivement currentSpeed vers targetSpeed pour simuler l'accélération/décélération
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);

        // Mise à jour de la vélocité du NavMeshAgent en fonction de la direction actuelle du joueur
        agent.velocity = transform.forward * currentSpeed;

        // --- Gestion des waypoints pour le score ---
        float distanceToWaypoint = Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position);
        DistanceCheckpoint = distanceToWaypoint;

        // Calcul d'une progression (entre 0 et 1) entre le waypoint courant et le suivant
        float nextWaypointDistance = Vector3.Distance(waypoints[currentWaypointIndex].position, waypoints[(currentWaypointIndex + 1) % waypoints.Length].position);
        float betweenCheckpoint = ScaleValue(distanceToWaypoint, nextWaypointDistance, 0, 0, 1);
        score = Checkpointpassed + betweenCheckpoint;

        // Si le joueur est suffisamment proche du waypoint, on passe au suivant
        if (distanceToWaypoint < activationRadius)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            Checkpointpassed++;
            agent.destination = waypoints[currentWaypointIndex].position;
        }
    }

    // Fonction utilitaire pour mettre à l'échelle une valeur d'un intervalle vers un autre
    public static float ScaleValue(float value, float inputMin, float inputMax, float outputMin, float outputMax)
    {
        return Mathf.Clamp(((value - inputMin) / (inputMax - inputMin) * (outputMax - outputMin) + outputMin), outputMin, outputMax);
    }

    // Permet de visualiser dans l'éditeur la zone d'activation des waypoints
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
