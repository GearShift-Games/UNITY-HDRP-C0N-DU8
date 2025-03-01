using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Navigation8: MonoBehaviour, IPlayerScore
{
    [Header("Waypoints and Directions")]
    /*public Transform[] waypoints;  // Tableau des points de passage (circuit)
    public Transform[] waypoints2;
    private Transform[] tempWaypointHolder;*/
    public Transform[] MainPath; // from start till path division
    public Transform[] LeftPath; // Side path 1 from division 1
    public Transform[] RightPath; // Side path 2 from division 1
    public Transform[] MainPath2; // if there's 2 side path, starts where the side path combine till division
    public Transform[] LeftPath2; // Side path 1 from division 2
    public Transform[] RightPath2; // Side path 2 from division 2
    public Transform[] CombinedPath; //the array where the ai store their full loop path
    public Transform[] EveryWaypoints; //put every waypoint here to render them
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
    private float currentSpeed;
    public float RubberBanding = 1.2f;          // Max speed * rubberbanding
        // Vitesse actuelle

    // Virage et activation
    [Header("Virage et activation")]
    public float turnAngleThreshold = 45.0f;  
    public float extremeTurnAngleThreshold = 90.0f;  
    public float activationRadius = 3.0f;  
    public float acceleration = 2.0f;
    public Transform Pivot;

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
    public GameObject Mainplayer;

    public float baseTurnSpeed = 120f;  // Vitesse de rotation de base (en degrés/seconde)

    // Destination actuelle (avec déviation)
    private Vector3 currentDestination;

    //TEST AREA 
    // Reference to the AI's NavMeshAgent.

    // Maximum bank (tilt) angle in degrees.
    public float maxBankAngle = 15f;
    // Speed (degrees per second) at which the bank effect changes.
    public float bankSpeed = 1000f;

    // Internal state tracking the current bank (tilt) around the z axis.
    private float currentBank = 0f;

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
        int DistanceFromPlayer = Mainplayer.GetComponent<JoueurNav2>().Checkpointpassed - Checkpointpassed;
        // Calcul de la distance jusqu'à la destination déviée
        DistanceCheckpoint = Vector3.Distance(transform.position, currentDestination);
        float distance = DistanceCheckpoint;

        if (Mainplayer.GetComponent<Timer>().position == 1)
        {
            // EXTREME BUG RN, THE AI COMPLETELY STOP WHEN AT THE SAME CHECKPOINT OF THE PLAYER
            /*normalSpeed = 3.5f * (RubberBanding * DistanceFromPlayer);
            slowedSpeed = 1.5f * (RubberBanding * DistanceFromPlayer);
            extremeSlowedSpeed = 1.0f * (RubberBanding * DistanceFromPlayer);
            maxSpeed = 6.0f * (RubberBanding * DistanceFromPlayer);*/
            Debug.Log("that bastard! he's " + DistanceFromPlayer + " ahead of us!");
        }
        else
        {
            /*normalSpeed = 3.5f;
            slowedSpeed = 1.5f;
            extremeSlowedSpeed = 1.0f;
            maxSpeed = 6.0f;*/
        }

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










        //
        //      HANDLING THE ROTATION OF Z SO THEY TILT
        //
        Vector3 desiredDirection = agent.desiredVelocity;
        float targetBank = 0f;  // This will be our desired z rotation (bank).

        // Process only if the agent is moving significantly.
        if (desiredDirection.sqrMagnitude > 0.01f)
        {
            // Determine how much the AI is turning.
            float turnAngle = Vector3.SignedAngle(transform.forward, desiredDirection, Vector3.up);
            float turnThreshold = 1f; // Ignore tiny variations.

            if (Mathf.Abs(turnAngle) > turnThreshold)
            {
                // Invert the turn angle to get the desired bank:
                // For example, a positive turnAngle (turning right) results in a negative bank (tilt to the right).
                // Optionally, you could scale the effect if you want less aggressive tilting.
                targetBank = Mathf.Clamp(-turnAngle, -maxBankAngle, maxBankAngle);
            }
        }

        // Smoothly interpolate the current bank angle towards the target bank.
        currentBank = Mathf.MoveTowardsAngle(currentBank, targetBank, bankSpeed * Time.deltaTime);

        // Apply the bank rotation.
        // Preserve the current x and y rotations and update the z rotation to create the tilt effect.
        Vector3 currentEuler = Pivot.transform.eulerAngles;
        Pivot.transform.rotation = Quaternion.Euler(currentEuler.x, currentEuler.y, currentBank);









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
        ChangeTrack = Random.Range(0, 4);

        if (ChangeTrack == 0)
        {
            CombinedPath = MainPath.Concat(LeftPath).Concat(MainPath2).Concat(LeftPath2).ToArray();
        }
        else if (ChangeTrack == 1)
        {
            CombinedPath = MainPath.Concat(RightPath).Concat(MainPath2).Concat(RightPath2).ToArray();
        }
        else if (ChangeTrack == 2)
        {
            CombinedPath = MainPath.Concat(LeftPath).Concat(MainPath2).Concat(RightPath2).ToArray();
        }
        else if (ChangeTrack == 3)
        {
            CombinedPath = MainPath.Concat(RightPath).Concat(MainPath2).Concat(LeftPath2).ToArray();
        }
    }
}




