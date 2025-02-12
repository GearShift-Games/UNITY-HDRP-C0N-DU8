using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Navigation8_Joueur_Gen2 : MonoBehaviour
{
    public Transform[] waypoints;  // Tableau des points de passage (circuit)
    private NavMeshAgent agent;
    private int currentWaypointIndex = 0;  // Index du point de passage actuel
    public float marginOfError = 1.0f;  // Marge d'erreur pour consid�rer que le waypoint est atteint
    public float normalSpeed = 3.5f;  // Vitesse normale
    public float slowedSpeed = 1.5f;  // Vitesse r�duite pour les virages serr�s
    public float extremeSlowedSpeed = 1.0f;  // Vitesse tr�s r�duite pour les virages extr�mes
    public float turnAngleThreshold = 45.0f;  // Seuil d'angle pour les virages serr�s
    public float extremeTurnAngleThreshold = 90.0f;  // Seuil d'angle pour les virages extr�mes
    public float activationRadius = 3.0f;  // Rayon d'activation autour du waypoint
    public float acceleration = 2.0f;  // Acc�l�ration
    public float maxSpeed = 6.0f;  // Vitesse maximale

    private float currentSpeed;  // Vitesse actuelle
    public float DistanceCheckpoint = 0f;
    public float score { get; set; }
    public GameObject Bike { get; set; }
    public int Checkpointpassed;

    // Nouveau param�tre pour la rotation contr�l�e par le joueur
    public float turnSpeed = 100f;  // Vitesse de rotation

    void Start()
    {
        Bike = this.gameObject;
        agent = GetComponent<NavMeshAgent>();

        // D�sactive la rotation automatique du NavMeshAgent
        agent.updateRotation = false;

        currentSpeed = normalSpeed;
        agent.speed = currentSpeed;
        agent.destination = waypoints[currentWaypointIndex].position;
    }

    void Update()
    {
        // CONTR�LE DE LA ROTATION PAR LE JOUEUR
        // Le joueur utilise les touches (A/D ou fl�ches) pour tourner.
        float horizontalInput = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up, horizontalInput * turnSpeed * Time.deltaTime);

        // GESTION DES WAYPOINTS ET DU SCORE
        float distanceToWaypoint = Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position);
        DistanceCheckpoint = distanceToWaypoint;

        // Calcul du ratio de progression entre le waypoint courant et le suivant pour le score
        float nextWaypointDistance = Vector3.Distance(waypoints[currentWaypointIndex].position, waypoints[(currentWaypointIndex + 1) % waypoints.Length].position);
        float betweenCheckpoint = ScaleValue(distanceToWaypoint, nextWaypointDistance, 0, 0, 1);
        score = Checkpointpassed + betweenCheckpoint;

        // Passe au waypoint suivant si on est dans la zone d'activation
        if (distanceToWaypoint < activationRadius)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            Checkpointpassed++;
            agent.destination = waypoints[currentWaypointIndex].position;
        }

        // CALCUL DE L'ANGLE POUR AJUSTER LA VITESSE
        // On calcule la direction vers le waypoint et l'angle entre cette direction et celle du joueur.
        Vector3 targetDirection = (waypoints[currentWaypointIndex].position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, targetDirection);

        // Ajustement de la vitesse en fonction de l'angle (si le joueur s'�carte de la trajectoire id�ale, il ralentit)
        if (angle > extremeTurnAngleThreshold)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, extremeSlowedSpeed, acceleration * Time.deltaTime);
        }
        else if (angle > turnAngleThreshold)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, slowedSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, normalSpeed, acceleration * Time.deltaTime);
        }

        // Limite la vitesse maximale
        currentSpeed = Mathf.Min(currentSpeed, maxSpeed);
        agent.speed = currentSpeed;

        // Optionnel : d�c�l�ration progressive quand le joueur approche trop pr�s du waypoint
        if (distanceToWaypoint < marginOfError)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, acceleration * Time.deltaTime);
        }

        // ASSIGNATION DE LA VITESSE DANS LA DIRECTION ACTUELLE DU JOUEUR
        // Le NavMeshAgent avance d�sormais dans la direction que le joueur a choisie.
        agent.velocity = transform.forward * currentSpeed;
    }

    // Fonction pour �chelle une valeur dans un intervalle donn� (utilis�e pour le calcul du score)
    public static float ScaleValue(float value, float inputMin, float inputMax, float outputMin, float outputMax)
    {
        return Mathf.Clamp(((value - inputMin) / (inputMax - inputMin) * (outputMax - outputMin) + outputMin), outputMin, outputMax);
    }

    // Permet de visualiser dans l'�diteur les zones d'activation autour des waypoints.
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
