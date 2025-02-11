using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Test : MonoBehaviour
{
    public Transform[] waypoints;  // Tableau des points de passage (circuit)
    private NavMeshAgent agent;
    private int currentWaypointIndex = 0;  // Index du point de passage actuel
    public float marginOfError = 1.0f;  // La marge d'erreur pour consid�rer que l'ennemi a atteint un point
    public float normalSpeed = 3.5f;  // Vitesse normale de l'ennemi
    public float slowedSpeed = 1.5f;  // Vitesse r�duite pour les virages serr�s
    public float extremeSlowedSpeed = 1.0f;  // Vitesse tr�s r�duite pour les virages extr�mes
    public float turnAngleThreshold = 45.0f;  // Seuil d'angle pour les virages serr�s
    public float extremeTurnAngleThreshold = 90.0f;  // Seuil d'angle pour les virages extr�mes
    public float activationRadius = 3.0f;  // Rayon d'activation autour du point de passage
    public float acceleration = 2.0f;  // Acc�l�ration de l'ennemi
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
        agent.destination = waypoints[currentWaypointIndex].position;  // D�finit la premi�re destination

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
        // Distance � l'actuel checkpoint
        DistanceCheckpoint = Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position);

        // Debug : afficher la distance � chaque checkpoint
        Debug.Log("Distance actuelle au checkpoint (" + currentWaypointIndex + ") : " + DistanceCheckpoint);

        // V�rifie si l'ennemi est dans la zone d'activation autour du point de destination actuel
        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < activationRadius)
        {
            // Si c'est le cas, passe au point suivant
            distanceTraveled += DistanceCheckpoint;  // Ajouter la distance parcourue depuis le dernier point

            // Debug : afficher la distance totale parcourue jusqu'� pr�sent
            Debug.Log("Distance parcourue jusqu'� pr�sent : " + distanceTraveled);

            if (currentWaypointIndex == waypoints.Length - 1)  // Si c'est le dernier checkpoint
            {
                // V�rifie si l'ennemi a parcouru toute la distance du circuit
                if (distanceTraveled >= totalDistance)
                {
                    // Si l'ennemi a parcouru la distance totale, c'est un tour complet
                    lapCount++;
                    // Debug : afficher le nombre de tours parcourus
                    Debug.Log("Tours parcourus : " + lapCount);
                    distanceTraveled = 0f;  // R�initialiser la distance parcourue
                }

                currentWaypointIndex = 0;  // Recommence au premier point de passage
            }
            else
            {
                currentWaypointIndex++;  // Passe au prochain point de passage
            }

            agent.destination = waypoints[currentWaypointIndex].position;  // Met � jour la destination
        }

        // Appliquer un calcul de pathfinding personnalis� pour forcer l'ennemi � rester "au milieu" du chemin
        // Pour cela, on pourrait utiliser le centre entre chaque waypoint comme direction "id�ale"
        Vector3 middlePoint = (waypoints[currentWaypointIndex].position + waypoints[(currentWaypointIndex + 1) % waypoints.Length].position) / 2f;
        Vector3 directionToMiddle = (middlePoint - transform.position).normalized;

        // Appliquer une direction "centr�e" entre les waypoints
        agent.destination = transform.position + directionToMiddle * 5f; // Ajout d'un facteur de distance pour forcer l'ennemi � aller vers le centre

        // Calcule l'angle entre la direction actuelle et la direction du prochain waypoint
        Vector3 currentDirection = transform.forward;
        Vector3 targetDirection = (waypoints[currentWaypointIndex].position - transform.position).normalized;
        float angle = Vector3.Angle(currentDirection, targetDirection);

        // Logique de changement de vitesse selon l'angle
        if (angle > extremeTurnAngleThreshold)  // Si l'angle est tr�s grand, virage extr�me
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, extremeSlowedSpeed, acceleration * Time.deltaTime);  // Tr�s forte d�c�l�ration
        }
        else if (angle > turnAngleThreshold)  // Si l'angle est encore assez grand, virage serr�
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, slowedSpeed, acceleration * Time.deltaTime);  // D�c�l�ration mod�r�e
        }
        else  // Virage doux ou ligne droite
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, normalSpeed, acceleration * Time.deltaTime);  // Acc�l�re progressivement
        }

        // Applique la vitesse au NavMeshAgent, mais avec un plafond � la vitesse maximale
        currentSpeed = Mathf.Min(currentSpeed, maxSpeed);  // Ne d�passe pas la vitesse maximale
        agent.speed = currentSpeed;

        // Optionnel : pour appliquer une d�c�l�ration progressive quand l'ennemi s'approche de la destination
        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < marginOfError)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, acceleration * Time.deltaTime);  // Ralentit quand on approche du point de destination
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
