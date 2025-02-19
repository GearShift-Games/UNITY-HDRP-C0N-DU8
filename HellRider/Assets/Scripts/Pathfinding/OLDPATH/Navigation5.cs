using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Navigation5 : MonoBehaviour
{
    public Transform[] waypoints;  // Tableau des points de passage (circuit)
    private NavMeshAgent agent;
    private int currentWaypointIndex = 0;  // Index du point de passage actuel
    public float marginOfError = 1.0f;  // La marge d'erreur pour consid�rer que l'ennemi a atteint un point
    public float normalSpeed = 3.5f;  // Vitesse normale de l'ennemi
    public float slowedSpeed = 1.5f;  // Vitesse r�duite pour les virages serr�s
    public float turnAngleThreshold = 45.0f;  // Seuil d'angle pour consid�rer qu'il y a un virage serr�
    public float activationRadius = 3.0f;  // Rayon d'activation autour du point de passage

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = normalSpeed;  // D�finit la vitesse initiale de l'ennemi
        agent.destination = waypoints[currentWaypointIndex].position;  // D�finit la premi�re destination
    }

    // Update is called once per frame
    void Update()
    {
        // V�rifie si l'ennemi est dans la zone d'activation autour du point de destination actuel
        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < activationRadius)
        {
            // Si c'est le cas, passe au point suivant
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            agent.destination = waypoints[currentWaypointIndex].position;  // Met � jour la destination
        }

        // Calcule l'angle entre la direction actuelle et la direction du prochain waypoint
        Vector3 currentDirection = transform.forward;
        Vector3 targetDirection = (waypoints[currentWaypointIndex].position - transform.position).normalized;
        float angle = Vector3.Angle(currentDirection, targetDirection);

        // Si l'angle est sup�rieur au seuil, on ralentit
        if (angle > turnAngleThreshold)
        {
            agent.speed = slowedSpeed;  // Ralentit dans les virages serr�s
        }
        else
        {
            agent.speed = normalSpeed;  // Acc�l�re une fois le virage pass�
        }
    }

    // Cette fonction permet de dessiner une sph�re de d�tection autour des waypoints dans l'�diteur (pour tester)
    void OnDrawGizmos()
    {
        if (waypoints != null)
        {
            Gizmos.color = Color.green;  // Couleur verte pour la zone d'activation
            foreach (var waypoint in waypoints)
            {
                Gizmos.DrawWireSphere(waypoint.position, activationRadius);  // Affiche la zone d'activation autour de chaque waypoint
            }
        }
    }
}
