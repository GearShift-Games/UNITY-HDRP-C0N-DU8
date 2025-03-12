using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using extOSC;
using static UnityEngine.GraphicsBuffer;
using Unity.VisualScripting;
using System;
using UnityEngine.SceneManagement;
using static UnityEditor.Experimental.GraphView.GraphView;
using TMPro;
using UnityEngine.UI;

public class OscBicycle : MonoBehaviour
{
    public extOSC.OSCReceiver oscReceiver;
    public extOSC.OSCTransmitter oscTransmitter;
    /*
    [Header("Tutorial Panel")]
    public GameObject[] tutorial;
    public GameObject tutorialPanel;
    private int tutorialNumber = 0;
    */
    // For the handling of resets and restarts
    private bool reset;
    private bool hasUser = true;
    private bool InTutorial = false;
    /*
    // Tutorial gameobject and such
    [Header("Tutorial Text")]
    public GameObject tutorialXPositionGameobject;
    public TMP_Text tutorialXPosition;
    public TMP_Text tutorialSetCenter;
    public TMP_Text tutorialSetLeft;
    public TMP_Text tutorialSetRight;
    public Slider tutorialSlider;
    
    // Value left and right for turning and for calibration
    
    private float Raw_x = 0;
    private float Left = 0;
    private float Center = 0;
    private float Right = 0; // put some 0 to help prevent major error when something goes wrong

    // Buttons control
    public float Confirm;
    public float Cancel;
    */
    //Speed

    public float Speed = 0;
    public float X = 0;



    private void Start()
    {
        // From Touchdesigner
        oscReceiver.Bind("/X", TraiterXOSC); // left right value of the player
        oscReceiver.Bind("/Reset", TraiterResetOSC); // if no value change for 5 seconds (not possible if still on the bike)
        oscReceiver.Bind("/Intro", TraiterIntroOSC); // starts the tutorial for new player when acitvated

        // From Arduino
        // oscReceiver.Bind("/Affirm", TraiterConfirmOSC); // starts the tutorial for new player when acitvated
        // oscReceiver.Bind("/Tease", TraiterPauseOSC); // starts the tutorial for new player when acitvated
        oscReceiver.Bind("/Raw", TraiterRawOSC); // starts the tutorial for new player when acitvated
        /*
        if (tutorialPanel != null)
        {
            oscReceiver.Bind("/X_RAW", TraiterXRAWOSC); // RAW left right value of the player for the tutorial only
            InTutorial = true;
        }
        */
    }

    private void Update()
    {
        if (hasUser == false)
        {
            // show leader board and stuff
        }
        else
        {
            // hide leaderboard
        }

        //Debug.Log("has user " + hasUser);
        //Debug.Log("in tutorial " + InTutorial);
        /*
        if (InTutorial == true)
        {
            tutorialXPosition.text = Raw_x.ToString();




            tutorialSlider.value = X;
        }
        */
    }

    private void messageTransmitter(string id, float value)
    {
        // Add ID
        var message = new OSCMessage(id);

        // Populate values.
        message.AddValue(OSCValue.Float(value));

        // Send message
        oscTransmitter.Send(message);
    }

    public static float ScaleValue(float value, float inputMin, float inputMax, float outputMin, float outputMax)
    {
        return Mathf.Clamp(((value - inputMin) / (inputMax - inputMin) * (outputMax - outputMin) + outputMin), outputMin, outputMax);
    }

    void TraiterMessageOSC(OSCMessage oscMessage)
    {
        // Récupérer une valeur numérique en tant que float
        // même si elle est de type float ou int :
        float value;
        if (oscMessage.Values[0].Type == OSCValueType.Int)
        {
            value = oscMessage.Values[0].IntValue;
        }
        else if (oscMessage.Values[0].Type == OSCValueType.Float)
        {
            value = oscMessage.Values[0].FloatValue;
        }
        else
        {
            // Si la valeur n'est ni un float ou int, on quitte la méthode :
            return;
        }

        Debug.Log(value);

        // Changer l'échelle de la valeur pour l'appliquer à la rotation :
        float rotation = ScaleValue(value, 0, 4095, 45, 315);
        // Appliquer la rotation au GameObject ciblé :
        //Joueur.transform.eulerAngles = new Vector3(0, rotation, 0);
    }

    void TraiterXOSC(OSCMessage oscMessage)
    {
        // Récupérer une valeur numérique en tant que float
        // même si elle est de type float ou int :
        float value;
        if (oscMessage.Values[0].Type == OSCValueType.Int)
        {
            value = oscMessage.Values[0].IntValue;
        }
        else if (oscMessage.Values[0].Type == OSCValueType.Float)
        {
            value = oscMessage.Values[0].FloatValue;
        }
        else
        {
            // Si la valeur n'est ni un float ou int, on quitte la méthode :
            return;
        }

        X = value;
        //Debug.Log(value);
    }

    /*
    void TraiterXRAWOSC(OSCMessage oscMessage)
    {
        if (InTutorial == true)
        {
            // Récupérer une valeur numérique en tant que float
            // même si elle est de type float ou int :
            float value;
            if (oscMessage.Values[0].Type == OSCValueType.Int)
            {
                value = oscMessage.Values[0].IntValue;
            }
            else if (oscMessage.Values[0].Type == OSCValueType.Float)
            {
                value = oscMessage.Values[0].FloatValue;
            }
            else
            {
                // Si la valeur n'est ni un float ou int, on quitte la méthode :
                return;
            }

            Raw_x = value;

            //Debug.Log(Raw_x);
        }
        return;
    }
    */

    void TraiterResetOSC(OSCMessage oscMessage)
    {
        // Récupérer une valeur numérique en tant que float
        // même si elle est de type float ou int :
        float value;
        if (oscMessage.Values[0].Type == OSCValueType.Int)
        {
            value = oscMessage.Values[0].IntValue;
        }
        else if (oscMessage.Values[0].Type == OSCValueType.Float)
        {
            value = oscMessage.Values[0].FloatValue;
        }
        else
        {
            // Si la valeur n'est ni un float ou int, on quitte la méthode :
            return;
        }

        Debug.Log("Reset armed");

        reset = true;

        if (reset == true && hasUser == true)
        {
            RestartGame();
        }
    }

    public void RestartGame()
    {
        //SceneManager.LoadScene("TUTORIAL");
        /*hasUser = false;
        reset = false;


        for (int i = 0; i < tutorial.Length; i++)
        {
            tutorial[i].SetActive(true);
        }
        tutorialNumber = 0;
        tutorialPanel.SetActive(true);
        tutorialXPositionGameobject.SetActive(true);*/
    }

    void TraiterIntroOSC(OSCMessage oscMessage)
    {
        // Récupérer une valeur numérique en tant que float
        // même si elle est de type float ou int :
        float value;
        if (oscMessage.Values[0].Type == OSCValueType.Int)
        {
            value = oscMessage.Values[0].IntValue;
        }
        else if (oscMessage.Values[0].Type == OSCValueType.Float)
        {
            value = oscMessage.Values[0].FloatValue;
        }
        else
        {
            // Si la valeur n'est ni un float ou int, on quitte la méthode :
            return;
        }

        //Debug.Log(value);

        hasUser = true;
        //InTutorial = true;
    }

    /* 
        void TraiterConfirmOSC(OSCMessage oscMessage)
        {
            // Récupérer une valeur numérique en tant que float
            // même si elle est de type float ou int :
            float value;
            if (oscMessage.Values[0].Type == OSCValueType.Int)
            {
                value = oscMessage.Values[0].IntValue;
            }
            else if (oscMessage.Values[0].Type == OSCValueType.Float)
            {
                value = oscMessage.Values[0].FloatValue;
            }
            else
            {
                // Si la valeur n'est ni un float ou int, on quitte la méthode :
                return;
            }

            Tuto order
             * 
             * 1 - explication basique du jeu
             * 2 - explication pedale
             * 3 - expliquer calibration
             * 4 - calibration milieu
             * 5 - calibration gauche
             * 6 - calibration droite
             * 7 - test calibration pour le joueur
             * 8 - appuyer de nouveau pour confirmer les calibration et commencer le jeu

            if (InTutorial == true) {
                if (value == 1 && tutorialNumber <= tutorial.Length - 1 && tutorialNumber >= 0)
                {
                    if (tutorialNumber == 3)
                    {
                        if (Raw_x < 0.1 && Raw_x > -0.1)
                        {
                            Center = Raw_x;
                            tutorialSetCenter.text = Center.ToString();
                            Debug.Log("center");
                            messageTransmitter("/Center", Center);
                            tutorial[tutorialNumber].SetActive(false);
                            tutorialNumber++;
                        }
                    }
                    else if (tutorialNumber == 4)
                    {
                        if (Raw_x < Center - 0.05)
                        {
                            Left = Raw_x;
                            tutorialSetLeft.text = Left.ToString();
                            Debug.Log("left");
                            messageTransmitter("/Left", Left);
                            tutorial[tutorialNumber].SetActive(false);
                            tutorialNumber++;
                        }
                    }
                    else if (tutorialNumber == 5)
                    {
                        if (Raw_x > Center + 0.05)
                        {
                            Right = Raw_x;
                            tutorialSetRight.text = Right.ToString();
                            Debug.Log("right");
                            messageTransmitter("/Right", Right);
                            tutorial[tutorialNumber].SetActive(false);
                            tutorialNumber++;
                            tutorialXPositionGameobject.SetActive(false);
                        }
                    } else
                    {
                        tutorial[tutorialNumber].SetActive(false);
                        tutorialNumber++;
                    }

                    Debug.Log(tutorialNumber);

                    if (tutorialNumber == tutorial.Length)
                    {
                        tutorialPanel.SetActive(false);
                        Debug.Log("tutorial done");
                        InTutorial = false;
                        SceneManager.LoadScene("Circuit01_Maquette");
                    }

                }

            }
            else
            {
                Confirm = value;
            }

        }

        void TraiterPauseOSC(OSCMessage oscMessage)
        {
            // Récupérer une valeur numérique en tant que float
            // même si elle est de type float ou int :
            float value;
            if (oscMessage.Values[0].Type == OSCValueType.Int)
            {
                value = oscMessage.Values[0].IntValue;
            }
            else if (oscMessage.Values[0].Type == OSCValueType.Float)
            {
                value = oscMessage.Values[0].FloatValue;
            }
            else
            {
                // Si la valeur n'est ni un float ou int, on quitte la méthode :
                return;
            }

            if (InTutorial == true) {
                if (value == 1 && tutorialNumber <= 7 && tutorialNumber >= 1) // 3 tuto for now
                {
                    Debug.Log("pause " + value);
                    tutorialNumber--;
                    tutorial[tutorialNumber].SetActive(true);

                    if (tutorialNumber == 6)
                    {
                        tutorialXPositionGameobject.SetActive(true);
                    }
                }
            }
            else
            {
                Cancel = value;
            }
        }
    */
    void TraiterRawOSC(OSCMessage oscMessage)
    {
        // Récupérer une valeur numérique en tant que float
        // même si elle est de type float ou int :
        float value;
        if (oscMessage.Values[0].Type == OSCValueType.Int)
        {
            value = oscMessage.Values[0].IntValue;
        }
        else if (oscMessage.Values[0].Type == OSCValueType.Float)
        {
            value = oscMessage.Values[0].FloatValue;
        }
        else
        {
            // Si la valeur n'est ni un float ou int, on quitte la méthode :
            return;
        }

        //Debug.Log(value);

        //value = ScaleValue(value, -15000,15000,-1,1);

        Speed = value/30000f;
        Speed = Speed * Speed * Speed;

        Debug.Log(Speed);
    }
}