using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class IntroCam : MonoBehaviour
{
    // All the possible camera positions (aka GameObjects called LookAtMe inside AIs)
    public Transform[] allCams;

    // Timer that switches automatically camera angle
    public float timerClock = 7.8f;

    // The actual camera
    public CinemachineVirtualCamera virtualCamera;

    // Variables to store the Follow and LookAt attributes
    private Transform followTarget;
    private Transform lookAtTarget;

    // Update is called once per frame
    void Update()
    {
        timerClock += Time.deltaTime;
        if (timerClock >= 8)
        {
            int randomIndex = Random.Range(0, allCams.Length);
            Transform cameraPos = allCams[randomIndex];
            virtualCamera.Follow = cameraPos.transform;
            virtualCamera.LookAt = cameraPos.transform;
            timerClock = 0;
        }
    }
}
