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
    private float Boost; // right button
    private float Pause; // left button

    public float DistanceCheckpoint = 0f;
    public int Checkpointpassed;

    // Sets On or Off the OSC controls
    public bool isOscOn = false;
    public float verticalInput = 0f;
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

        // --- Contrôle de la rotation avec limitation ---
        float horizontalInput = Input.GetAxis("Horizontal"); // Input.GetAxis("Horizontal");XValue * 1.5f;
        //Debug.Log(XValue);

        // Détecte l'arête la plus proche sur le NavMesh
        NavMeshHit hit;
        if (NavMesh.FindClosestEdge(transform.position, out hit, NavMesh.AllAreas))
        {
            float dot = Vector3.Dot(hit.normal, transform.right);
            float minDistance = 1.0f; // Distance minimale pour autoriser la rotation vers le mur

            if (hit.distance < minDistance)
            {
                // Si le mur est sur la gauche du joueur (normal pointe vers la droite : dot positif)
                // et que l'input est négatif (tournant à gauche), on bloque cet input.
                if (dot > 0 && horizontalInput < 0)
                {
                    horizontalInput = 0;
                }
                // Si le mur est sur la droite (normal pointe vers la gauche : dot négatif)
                // et que l'input est positif (tournant à droite), on bloque cet input.
                else if (dot < 0 && horizontalInput > 0)
                {
                    horizontalInput = 0;
                }
            }
        }

        // Appliquer la rotation
        transform.Rotate(Vector3.up, horizontalInput * turnSpeed * Time.deltaTime);

        // --- Contrôle de la vitesse ---
        if (isOscOn)
        {
            verticalInput = RealSpeed;
        } else
        {
            verticalInput = Input.GetAxis("Vertical");
        }
        float targetSpeed = verticalInput * maxSpeed;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
        agent.velocity = transform.forward * currentSpeed;

        // --- Gestion des waypoints pour le score ---
        float distanceToWaypoint = Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position);
        DistanceCheckpoint = distanceToWaypoint;
        float nextWaypointDistance = Vector3.Distance(waypoints[currentWaypointIndex].position,
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