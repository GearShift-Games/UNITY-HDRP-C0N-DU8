using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Test : MonoBehaviour
{
    public Transform[] waypoints;  // Tableau des points de passage (circuit)
    private NavMeshAgent agent;
    private int currentWaypointIndex = 0;  // Index du point de passage actuel
    public float marginOfError = 1.0f;  // La marge d'erreur pour considérer que l'ennemi a atteint un point
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

    private int lapCount = 0;  // Compteur de tours
    private float totalDistance;  // Distance totale du circuit
    private float distanceTraveled;  // Distance parcourue par l'ennemi

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentSpeed = normalSpeed;  // La vitesse initiale est normale
        agent.speed = currentSpeed;  // Applique la vitesse initiale au NavMeshAgent
        agent.destination = waypoints[currentWaypointIndex].position;  // Définit la première destination

        // Calculer la distance totale du circuit
        totalDistance = 0f;
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            totalDistance += Vector3.Distance(waypoints[i].position, waypoints[i + 1].position);
        }
        // Ajouter la distance entre le dernier waypoint et le premier pour fermer le circuit
        totalDistance += Vector3.Distance(waypoints[waypoints.Length - 1].position, waypoints[0].position);

        // Debug : afficher la distance totale du circuit
        Debug.Log("Distance totale du circuit : " + totalDistance);
    }

    // Update is called once per frame
    void Update()
    {
        // Distance à l'actuel checkpoint
        DistanceCheckpoint = Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position);

        // Debug : afficher la distance à chaque checkpoint
        Debug.Log("Distance actuelle au checkpoint (" + currentWaypointIndex + ") : " + DistanceCheckpoint);

        // Vérifie si l'ennemi est dans la zone d'activation autour du point de destination actuel
        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < activationRadius)
        {
            // Si c'est le cas, passe au point suivant
            distanceTraveled += DistanceCheckpoint;  // Ajouter la distance parcourue depuis le dernier point

            // Debug : afficher la distance totale parcourue jusqu'à présent
            Debug.Log("Distance parcourue jusqu'à présent : " + distanceTraveled);

            if (currentWaypointIndex == waypoints.Length - 1)  // Si c'est le dernier checkpoint
            {
                // Vérifie si l'ennemi a parcouru toute la distance du circuit
                if (distanceTraveled >= totalDistance)
                {
                    // Si l'ennemi a parcouru la distance totale, c'est un tour complet
                    lapCount++;
                    // Debug : afficher le nombre de tours parcourus
                    Debug.Log("Tours parcourus : " + lapCount);
                    distanceTraveled = 0f;  // Réinitialiser la distance parcourue
                }

                currentWaypointIndex = 0;  // Recommence au premier point de passage
            }
            else
            {
                currentWaypointIndex++;  // Passe au prochain point de passage
            }

            agent.destination = waypoints[currentWaypointIndex].position;  // Met à jour la destination
        }

        // Appliquer un calcul de pathfinding personnalisé pour forcer l'ennemi à rester "au milieu" du chemin
        // Pour cela, on pourrait utiliser le centre entre chaque waypoint comme direction "idéale"
        Vector3 middlePoint = (waypoints[currentWaypointIndex].position + waypoints[(currentWaypointIndex + 1) % waypoints.Length].position) / 2f;
        Vector3 directionToMiddle = (middlePoint - transform.position).normalized;

        // Appliquer une direction "centrée" entre les waypoints
        agent.destination = transform.position + directionToMiddle * 5f; // Ajout d'un facteur de distance pour forcer l'ennemi à aller vers le centre

        // Calcule l'angle entre la direction actuelle et la direction du prochain waypoint
        Vector3 currentDirection = transform.forward;
        Vector3 targetDirection = (waypoints[currentWaypointIndex].position - transform.position).normalized;
        float angle = Vector3.Angle(currentDirection, targetDirection);

        // Logique de changement de vitesse selon l'angle
        if (angle > extremeTurnAngleThreshold)  // Si l'angle est très grand, virage extrême
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, extremeSlowedSpeed, acceleration * Time.deltaTime);  // Très forte décélération
        }
        else if (angle > turnAngleThreshold)  // Si l'angle est encore assez grand, virage serré
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, slowedSpeed, acceleration * Time.deltaTime);  // Décélération modérée
        }
        else  // Virage doux ou ligne droite
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
