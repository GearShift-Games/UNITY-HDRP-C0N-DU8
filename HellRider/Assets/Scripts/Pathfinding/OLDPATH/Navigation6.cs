using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Navigation6 : MonoBehaviour
{
    public Transform[] waypoints;  // Tableau des points de passage (circuit)
    private NavMeshAgent agent;
    private int currentWaypointIndex = 0;  // Index du point de passage actuel
    public float marginOfError = 1.0f;  // La marge d'erreur pour considérer que l'ennemi a atteint un point
    public float normalSpeed = 3.5f;  // Vitesse normale de l'ennemi
    public float slowedSpeed = 1.5f;  // Vitesse réduite pour les virages serrés
    public float turnAngleThreshold = 45.0f;  // Seuil d'angle pour considérer qu'il y a un virage serré
    public float activationRadius = 3.0f;  // Rayon d'activation autour du point de passage
    public float acceleration = 2.0f;  // Accélération de l'ennemi
    public float maxSpeed = 6.0f;  // Vitesse maximale que l'ennemi peut atteindre

    private float currentSpeed;  // Vitesse actuelle de l'ennemi

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentSpeed = normalSpeed;  // La vitesse initiale est normale
        agent.speed = currentSpeed;  // Applique la vitesse initiale au NavMeshAgent
        agent.destination = waypoints[currentWaypointIndex].position;  // Définit la première destination
    }

    // Update is called once per frame
    void Update()
    {
        // Vérifie si l'ennemi est dans la zone d'activation autour du point de destination actuel
        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < activationRadius)
        {
            // Si c'est le cas, passe au point suivant
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            agent.destination = waypoints[currentWaypointIndex].position;  // Met à jour la destination
        }

        // Calcule l'angle entre la direction actuelle et la direction du prochain waypoint
        Vector3 currentDirection = transform.forward;
        Vector3 targetDirection = (waypoints[currentWaypointIndex].position - transform.position).normalized;
        float angle = Vector3.Angle(currentDirection, targetDirection);

        // Si l'angle est supérieur au seuil, on ralentit dans les virages serrés
        if (angle > turnAngleThreshold)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, slowedSpeed, acceleration * Time.deltaTime);  // Décélère progressivement
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, normalSpeed, acceleration * Time.deltaTime);  // Accélère progressivement
        }

        // Applique la vitesse au NavMeshAgent, mais avec un plafond à la vitesse maximale
        currentSpeed = Mathf.Min(currentSpeed, maxSpeed);  // Ne dépasse pas la vitesse maximale
        agent.speed = currentSpeed;

        // Optionnel : pour appliquer une décélération progressive quand l'ennemi s'approche de la destination
        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < marginOfError)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, acceleration * Time.deltaTime);  // Ralentit quand on approche du point de destination
        }
    }

    // Cette fonction permet de dessiner une sphère de détection autour des waypoints dans l'éditeur (pour tester)
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
