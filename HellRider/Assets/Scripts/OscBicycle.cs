using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using extOSC;
using static UnityEngine.GraphicsBuffer;
using Unity.VisualScripting;
using System;
using UnityEngine.SceneManagement;
using static UnityEditor.Experimental.GraphView.GraphView;

public class OscBicycle : MonoBehaviour
{
    public extOSC.OSCReceiver oscReceiver;
    public extOSC.OSCTransmitter oscTransmitter;

    public GameObject[] tutorial;
    public GameObject tutorialPanel;
    private int tutorialNumber = 0;

    // For the handling of resets and restarts
    private bool reset;
    private bool hasUser;
    private bool InTutorial;

    // Value left and right for turning
    public float X;
    private float Left;
    private float Center;
    private float Right;

    private float Confirm;
    private float Cancel;


    private void Start()
    {
        // From Touchdesigner
        oscReceiver.Bind("/X", TraiterXOSC); // left right value of the player
        oscReceiver.Bind("/Reset", TraiterResetOSC); // if no value change for 5 seconds (not possible if still on the bike)
        oscReceiver.Bind("/Intro", TraiterIntroOSC); // starts the tutorial for new player when acitvated

        // From Arduino
        oscReceiver.Bind("/Affirm", TraiterConfirmOSC); // starts the tutorial for new player when acitvated
        oscReceiver.Bind("/Tease", TraiterPauseOSC); // starts the tutorial for new player when acitvated

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

        //Debug.Log(X);

        messageTransmitter("/test", 3);
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

        Debug.Log("Reset " + value);

        reset = true;

        if (reset == true && hasUser == true)
        {
            RestartGame();
        }
    }

    public void RestartGame()
    {
        //SceneManager.LoadScene("TEST-Jay");
        hasUser = false;
        reset = false;


        for (int i = 0; i < tutorial.Length; i++)
        {
            tutorial[i].SetActive(true);
        }
        tutorialNumber = 0;
        tutorialPanel.SetActive(true);
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
        InTutorial = true;
    }

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

        Confirm = value;


        /* Tuto order
         * 
         * 1 - explication basique du jeu
         * 2 - explication pedale
         * 3 - expliquer calibration
         * 4 - calibration milieu
         * 5 - calibration gauche
         * 6 - calibration droite
         * 7 - test calibration pour le joueur
         * 8 - appuyer de nouveau pour confirmer les calibration et commencer le jeu
         */
        //if (InTutorial == true) {
        if (value == 1 && tutorialNumber <= tutorial.Length - 1 && tutorialNumber >= 0)
        {
            if (tutorialNumber == 3)
            {
                if (X < 0.1 && X > -0.1)
                {
                    Center = X;
                    messageTransmitter("/Center", Center);
                }
            }
            else if (tutorialNumber == 4)
            {
                if (X < -0.1)
                {
                    Left = X;
                    messageTransmitter("/Left", Left);
                }
            }
            else if (tutorialNumber == 5)
            {
                if (X > 0.1)
                {
                    Right = X;
                    messageTransmitter("/Right", Right); 
                }
            }

            Debug.Log("Confirm " + X);
            tutorial[tutorialNumber].SetActive(false);
            tutorialNumber++;
        }
        //}
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

        Cancel = value;

        //if (InTutorial == true) {
        if (value == 1 && tutorialNumber <= 2 && tutorialNumber >= 1) // 3 tuto for now
        {
            Debug.Log("pause " + value);
            tutorialNumber--;
            tutorial[tutorialNumber].SetActive(true);
        }
        //}
    }
}