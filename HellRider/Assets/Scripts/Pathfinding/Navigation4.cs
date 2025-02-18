using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Navigation4 : MonoBehaviour
{
    public Transform[] waypoints;  // Tableau des points de passage (circuit)
    private NavMeshAgent agent;
    private int currentWaypointIndex = 0;  // Index du point de passage actuel
    public float marginOfError = 3.0f;  // Marge d'erreur autour du chemin idéal
    public float normalSpeed = 3.5f;  // Vitesse normale de l'ennemi
    public float slowedSpeed = 1.5f;  // Vitesse réduite pour les virages serrés
    public float turnAngleThreshold = 45.0f;  // Seuil d'angle pour les virages serrés
    private Vector3 lastValidDestination;  // Dernière destination valide de l'agent

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = normalSpeed;  // Définit la vitesse initiale
        agent.destination = waypoints[currentWaypointIndex].position;  // Première destination
        lastValidDestination = waypoints[currentWaypointIndex].position;
    }

    // Update is called once per frame
    void Update()
    {
        // Vérifie si l'ennemi est proche du point de destination actuel
        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < marginOfError)
        {
            // Passe au prochain point de passage
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            agent.destination = waypoints[currentWaypointIndex].position;
            lastValidDestination = waypoints[currentWaypointIndex].position;
        }

        // Calcul de l'angle entre la direction actuelle et la direction du prochain waypoint
        Vector3 currentDirection = transform.forward;
        Vector3 targetDirection = (waypoints[currentWaypointIndex].position - transform.position).normalized;
        float angle = Vector3.Angle(currentDirection, targetDirection);

        // Ajuste la vitesse en fonction de l'angle du virage
        if (angle > turnAngleThreshold)
        {
            agent.speed = slowedSpeed;  // Ralentir si virage serré
        }
        else
        {
            agent.speed = normalSpeed;  // Vitesse normale dans les sections plus droites
        }

        // Appliquer une "marge d'erreur" pour permettre des trajectoires moins rigides
        // Si l'agent est suffisamment éloigné de la trajectoire idéale, ajuster la destination
        if (Vector3.Distance(transform.position, lastValidDestination) > marginOfError)
        {
            Vector3 directionToLast = (lastValidDestination - transform.position).normalized;
            agent.destination = transform.position + directionToLast * marginOfError;  // Déviation légère du chemin idéal
        }
    }
}
