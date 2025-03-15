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

    //Speed
    public extOSC.OSCReceiver oscReceiver;
    public extOSC.OSCTransmitter oscTransmitter;

    private bool reset;
    private bool hasUser;
    public bool InTutorial;

    public float Speed = 0;
    public float X = 0;
    //private float Center = 0;

    Scene scene;

    private void Awake()
    {
        scene = SceneManager.GetActiveScene();
    }

    private void Start()
    {
        // From Touchdesigner
        oscReceiver.Bind("/X", TraiterXOSC); // left right value of the player
        oscReceiver.Bind("/Reset", TraiterResetOSC); // if no value change for 5 seconds (not possible if still on the bike)
        oscReceiver.Bind("/Intro", TraiterIntroOSC); // starts the tutorial for new player when acitvated

        // From Arduino
        oscReceiver.Bind("/Raw", TraiterRawOSC); // starts the tutorial for new player when acitvated

        
        if (scene.name != "INTRO")
        {
            hasUser = true;
        }

    }

    public void Calibrator()
    {
        if (scene.name == "00a_tutorial_speed")
        {
            Debug.Log("set center");
            messageTransmitter("/Center", 0);
        }
        else if (scene.name == "00b_tutorial_turn")
        {
            Debug.Log("calibrating");
            messageTransmitter("/Calibrate", 1);
        }
        else
        {
            Debug.Log("stop calibrating");
            messageTransmitter("/Calibrate", 0);
        }

        messageTransmitter("/Reset", 0);
        Debug.Log("end of calibrator");
    }

    private void Update()
    {
        if (hasUser == false)
        {
            // show video
        }
        else
        {
            // hide video
        }

        //Debug.Log("has user " + hasUser);
        //Debug.Log("in tutorial " + InTutorial);


        
        
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

        //Debug.Log("Reset armed");

        reset = true;

        if (reset == true && hasUser == true)
        {
            RestartGame();
        }
    }

    public void RestartGame()
    {
        /*
        messageTransmitter("/Reset", 1);
        messageTransmitter("/Calibrate", 0);

        SceneManager.LoadScene(0);
        */
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

        
        //InTutorial = true;
        SceneManager.LoadScene(1);
    }

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

        //Debug.Log(Speed);
    }
}