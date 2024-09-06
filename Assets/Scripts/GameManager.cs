using System.Collections;
using System.Collections.Generic;
using RootMotion.Demos;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject leftHandControllerMesh;
    public GameObject rightHandControllerMesh;
    public GameObject playerMesh;
    public VRIKCalibrationBasic VRIKCalibration;
    public GameObject startUI;
    public void StartGame()
    {
        leftHandControllerMesh.SetActive(false);
        rightHandControllerMesh.SetActive(false);
        playerMesh.SetActive(true);
        VRIKCalibration.Calibrate();
        startUI.SetActive(false);
    }
}
