using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Navigation8: MonoBehaviour, IPlayerScore
{
    [Header("Waypoints and Directions")]
    /*public Transform[] waypoints;  // Tableau des points de passage (circuit)
    public Transform[] waypoints2;
    private Transform[] tempWaypointHolder;*/
    private NavMeshAgent agent;
    private int currentWaypointIndex = 0;  // Index du waypoint actuel
    float nextWaypointDistance;


    // Paramètre pour décaler la destination du waypoint (déviation aléatoire)
    public float marginOfError = 1.0f;

    // Vitesse
    [Header("Vitesse")]
    public float normalSpeed = 3.5f;            // Vitesse normale (sera modifiée toutes les 5 sec)
    public float slowedSpeed = 1.5f;            // Vitesse réduite pour les virages serrés
    public float extremeSlowedSpeed = 1.0f;     // Vitesse très réduite pour les virages extrêmes
    public float maxSpeed = 6.0f;               // Vitesse maximale que l'ennemi peut atteindre
    private float currentSpeed;               // Vitesse actuelle

    // Virage et activation
    [Header("Virage et activation")]
    public float turnAngleThreshold = 45.0f;  
    public float extremeTurnAngleThreshold = 90.0f;  
    public float activationRadius = 3.0f;  
    public float acceleration = 2.0f;

    [Header("Score and Checkpoints")]
    public float DistanceCheckpoint = 0f;
    public int Checkpointpassed;
    public float score { get; set; }
    public GameObject Bike { get; set; }

    public AudioSource BikeAudioSource;
    public AudioClip BikeSound;


    public int ChangeTrack = 0;

    // Propriétés de l'interface
    float IPlayerScore.score => score;
    GameObject IPlayerScore.Bike => Bike;

    //players position if we want some rubber banding
    //public GameObject Mainplayer;
    //private int MainplayerPosition;

    public float baseTurnSpeed = 120f;  // Vitesse de rotation de base (en degrés/seconde)

    // Destination actuelle (avec déviation)
    private Vector3 currentDestination;

    //TEST AREA 
    public Transform[] MainPath;
    public Transform[] InPath;
    public Transform[] OutPath;

    public Transform[] CombinedPath;
    public Transform[] EveryWaypoints;

    void Start()
    {
        Bike = this.gameObject;
        agent = GetComponent<NavMeshAgent>();
        currentSpeed = normalSpeed;
        agent.speed = currentSpeed;

        ChoosePath();

        // Calcule la destination initiale avec une déviation aléatoire
        currentDestination = GetWaypointDestination(CombinedPath,currentWaypointIndex);
        agent.destination = currentDestination;

        // Désactive la rotation automatique pour la gérer manuellement
        agent.updateRotation = false;

        // Mise à jour de normalSpeed toutes les 3 secondes (random entre 35 et 50)
        InvokeRepeating("ChangeNormalSpeed", 3f, 3f);

        BikeAudioSource.clip = BikeSound;
        BikeAudioSource.loop = true; 
    }

    void Update()
    {

        
        //MainplayerPosition = Mainplayer.GetComponent<Timer>().position;

        // Calcul de la distance jusqu'à la destination déviée
        DistanceCheckpoint = Vector3.Distance(transform.position, currentDestination);
        float distance = DistanceCheckpoint;

        /*
        if (waypoints[(currentWaypointIndex + 1) % waypoints.Length].gameObject.name == "Choice")
        {
            ChangeTrack = Random.Range(0, 2);
            //Debug.Log(ChangeTrack);
        }

        if (ChangeTrack == 0)
        {
            tempWaypointHolder = waypoints;
            nextWaypointDistance = Vector3.Distance(waypoints[currentWaypointIndex].position, waypoints[(currentWaypointIndex + 1) % waypoints.Length].position);
        }
        else if (ChangeTrack == 1)
        {
            tempWaypointHolder = waypoints2;
            nextWaypointDistance = Vector3.Distance(waypoints2[currentWaypointIndex].position, waypoints2[(currentWaypointIndex + 1) % waypoints2.Length].position);
        }*/

        nextWaypointDistance = Vector3.Distance(CombinedPath[currentWaypointIndex].position, CombinedPath[(currentWaypointIndex + 1) % CombinedPath.Length].position);

        float betweenCheckpoint = ScaleValue(distance, nextWaypointDistance, 0, 0, 1);
        score = Checkpointpassed + betweenCheckpoint;

        // Passage au waypoint suivant si l'on est suffisamment proche de la destination déviée
        if (distance < activationRadius)
        {
            if (CombinedPath[(currentWaypointIndex) % CombinedPath.Length].gameObject.name == "Start")
            {
                ChoosePath();
                Debug.Log("Turn done"); // TURN DONE HERE
            }

            currentWaypointIndex = (currentWaypointIndex + 1) % CombinedPath.Length;
            Checkpointpassed++;
            currentDestination = GetWaypointDestination(CombinedPath, currentWaypointIndex);
            agent.destination = currentDestination;
        }

        // --- Rotation ---
        // On utilise agent.steeringTarget pour obtenir un point plus loin sur le chemin
        Vector3 targetDirection = (agent.steeringTarget - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, targetDirection);

        // Ajustement de la vitesse selon l'angle (virage serré => décélération)
        if (angle > extremeTurnAngleThreshold)
            currentSpeed = Mathf.MoveTowards(currentSpeed, extremeSlowedSpeed, acceleration * Time.deltaTime);
        else if (angle > turnAngleThreshold)
            currentSpeed = Mathf.MoveTowards(currentSpeed, slowedSpeed, acceleration * Time.deltaTime);
        else
            currentSpeed = Mathf.MoveTowards(currentSpeed, normalSpeed, acceleration * Time.deltaTime);

        // Plafonnement de la vitesse
        currentSpeed = Mathf.Min(currentSpeed, maxSpeed);
        agent.speed = currentSpeed;

        // Calcul d'un facteur dynamique pour la rotation
        float speedFactor = currentSpeed / normalSpeed;
        if (speedFactor < 1f) speedFactor = 1f;
        float dynamicTurnSpeed = baseTurnSpeed * speedFactor;

        // Rotation manuelle vers la direction cible
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, dynamicTurnSpeed * Time.deltaTime);

        // Application de la vélocité dans la direction actuelle du transform
        agent.velocity = transform.forward * currentSpeed;

        // for the bike wheel sound
        if (currentSpeed != 0)
        {
            if (!BikeAudioSource.isPlaying)
            {
                BikeAudioSource.Play();
            }
        }
        else
        {
            if (BikeAudioSource.isPlaying)
            {
                BikeAudioSource.Stop();
            }
        }
    }

    // Méthode pour changer la destination d'un waypoint avec une déviation aléatoire dans le plan XZ
    private Vector3 GetWaypointDestination(Transform[] waypoints, int index)
    {
        Vector3 wp = waypoints[index].position;
        Vector2 randomOffset = Random.insideUnitCircle * marginOfError;
        return new Vector3(wp.x + randomOffset.x, wp.y, wp.z + randomOffset.y);
    }

    // Méthode appelée toutes les 5 secondes pour changer la normalSpeed
    void ChangeNormalSpeed()
    {
        normalSpeed = Random.Range(40f, 50f);
        //Debug.Log("Nouvelle normalSpeed: " + normalSpeed);
    }

    // Fonction utilitaire pour mettre à l'échelle une valeur d'un intervalle vers un autre
    public static float ScaleValue(float value, float inputMin, float inputMax, float outputMin, float outputMax)
    {
        return Mathf.Clamp(((value - inputMin) / (inputMax - inputMin) * (outputMax - outputMin) + outputMin), outputMin, outputMax);
    }

    // Visualisation dans l'éditeur de la zone d'activation et des destinations déviées
    void OnDrawGizmos()
    {
        /*if (waypoints != null)
        {
            foreach (var waypoint in waypoints)
            {
                // Zone d'activation
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(waypoint.position, activationRadius);

                // Visualisation de la destination déviée (approximative)
                Vector2 randomOffset = Random.insideUnitCircle * marginOfError;
                Vector3 randomDestination = waypoint.position + new Vector3(randomOffset.x, 0, randomOffset.y);
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(randomDestination, 0.5f);
            }
        }

        if (waypoints2 != null)
        {
            foreach (var waypoint in waypoints2)
            {
                // Zone d'activation
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(waypoint.position, activationRadius);

                // Visualisation de la destination déviée (approximative)
                Vector2 randomOffset = Random.insideUnitCircle * marginOfError;
                Vector3 randomDestination = waypoint.position + new Vector3(randomOffset.x, 0, randomOffset.y);
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(randomDestination, 0.5f);
            }
        }*/

        if (EveryWaypoints != null)
        {
            foreach (var waypoint in EveryWaypoints)
            {
                // Zone d'activation
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(waypoint.position, activationRadius);

                // Visualisation de la destination déviée (approximative)
                Vector2 randomOffset = Random.insideUnitCircle * marginOfError;
                Vector3 randomDestination = waypoint.position + new Vector3(randomOffset.x, 0, randomOffset.y);
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(randomDestination, 0.5f);
            }
        }
    }

    void ChoosePath()
    {
        ChangeTrack = Random.Range(0, 2);

        if (ChangeTrack == 0)
        {
            CombinedPath = MainPath.Concat(InPath).ToArray();
        }
        else if (ChangeTrack == 1)
        {
            CombinedPath = MainPath.Concat(OutPath).ToArray();
        }
    }
}




