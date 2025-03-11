using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class PowerUps : MonoBehaviour
{

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
