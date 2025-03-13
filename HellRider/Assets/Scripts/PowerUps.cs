using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class PowerUps : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] TurboSound;
    public GameObject[] otherPlayers;
    public GameObject MagneticFPlayer;
    public GameObject MagneticFAi1;
    public GameObject MagneticFAi2;
    public GameObject MagneticFAi3;
    public GameObject MagneticFAi4;

    [Header("Position")]
    public int PlayersAlive;
    public int position;

    /* ARRAYS POG
     * 5 PLAYER
     *  1ST : LASER - CHAMP MAGNETIQUE - MAIN PLEINE - OBSTACLE                                             1
     *  2ND : TURBO - LASER - CHAMP MAGNETIQUE - OBSTACLE                                                   2
     *  3RD : TURBO - LASER - DE PORTAIL - CHAMP MAGNETIQUE                                                 3
     *  4TH : TURBO - LASER - DE PORTAIL - FUSIBLE NEUF                                                     4
     *  5TH : TURBO - LASER - DE PORTAIL - FUSIBLE NEUF - RECHARGEMENT                                      5
     *  
     * 4 PLAYER
     *  1ST : LASER - CHAMP MAGNETIQUE - MAIN PLEINE - OBSTACLE (SAME AS 5 PLAYER)                          1
     *  2ND : TURBO - LASER - CHAMP MAGNETIQUE - OBSTACLE       (SAME AS 5 PLAYER)                          2
     *  3RD : TURBO - LASER - DE PORTAIL - FUSIBLE NEUF         (SAME AS 5 PLAYER)                          4
     *  4TH : TURBO - LASER - DE PORTAIL - FUSIBLE NEUF - RECHARGEMENT (SAME AS 5TH PLACE OF 5 PLAYER)      5
     *  
     * 3 PLAYER
     *  1ST : LASER - CHAMP MAGNETIQUE - MAIN PLEINE - OBSTACLE (SAME AS 5 PLAYER)                          1
     *  2ND : TURBO - LASER - CHAMP MAGNETIQUE - OBSTACLE       (SAME AS 5 PLAYER)                          2
     *  3RD : TURBO - LASER - DE PORTAIL - FUSIBLE NEUF - RECHARGEMENT (SAME AS 5TH PLACE OF 5 PLAYER)      5
     *  
     * 2 PLAYER
     *  1ST : TURBO - CHAMP MAGNETIQUE - MAIN PLEINE                                                        6
     *  2ND : TURBO - LASER - CHAMP MAGNETIQUE - MAIN PLEINE                                                7
     *  
     *  
     * ARRAY NAMES
     *  1 : FIRSTPLACE
     *  2 : SECONDPLACE
     *  3 : THIRDPLACE
     *  4 : FOURTHPLACE
     *  5 : LASTPLACE
     *  
     *  6 : LASTTWOFIRST
     *  7 : LASTTWOLAST
     *  
     *  
     *  
     *  Power Up Description
     *  
     *  Turbo: Boost temporaire de la vitesse (2e a 5e place)
     *  TP: Téléporte avec un joueur aléatoire (2e a 5e place)**
     *  Recharge: Recharge de fusible 50% max(3e a 5e place et peut pas aller plus haut que 100%) 
     *  //Laser: Ralenti tous les joueurs touché
     *  Blue Chipmuncks: Le premier joueur freine**
     *  Consolation: Donne 2 objets aléatoires a la suite(derniere place uniquement)
     *  Shield: Ne peut être affecter par un malus, utiliser une seule fois**
     */

    string[] FIRSTPLACE = { "Laser", "MagneticField", "FullHand", "Obstacle"};
    string[] SECONDPLACE = {"Turbo", "Laser", "MagneticField", "Obstacle"};
    string[] THIRDPLACE = {"Turbo", "Laser", "DiePortal", "MagneticField"};
    string[] FOURTHPLACE = {"Turbo", "Laser", "DiePortal", "NewFuse"};
    string[] LASTPLACE = {"Turbo", "Laser", "DiePortal", "NewFuse", "Reload"};

    string[] LASTTWOFIRST = {"Turbo", "MagneticField", "FullHand"};
    string[] LASTTWOLAST = {"Turbo", "Laser", "MagneticField", "FullHand"};

    string[] TempArray;

    private string PowerGotten;

    JoueurNav2 Joueur;
    Navigation8 AI;

    // Start is called before the first frame update
    void Start()
    {
        if (this.gameObject.tag == "AI")
        {
            //Debug.Log("AI's will take over the world!");
            AI = this.gameObject.GetComponent<Navigation8>();
        }
        else if (this.gameObject.tag == "Player")
        {
            //Debug.Log("They'll never take us down! Humanity shall thrive!");
            Joueur = this.gameObject.GetComponent<JoueurNav2>();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ItemBox") && this.gameObject.CompareTag("Player"))
        {
            //Debug.Log(this.gameObject.name + " boxed Player");
            PowerChooser(position, PlayersAlive);
            //StartCoroutine("Turbo");
        }
        else if (other.CompareTag("ItemBox") && this.gameObject.CompareTag("AI"))
        {
            //Debug.Log(this.gameObject.name + " boxed AI");
            PowerChooser(position, PlayersAlive);
            //StartCoroutine("Turbo");
        }

        if (other.CompareTag("Boostpad") && this.gameObject.CompareTag("Player"))
        {
            StartCoroutine("Turbo");
        }
        else if (other.CompareTag("Boostpad") && this.gameObject.CompareTag("AI"))
        {
            StartCoroutine("Turbo");
        }
    }

    void PowerChooser(int Position, int playersAlive)
    {
        Debug.Log("position : " + Position + ", PlayersAlive : " + playersAlive);
        if (playersAlive == 2)
        {
            if (Position == 1)
            {
                TempArray = LASTTWOFIRST;
            }
            else
            {
                TempArray = LASTTWOLAST;
            }
        }
        else if (playersAlive == 3)
        {
            if (Position == 1)
            {
                TempArray = FIRSTPLACE;
            }
            else if (Position == 2)
            {
                TempArray = SECONDPLACE;
            }
            else
            {
                TempArray = LASTPLACE;
            }
        }
        else if (playersAlive == 4)
        {
            if (Position == 1)
            {
                TempArray = FIRSTPLACE;
            }
            else if (Position == 2)
            {
                TempArray = SECONDPLACE;
            }
            else if (Position == 3)
            {
                TempArray = FOURTHPLACE;
            }
            else
            {
                TempArray = LASTPLACE;
            }
        }
        else if (playersAlive == 5)
        {
            if (Position == 1)
            {
                TempArray = FIRSTPLACE;
            }
            else if (Position == 2)
            {
                TempArray = SECONDPLACE;
            }
            else if (Position == 3)
            {
                TempArray = THIRDPLACE;
            }
            else if (Position == 4)
            {
                TempArray = FOURTHPLACE;
            }
            else
            {
                TempArray = LASTPLACE;
            }
        }

        PowerGotten = TempArray[Random.Range(0, TempArray.Length)];
        StartCoroutine(PowerGotten);
    }

    // we'll need to activate it here most likely, dunno how to do it for the ai yet tho
    private IEnumerator Turbo()
    {
        if (this.gameObject.tag == "AI")
        {
            AI.currentSpeedMultiplier = 3;
        }
        else if (this.gameObject.tag == "Player")
        {
            Joueur.currentSpeedMultiplier = 3;
            // Rajouter son ici
            
            int randomIndex = Random.Range(0, TurboSound.Length); // Sélection aléatoire
            audioSource.PlayOneShot(TurboSound[randomIndex], 1f);

        }

        Debug.Log(this.gameObject.name + " Boostah ON!");
        yield return new WaitForSeconds(5f);

        if (this.gameObject.tag == "AI")
        {
            AI.currentSpeedMultiplier = 1;
        }
        else if (this.gameObject.tag == "Player")
        {
            Joueur.currentSpeedMultiplier = 1;
        }

        //Debug.Log(this.gameObject.name + " Boostah OFF!");
        yield break;
    }

    private IEnumerator Laser()
    {
        Debug.Log(this.gameObject.name + " Laser");
        yield break;
    }

    private IEnumerator DiePortal()
    {
        Debug.Log(this.gameObject.name + " DiePortal");
        yield break;
        ////////////////////////// Code tp de chatGpt /////////////////////////////////
        /*private IEnumerator DiePortal()
        {
            // Crée une liste des cibles éligibles (joueurs/IA mieux classés que nous)
            List<GameObject> eligibleTargets = new List<GameObject>();
            foreach (GameObject target in otherPlayers)
            {
                // On ne veut pas se comparer à soi-même
                if (target == gameObject) continue;

                PowerUps targetPU = target.GetComponent<PowerUps>();
                if (targetPU != null && targetPU.position < this.position)
                {
                    eligibleTargets.Add(target);
                }
            }

            if (eligibleTargets.Count == 0)
            {
                Debug.Log("Aucun joueur/IA éligible pour le téléportation.");
                yield break;
            }

            // Sélection aléatoire d'une cible parmi les éligibles
            int randomIndex = Random.Range(0, eligibleTargets.Count);
            GameObject targetToSwap = eligibleTargets[randomIndex];

            // Sauvegarde des positions actuelles
            Vector3 myPosition = transform.position;
            Vector3 targetPosition = targetToSwap.transform.position;

            // Échange des positions dans la scène
            transform.position = targetPosition;
            targetToSwap.transform.position = myPosition;

            // Sauvegarde des checkpoints passés pour chacun
            int myCheckpoints = 0;
            int targetCheckpoints = 0;

            // Pour le joueur
            if (gameObject.CompareTag("Player"))
            {
                JoueurNav2 myScript = GetComponent<JoueurNav2>();
                if (myScript != null)
                    myCheckpoints = myScript.checkpointsPassed;
            }
            else if (gameObject.CompareTag("AI"))
            {
                Navigation8 myScript = GetComponent<Navigation8>();
                if (myScript != null)
                    myCheckpoints = myScript.checkpointsPassed;
            }

            // Pour la cible sélectionnée
            if (targetToSwap.CompareTag("Player"))
            {
                JoueurNav2 targetScript = targetToSwap.GetComponent<JoueurNav2>();
                if (targetScript != null)
                    targetCheckpoints = targetScript.checkpointsPassed;
            }
            else if (targetToSwap.CompareTag("AI"))
            {
                Navigation8 targetScript = targetToSwap.GetComponent<Navigation8>();
                if (targetScript != null)
                    targetCheckpoints = targetScript.checkpointsPassed;
            }

            // Échange des checkpoints
            if (gameObject.CompareTag("Player"))
            {
                JoueurNav2 myScript = GetComponent<JoueurNav2>();
                if (myScript != null)
                    myScript.checkpointsPassed = targetCheckpoints;
            }
            else if (gameObject.CompareTag("AI"))
            {
                Navigation8 myScript = GetComponent<Navigation8>();
                if (myScript != null)
                    myScript.checkpointsPassed = targetCheckpoints;
            }

            if (targetToSwap.CompareTag("Player"))
            {
                JoueurNav2 targetScript = targetToSwap.GetComponent<JoueurNav2>();
                if (targetScript != null)
                    targetScript.checkpointsPassed = myCheckpoints;
            }
            else if (targetToSwap.CompareTag("AI"))
            {
                Navigation8 targetScript = targetToSwap.GetComponent<Navigation8>();
                if (targetScript != null)
                    targetScript.checkpointsPassed = myCheckpoints;
            }

            // (Optionnel) Échange du classement en inversant la variable 'position'
            int tempPos = this.position;
            this.position = targetToSwap.GetComponent<PowerUps>().position;
            targetToSwap.GetComponent<PowerUps>().position = tempPos;

            Debug.Log("Téléportation effectuée entre " + gameObject.name + " et " + targetToSwap.name);

            yield break;
        }
        */
    }

    private IEnumerator NewFuse()
    {
        Debug.Log(this.gameObject.name + " NewFuse");
        yield break;
    }

    private IEnumerator MagneticField()
    {
        Debug.Log(this.gameObject.name + " MagneticField");
        /////////// ChatGpt a cook sa pour le Magnetic field, je l'ai pas essayé c surtout pour donné une idée
        /*private IEnumerator MagneticField()
        {
            // Choix du GameObject à activer selon que ce soit le joueur ou l'IA
            GameObject magneticFieldObj = null;
            if (gameObject.CompareTag("Player"))
            {
                magneticFieldObj = MagneticFPlayer;
            }
            else if (gameObject.CompareTag("AI"))
            {
                // Ici, vous pouvez choisir dynamiquement lequel utiliser en fonction de l'IA.
                // Pour cet exemple, nous utilisons le premier.
                magneticFieldObj = MagneticFAi1;
            }

            if (magneticFieldObj == null)
            {
                Debug.LogWarning("Aucun objet magneticField n'a été assigné pour " + gameObject.name);
                yield break;
            }

            // Active le champ magnétique
            magneticFieldObj.SetActive(true);

            // On attend un frame pour être sûr que le collider est actif
            yield return null;

            // Récupère le collider du champ magnétique
            Collider fieldCollider = magneticFieldObj.GetComponent<Collider>();
            if (fieldCollider == null)
            {
                Debug.LogWarning("Aucun collider trouvé sur " + magneticFieldObj.name);
                yield break;
            }

            // Utilise OverlapBox pour récupérer tous les objets dans le volume du collider.
            // (Si le collider est sphérique, vous pouvez utiliser Physics.OverlapSphere)
            Collider[] affectedColliders = Physics.OverlapBox(
                fieldCollider.bounds.center,
                fieldCollider.bounds.extents,
                Quaternion.identity
            );

            // Liste pour stocker les objets affectés (pour leur retirer le ralentissement après)
            List<GameObject> affectedObjects = new List<GameObject>();

            // Applique le ralentissement de 20 % à chaque joueur ou IA présent
            foreach (Collider col in affectedColliders)
            {
                // On vérifie si le tag est "Player" ou "AI"
                if (col.gameObject.CompareTag("Player") || col.gameObject.CompareTag("AI"))
                {
                    // Pour éviter de s'appliquer son propre effet si besoin
                    // if(col.gameObject == this.gameObject) continue;

                    // Applique le ralentissement si le composant de contrôle existe
                    if (col.gameObject.CompareTag("Player"))
                    {
                        JoueurNav2 joueurScript = col.gameObject.GetComponent<JoueurNav2>();
                        if (joueurScript != null)
                        {
                            // Multiplie la vitesse par 0.8 (ralentissement de 20 %)
                            joueurScript.currentSpeedMultiplier *= 0.8f;
                            affectedObjects.Add(col.gameObject);
                        }
                    }
                    else if (col.gameObject.CompareTag("AI"))
                    {
                        Navigation8 aiScript = col.gameObject.GetComponent<Navigation8>();
                        if (aiScript != null)
                        {
                            aiScript.currentSpeedMultiplier *= 0.8f;
                            affectedObjects.Add(col.gameObject);
                        }
                    }
                }
            }

            // Le powerup dure 2 secondes
            yield return new WaitForSeconds(2f);

            // Restaure la vitesse d'origine en retirant le ralentissement appliqué
            foreach (GameObject obj in affectedObjects)
            {
                if (obj.CompareTag("Player"))
                {
                    JoueurNav2 joueurScript = obj.GetComponent<JoueurNav2>();
                    if (joueurScript != null)
                    {
                        // Inverse l'effet précédent
                        joueurScript.currentSpeedMultiplier /= 0.8f;
                    }
                }
                else if (obj.CompareTag("AI"))
                {
                    Navigation8 aiScript = obj.GetComponent<Navigation8>();
                    if (aiScript != null)
                    {
                        aiScript.currentSpeedMultiplier /= 0.8f;
                    }
                }
            }

            // Désactive le champ magnétique
            magneticFieldObj.SetActive(false);

            yield break;
        }*/

        yield break;
    }

    private IEnumerator FullHand()
    {
        Debug.Log(this.gameObject.name + " FullHand");
        yield break;
    }

    private IEnumerator Reload()
    {
        Debug.Log(this.gameObject.name + " Reload");
        yield break;
    }

    private IEnumerator Obstacle()
    {
        Debug.Log(this.gameObject.name + " Obstacle");
        yield break;
    }
}
