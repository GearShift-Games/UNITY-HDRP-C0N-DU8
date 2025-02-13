using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Navigation8 : MonoBehaviour, IPlayerScore
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
    public float score { get; set; }
    public GameObject Bike { get; set; }

    // The interface property can directly use the field.
    float IPlayerScore.score => score;
    GameObject IPlayerScore.Bike => Bike;


    public int Checkpointpassed;
    // Start is called before the first frame update
    void Start()
    {
        Bike = this.gameObject;

        agent = GetComponent<NavMeshAgent>();
        currentSpeed = normalSpeed;  // La vitesse initiale est normale
        agent.speed = currentSpeed;  // Applique la vitesse initiale au NavMeshAgent
        agent.destination = waypoints[currentWaypointIndex].position;  // Définit la première destination
    }

    // Update is called once per frame
    void Update()
    {
        DistanceCheckpoint = Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position);
        //Debug.Log(Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position));
        // Vérifie si l'ennemi est dans la zone d'activation autour du point de destination actuel

        var distance = Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position);
        //Debug.Log(distance);
        //Debug.Log(Vector3.Distance(waypoints[currentWaypointIndex].position, waypoints[currentWaypointIndex + 1].position));
        var betweenCheckpoint = ScaleValue(distance, Vector3.Distance(waypoints[currentWaypointIndex].position, waypoints[(currentWaypointIndex + 1) % waypoints.Length].position), 0, 0, 1);

        //Debug.Log(gameObject.name + " " + betweenCheckpoint + " " + Checkpointpassed);

        score = Checkpointpassed + betweenCheckpoint;
        //Debug.Log("score of " + gameObject.name + " : " + score);

        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < activationRadius)
        {
            // Si c'est le cas, passe au point suivant
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            Checkpointpassed++;
            //Debug.Log(gameObject.name + Checkpointpassed);
            

            agent.destination = waypoints[currentWaypointIndex].position;  // Met à jour la destination
        }

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

    public static float ScaleValue(float value, float inputMin, float inputMax, float outputMin, float outputMax)
    {
        return Mathf.Clamp(((value - inputMin) / (inputMax - inputMin) * (outputMax - outputMin) + outputMin), outputMin, outputMax);
    }//im gonna take the distance between checkpoint to 0 to 1 to make more sense

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
