using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class JoueurNav2 : MonoBehaviour, IPlayerScore
{
    [Header("Waypoints and Directions")]
    public Transform[] waypoints;
    private NavMeshAgent agent;
    public Transform[] MainPath; // from start till path division
    public Transform[] InPath; // Side path 1 from division 1
    public Transform[] OutPath; // Side path 2 from division 1
    public Transform[] MainPath2; // if there's 2 side path, starts where the side path combine till division
    public Transform[] InPath2; // Side path 1 from division 2
    public Transform[] OutPath2; // Side path 2 from division 2
    public Transform[] CombinedPath; // the array where the AI store their full loop path
    public Transform[] EveryWaypoints; // put every waypoint here to render them

    private int currentWaypointIndex = 0;
    public float activationRadius = 3.0f;

    // Variables de vitesse
    public float maxSpeed = 6.0f;
    private float currentSpeed = 0f;
    public int speedUI;
    public bool isOscOn = false;
    [Header("Turbo")]
    public float TurboMult = 3f;
    public float TurboDure = 2f;
    [Header("Osc Data")]
    public GameObject Osc;
    public float RealSpeed;
    public float XValue;

    public float DistanceCheckpoint = 0f;
    public int Checkpointpassed;

    public float score { get; set; }
    public GameObject Bike { get; set; }

    float IPlayerScore.score => score;
    GameObject IPlayerScore.Bike => Bike;

    [Header("Rotation Settings")]
    public float rotationSmoothing = 5f;
    public float speedReductionDuration = 5.0f; // Durée pour récupérer le multiplicateur
    // Le multiplicateur sera appliqué à la vitesse
    private float currentSpeedMultiplier = 1.0f;

    [Header("Courbe d'accélération")]
    public float verticalInput = 0f;
    public float horizontalInput = 0f;
    public float speedThreshold1 = 60f;
    public float speedThreshold2 = 80f;
    public float accelerationLow = 15f;
    public float accelerationMedium = 8f;
    public float accelerationHigh = 4f;

    public float lopSmoothingFactor = 5f;
    private Quaternion lopSmoothedRotation;

    public float Sacceleration = 2.0f;
    public float SmaxSpeed = 5.0f;
    private float ScurretnSpeed;
    private Quaternion ScurrentVelocity = Quaternion.identity;

    public float PedaleMaxSpeed = 250f;

    // [Header("Drift Settings")]
    // La gestion du drift a été supprimée.

    [Header("NavMesh Edge Slowdown Settings")]
    [Tooltip("Distance à partir de laquelle le ralentissement commence (ex: 1 mètre)")]
    public float edgeSlowdownThreshold = 1f;
    [Tooltip("Multiplicateur appliqué quand le joueur est exactement sur la bordure (ex: 0.1 pour 10% de la vitesse normale)")]
    public float slowDownFactorAtEdge = 0.1f;


    //TEST ZONE
    public GameObject Pivot;
    //TEST AREA 
    // Reference to the AI's NavMeshAgent.

    // Maximum bank (tilt) angle in degrees.
    public float maxBankAngle = 15f;
    // Speed (degrees per second) at which the bank effect changes.
    public float bankSpeed = 1000f;

    // Internal state tracking the current bank (tilt) around the z axis.
    private float currentBank = 0f;
    // Variables pour le turbo
    private bool isTurboActive = false;

    void Start()
    {
        Bike = this.gameObject;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        ChoosePath(0);
        agent.destination = CombinedPath[currentWaypointIndex].position;

        RealSpeed = Osc.GetComponent<OscBicycle>().Speed;
        XValue = Osc.GetComponent<OscBicycle>().X;
    }

    void Update()
    {

        //TEST ZONE

        //TEST ZONE


        RealSpeed = Osc.GetComponent<OscBicycle>().Speed;
        XValue = Osc.GetComponent<OscBicycle>().X;
        speedUI = Mathf.FloorToInt(currentSpeed);

        // --- Gestion de la rotation ---
        Vector3 desiredVelocity = agent.desiredVelocity;
        if (desiredVelocity.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(desiredVelocity);
            float angle = Quaternion.Angle(transform.rotation, targetRotation);
            float StargetSpeed = Mathf.Clamp(angle * Sacceleration, 0.1f, SmaxSpeed);
            ScurretnSpeed = Mathf.Lerp(ScurretnSpeed, StargetSpeed, Time.deltaTime * 5f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * ScurretnSpeed);
        }




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


        if (currentSpeed > 500)
        {
            Pivot.transform.rotation = Quaternion.Euler(-45f, currentEuler.y, currentEuler.z);
        }
        else
        {
            Pivot.transform.rotation = Quaternion.Euler(0f, currentEuler.y, currentEuler.z);
        }


        // --- Gestion de l'accélération ---
        if (agent.isOnNavMesh)
        {
            horizontalInput = XValue + Input.GetAxis("Horizontal");
            float targetSpeed = Input.GetAxis("Vertical") * maxSpeed; // CLAVIER
            // currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, GetAccelerationForSpeed(currentSpeed) * Time.deltaTime);
            currentSpeed = RealSpeed * PedaleMaxSpeed;  // PEDALES
        }

        // --- Détection de la proximité de la bordure du NavMesh ---
        NavMeshHit edgeHit;
        if (NavMesh.FindClosestEdge(transform.position, out edgeHit, NavMesh.AllAreas))
        {
            float distanceToEdge = edgeHit.distance;
            if (distanceToEdge < edgeSlowdownThreshold)
            {
                float slowFactor = Mathf.Lerp(slowDownFactorAtEdge, 1f, distanceToEdge / edgeSlowdownThreshold);
                currentSpeed = Mathf.Max(currentSpeed * slowFactor, 25.0f);
            }
        }

        // --- Activation du turbo (prioritaire sur les autres commandes) ---
        if (Input.GetKeyDown(KeyCode.Space) && !isTurboActive)
        {
            StartCoroutine(ActivateTurbo());
        }

        // --- Application de la vitesse avec le multiplicateur (turbo inclus) ---
        Vector3 targetVelocity = transform.forward * currentSpeed * currentSpeedMultiplier;
        agent.velocity = targetVelocity;

        // --- Gestion des waypoints pour le score ---
        float distanceToWaypoint = Vector3.Distance(transform.position, CombinedPath[currentWaypointIndex].position);
        DistanceCheckpoint = distanceToWaypoint;
        float nextWaypointDistance = Vector3.Distance(
            CombinedPath[currentWaypointIndex].position,
            CombinedPath[(currentWaypointIndex + 1) % CombinedPath.Length].position);
        float betweenCheckpoint = ScaleValue(distanceToWaypoint, nextWaypointDistance, 0, 0, 1);
        score = Checkpointpassed + betweenCheckpoint;

        if (distanceToWaypoint < activationRadius)
        {
            if (CombinedPath[(currentWaypointIndex + 1) % CombinedPath.Length].gameObject.name == "Choice")
            {
                if (horizontalInput < 0)
                {
                    ChoosePath(1);
                }
                else if (horizontalInput > 0)
                {
                    ChoosePath(0);
                }
            }
            else if (CombinedPath[(currentWaypointIndex + 1) % CombinedPath.Length].gameObject.name == "Choice2")
            {
                // Traitement pour "Choice2" si nécessaire
            }
            currentWaypointIndex = (currentWaypointIndex + 1) % CombinedPath.Length;
            Checkpointpassed++;
            agent.destination = CombinedPath[currentWaypointIndex].position;
        }
    }

    private float GetAccelerationForSpeed(float speed)
    {
        if (speed < speedThreshold1)
            return accelerationLow;
        else if (speed < speedThreshold2)
            return accelerationMedium;
        else
            return accelerationHigh;
    }

    public static float ScaleValue(float value, float inputMin, float inputMax, float outputMin, float outputMax)
    {
        return Mathf.Clamp(((value - inputMin) / (inputMax - inputMin) * (outputMax - outputMin) + outputMin),
                           outputMin, outputMax);
    }

    void OnDrawGizmos()
    {
        if (EveryWaypoints != null)
        {
            Gizmos.color = Color.green;
            foreach (var waypoint in EveryWaypoints)
            {
                Gizmos.DrawWireSphere(waypoint.position, activationRadius);
            }
        }
    }

    void ChoosePath(int track)
    {
        if (track == 0)
        {
            CombinedPath = MainPath.Concat(InPath).Concat(MainPath2).Concat(InPath2).ToArray();
        }
        else if (track == 1)
        {
            CombinedPath = MainPath.Concat(OutPath).Concat(MainPath2).Concat(OutPath2).ToArray();
        }
        else if (track == 2)
        {
            CombinedPath = MainPath.Concat(InPath).Concat(MainPath2).Concat(OutPath2).ToArray();
        }
        else if (track == 3)
        {
            CombinedPath = MainPath.Concat(OutPath).Concat(MainPath2).Concat(InPath2).ToArray();
        }
    }

    private IEnumerator ActivateTurbo()
    {
        isTurboActive = true;
        currentSpeedMultiplier = TurboMult;  // Double la vitesse
        yield return new WaitForSeconds(TurboDure);
        currentSpeedMultiplier = 1f;  // Retour à la vitesse normale
        isTurboActive = false;
    }
}
