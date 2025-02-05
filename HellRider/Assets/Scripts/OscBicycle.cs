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

    private bool reset;
    private bool hasUser;

    public float X;


    private void Start()
    {
        // Touchdesigner
        oscReceiver.Bind("/X", TraiterXOSC); // left right value of the player
        oscReceiver.Bind("/Reset", TraiterResetOSC); // if no value change for 5 seconds (not possible if still on the bike)
        oscReceiver.Bind("/Intro", TraiterIntroOSC); // starts the tutorial for new player when acitvated

        //Arduino
        oscReceiver.Bind("/Affirm", TraiterConfirmOSC); // starts the tutorial for new player when acitvated
        oscReceiver.Bind("/Tease", TraiterPauseOSC); // starts the tutorial for new player when acitvated
    }

    private void Update()
    {
        if (reset == true)
        {
            RestartGame();
        }

        if (hasUser == false)
        {
            // show leader board and stuff
        } else
        {
            // hide leaderboard
        }

        //Debug.Log(X);
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

    public void RestartGame()
    {
        //SceneManager.LoadScene("TEST-Jay");
        hasUser = false;
        reset = false;
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

        Debug.Log("Confirm " + value);

        // Changer l'échelle de la valeur pour l'appliquer à la rotation :
        float rotation = ScaleValue(value, 0, 4095, 45, 315);
        // Appliquer la rotation au GameObject ciblé :
        //Joueur.transform.eulerAngles = new Vector3(0, rotation, 0);
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

        Debug.Log("pause " + value);

        // Changer l'échelle de la valeur pour l'appliquer à la rotation :
        float rotation = ScaleValue(value, 0, 4095, 45, 315);
        // Appliquer la rotation au GameObject ciblé :
        //Joueur.transform.eulerAngles = new Vector3(0, rotation, 0);
    }
}