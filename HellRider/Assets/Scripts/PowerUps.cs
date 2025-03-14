using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class PowerUps : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] TurboSound;
    public GameObject[] otherPlayers;

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
     *  dieportal: Téléporte avec un joueur aléatoire (last and before last place when more than 2/3 player)
     *  Recharge: Recharge de fuse 6 seconde (3e a 5e place et peut pas aller plus haut que 100%) 
     *  Laser: Ralenti tous les joueurs touché
     *  Hacking: Le premier joueur freine
     *  Shield: Ne peut être affecter par un malus, utiliser une seule fois
     *  
     *  
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



    //to store their values
    float theirScore;
    int theirPath;
    int theirwaypointIndex;
    UnityEngine.Vector3 their3DPosition;

    //to store our values
    float thisScore;
    int thisPath;
    int thiswaypointIndex;
    UnityEngine.Vector3 this3DPosition;



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
        //Debug.Log(this.gameObject.name + " " + this.gameObject.transform.position);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ItemBox") && this.gameObject.CompareTag("Player"))
        {
            //Debug.Log(this.gameObject.name + " boxed Player");
            //PowerChooser(position, PlayersAlive);
            //StartCoroutine("DiePortal");
        }
        else if (other.CompareTag("ItemBox") && this.gameObject.CompareTag("AI"))
        {
            //Debug.Log(this.gameObject.name + " boxed AI");
            //PowerChooser(position, PlayersAlive);
            //StartCoroutine("DiePortal");
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
        //Debug.Log("position : " + Position + ", PlayersAlive : " + playersAlive);
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
        if (this.gameObject.CompareTag("AI"))
        {
            AI.currentSpeedMultiplier = 2;
        }
        else if (this.gameObject.CompareTag("Player"))
        {
            Joueur.currentSpeedMultiplier = 2;
            // Rajouter son ici
            
            int randomIndex = Random.Range(0, TurboSound.Length); // Sélection aléatoire
            audioSource.PlayOneShot(TurboSound[randomIndex], 1f);

        }

        Debug.Log(this.gameObject.name + " Boostah ON!");
        yield return new WaitForSeconds(3f);

        if (this.gameObject.CompareTag("AI"))
        {
            AI.currentSpeedMultiplier = 1;
        }
        else if (this.gameObject.CompareTag("Player"))
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


        //Debug.Log(this.gameObject.name + " DiePortal");

        int randomIndex = Random.Range(0, otherPlayers.Length);

        //make sure the one picked isnt already dead
        while (!otherPlayers[randomIndex].activeInHierarchy)
        {
            randomIndex = Random.Range(0, otherPlayers.Length);
        }

        //Debug.Log(this.gameObject.name + " switch " + otherPlayers[randomIndex].name);

        //getting this gameobject value
        if (this.gameObject.CompareTag("AI"))
        {
            thisScore = AI.score;
            thisPath = AI.CurrentPathNumber;
            thiswaypointIndex = AI.currentWaypointIndex;
        }
        else if (this.gameObject.CompareTag("Player"))
        {
            thisScore = Joueur.score;
            thisPath = Joueur.CurrentPathNumber;
            thiswaypointIndex = Joueur.currentWaypointIndex;
        }


        if (otherPlayers[randomIndex].gameObject.CompareTag("AI"))
        {
            //get their values
            theirScore = otherPlayers[randomIndex].GetComponent<Navigation8>().score;
            theirPath = otherPlayers[randomIndex].GetComponent<Navigation8>().CurrentPathNumber;
            theirwaypointIndex = otherPlayers[randomIndex].GetComponent<Navigation8>().currentWaypointIndex;

            otherPlayers[randomIndex].GetComponent<Navigation8>().score = thisScore;
            otherPlayers[randomIndex].GetComponent<Navigation8>().CurrentPathNumber = thisPath;
            otherPlayers[randomIndex].GetComponent<Navigation8>().currentWaypointIndex = thiswaypointIndex;
            otherPlayers[randomIndex].GetComponent<Navigation8>().changeRoutePortal();
        }
        else if (otherPlayers[randomIndex].gameObject.CompareTag("Player"))
        {
            //get their values
            theirScore = otherPlayers[randomIndex].GetComponent<JoueurNav2>().score;
            theirPath = otherPlayers[randomIndex].GetComponent<JoueurNav2>().CurrentPathNumber;
            theirwaypointIndex = otherPlayers[randomIndex].GetComponent<JoueurNav2>().currentWaypointIndex;

            otherPlayers[randomIndex].GetComponent<JoueurNav2>().score = thisScore;
            otherPlayers[randomIndex].GetComponent<JoueurNav2>().CurrentPathNumber = thisPath;
            otherPlayers[randomIndex].GetComponent<JoueurNav2>().currentWaypointIndex = thiswaypointIndex;
            otherPlayers[randomIndex].GetComponent<JoueurNav2>().changeRoutePortal();
        }



        if (this.gameObject.CompareTag("AI"))
        {
            AI.score = theirScore;
            AI.CurrentPathNumber = theirPath;
            AI.currentWaypointIndex = theirwaypointIndex;
            AI.GetComponent<Navigation8>().changeRoutePortal();

        }
        else if (this.gameObject.CompareTag("Player"))
        {
            Joueur.score = theirScore;
            Joueur.CurrentPathNumber = theirPath;
            Joueur.currentWaypointIndex = theirwaypointIndex;
            Joueur.GetComponent<JoueurNav2>().changeRoutePortal();
        }

        their3DPosition = otherPlayers[randomIndex].transform.position;
        this3DPosition = this.gameObject.transform.position;

        this.gameObject.transform.position = their3DPosition;
        otherPlayers[randomIndex].transform.position = this3DPosition;

        yield break;
    }

    private IEnumerator NewFuse()
    {
        Debug.Log(this.gameObject.name + " NewFuse");
        yield break;
    }

    private IEnumerator MagneticField()
    {
        Debug.Log(this.gameObject.name + " MagneticField");

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
