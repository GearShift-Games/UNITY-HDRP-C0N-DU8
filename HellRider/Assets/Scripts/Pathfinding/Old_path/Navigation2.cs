using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Navigation2 : MonoBehaviour
{
    public Transform[] waypoints;  // Tableau des points de passage (circuit)
    private NavMeshAgent agent;
    private int currentWaypointIndex = 0;  // Index du point de passage actuel
    public float marginOfError = 1.0f;  // La marge d'erreur pour considérer que l'ennemi a atteint un point
    public float speed = 3.5f;  // Vitesse de l'ennemi

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;  // Ajuste la vitesse de l'ennemi
        agent.destination = waypoints[currentWaypointIndex].position;  // Définit la première destination
    }

    // Update is called once per frame
    void Update()
    {
        // Vérifie si l'ennemi est proche du point de destination actuel
        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < marginOfError)
        {
            // Si c'est le cas, passe au point suivant
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            agent.destination = waypoints[currentWaypointIndex].position;  // Met à jour la destination
        }
    }
}
