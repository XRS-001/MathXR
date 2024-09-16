using System.Collections;
using System.Collections.Generic;
using RootMotion.Demos;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class Controller
    {
        public GameObject leftHandControllerMesh;
        public Vector3 rayCharacterLeftHandOffset;
        public Transform rayOffsetLeft;
        [HideInInspector]
        public Transform leftIKTarget;
        [HideInInspector]
        public Vector3 leftIKTargetStartPosition;
        [HideInInspector]
        public Quaternion leftIKTargetStartRotation;
        public Transform leftHandBonePoint;
        public GameObject rightHandControllerMesh;
        public Vector3 rayCharacterRightHandOffset;
        public Transform rayOffsetRight;
        [HideInInspector]
        public Transform rightIKTarget;
        [HideInInspector]
        public Vector3 rightIKTargetStartPosition;
        [HideInInspector]
        public Quaternion rightIKTargetStartRotation;
        public Transform rightHandBonePoint;
    }
    public Controller controller;
    public GameObject playerMesh;
    public VRIKCalibrationBasic VRIKCalibration;
    [System.Serializable]
    public class GameUI
    {
        public GameObject startUI;
        public GameObject handSelectionUI;
        public GameObject gameModeSelectionUI;
    }
    public GameUI gameUI;
    [System.Serializable]
    public class Bow
    {
        public bool leftHanded = false;
        [HideInInspector]
        public bool leftGrabbing, rightGrabbing;
        public Transform leftController, rightController;
        public float stringGrabRadius;
        public Vector3 grabPoint;
        public GameObject leftHandedBow, rightHandedBow;
        public Vector3 leftBowPositionOffset, leftBowRotationOffset, rightBowPositionOffset, rightBowRotationOffset;
        [HideInInspector]
        public BowSettings spawnedBow;
        [HideInInspector]
        public bool leftGrabbingString, rightGrabbingString;
        public float stringDisconnectThreshold;
        public Vector3 stringLeftGrabPositionOffset, stringRightGrabPositionOffset, stringLeftGrabRotationOffset, stringRightGrabRotationOffset;
        public InputActionProperty leftGrabInput, rightGrabInput;
        [HideInInspector]
        public bool offsetSet = false;
        [HideInInspector]
        public Vector3 stringGrabOffset;
        public float stringDistanceTarget;
    }
    public Bow bow;
    public enum GameModeSelection { additionAndSubtraction, multiplication, division }
    private GameModeSelection gameMode;
    public void StartGame()
    {
        controller.leftHandControllerMesh.SetActive(false);
        controller.rayOffsetLeft.localPosition = controller.rayCharacterLeftHandOffset;
        controller.rightHandControllerMesh.SetActive(false);
        controller.rayOffsetRight.localPosition = controller.rayCharacterRightHandOffset;
        playerMesh.SetActive(true);
        VRIKCalibration.Calibrate();
        gameUI.startUI.SetActive(false);
        gameUI.handSelectionUI.SetActive(true);

        controller.leftIKTarget = GameObject.Find("Left Hand IK Target").transform;
        controller.leftIKTargetStartPosition = controller.leftIKTarget.localPosition;
        controller.leftIKTargetStartRotation = controller.leftIKTarget.localRotation;

        controller.rightIKTarget = GameObject.Find("Right Hand IK Target").transform;
        controller.rightIKTargetStartPosition = controller.rightIKTarget.localPosition;
        controller.rightIKTargetStartRotation = controller.rightIKTarget.localRotation;
    }
    public void HandPicked(bool isLeftHanded)
    {
        if (isLeftHanded)
            bow.leftHanded = true;
        gameUI.handSelectionUI.SetActive(false);
        gameUI.gameModeSelectionUI.SetActive(true);
    }
    public void PickGameMode(string gameModeChosen)
    {
        switch (gameModeChosen)
        {
            case "Addition & Subtraction":
                gameUI.gameModeSelectionUI.SetActive(false);
                break;

            case "Multiplication":
                gameUI.gameModeSelectionUI.SetActive(false);
                break;

            case "Division":
                gameUI.gameModeSelectionUI.SetActive(false);
                break;
        }
        if (bow.leftHanded)
        {
            bow.spawnedBow = Instantiate(bow.leftHandedBow, bow.rightController).GetComponent<BowSettings>();
            bow.spawnedBow.transform.localPosition = bow.leftBowPositionOffset;
            bow.spawnedBow.transform.localRotation = Quaternion.Euler(bow.leftBowRotationOffset);
            bow.rightGrabbing = true;
        }
        else
        {
            bow.spawnedBow = Instantiate(bow.rightHandedBow, bow.leftController).GetComponent<BowSettings>();
            bow.spawnedBow.transform.localPosition = bow.rightBowPositionOffset;
            bow.spawnedBow.transform.localRotation = Quaternion.Euler(bow.rightBowRotationOffset);
            bow.rightGrabbing = true;
        }
    }
    private void Update()
    {
        if(Physics.CheckSphere(bow.rightController.transform.position - (bow.rightController.rotation * bow.grabPoint), bow.stringGrabRadius) || Physics.CheckSphere(bow.leftController.transform.position - (bow.leftController.rotation * new Vector3(-bow.grabPoint.x, bow.grabPoint.y, bow.grabPoint.z)), bow.stringGrabRadius))
        {
            Collider colliderRight = Physics.OverlapSphere(bow.rightController.transform.position - (bow.rightController.rotation * bow.grabPoint), bow.stringGrabRadius)?[0];
            if (!colliderRight)
            {
                Collider colliderLeft = Physics.OverlapSphere(bow.leftController.transform.position - (bow.leftController.rotation * new Vector3(-bow.grabPoint.x, bow.grabPoint.y, bow.grabPoint.z)), bow.stringGrabRadius)?[0];
                if (colliderLeft.name != "StringGrabPoint")
                    colliderLeft = null;
                if (colliderLeft && bow.leftGrabInput.action.ReadValue<float>() > 0.25f)
                    bow.leftGrabbingString = true;
            }
            else
            {
                if (colliderRight.name != "StringGrabPoint")
                    colliderRight = null;
                if (colliderRight && bow.rightGrabInput.action.ReadValue<float>() > 0.25f) 
                {
                    bow.rightGrabbingString = true;
                }
            }
        }
        if (bow.rightGrabbingString)
        {
            controller.rightIKTarget.position = bow.spawnedBow.stringArmaturePiece.transform.position - (bow.spawnedBow.stringArmaturePiece.transform.rotation * bow.stringRightGrabPositionOffset);
            controller.rightIKTarget.rotation = bow.spawnedBow.stringArmaturePiece.transform.rotation * Quaternion.Euler(bow.stringRightGrabRotationOffset);
            Vector3 handInBowSpace = bow.spawnedBow.stringGrabPoint.transform.InverseTransformPoint(bow.rightController.position);
            handInBowSpace = new Vector3(bow.spawnedBow.stringGrabPoint.transform.position.x, bow.spawnedBow.stringGrabPoint.transform.position.y, handInBowSpace.z);
            if (!bow.offsetSet)
            {
                bow.stringGrabOffset = handInBowSpace - bow.spawnedBow.stringGrabPoint.transform.position;
                bow.offsetSet = true;
            }
            bow.spawnedBow.GetComponent<Animator>().SetFloat("PullBack", Mathf.Lerp(bow.spawnedBow.GetComponent<Animator>().GetFloat("PullBack"), Mathf.Clamp(Mathf.Clamp((bow.spawnedBow.stringGrabPoint.transform.position - (handInBowSpace - bow.stringGrabOffset)).z, 0, float.PositiveInfinity) / bow.stringDistanceTarget, 0, 1), 0.1f));
        }
        else if (bow.leftGrabbingString)
        {
            
        }
        if ((bow.rightGrabInput.action.ReadValue<float>() < 0.25f || Vector3.Distance(controller.leftIKTarget.position, controller.leftHandBonePoint.position) > bow.stringDisconnectThreshold) && !bow.leftGrabbingString)
        {
            bow.rightGrabbingString = false;
            if(bow.spawnedBow)
                bow.spawnedBow.GetComponent<Animator>().SetFloat("PullBack", Mathf.Lerp(bow.spawnedBow.GetComponent<Animator>().GetFloat("PullBack"), 0, 0.1f));
            if (controller.rightIKTarget && bow.offsetSet)
            {
                controller.rightIKTarget.localPosition = controller.rightIKTargetStartPosition;
                controller.rightIKTarget.localRotation = controller.rightIKTargetStartRotation;
            }
            bow.offsetSet = false;
        }

        if ((bow.leftGrabInput.action.ReadValue<float>() < 0.25f || Vector3.Distance(controller.leftIKTarget.position, controller.leftHandBonePoint.position) > bow.stringDisconnectThreshold) && !bow.rightGrabbingString)
        {
            bow.leftGrabbingString = false;
            if (bow.spawnedBow)
                bow.spawnedBow.GetComponent<Animator>().SetFloat("PullBack", Mathf.Lerp(bow.spawnedBow.GetComponent<Animator>().GetFloat("PullBack"), 0, 0.1f));
            if (controller.leftIKTarget && bow.offsetSet)
            {
                controller.leftIKTarget.localPosition = controller.leftIKTargetStartPosition;
                controller.leftIKTarget.localRotation = controller.leftIKTargetStartRotation;
            }
            bow.offsetSet = false;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.25f);
        Gizmos.DrawSphere(bow.rightController.transform.position - (bow.rightController.rotation * bow.grabPoint), bow.stringGrabRadius);
        Gizmos.DrawSphere(bow.leftController.transform.position - (bow.leftController.rotation * new Vector3(-bow.grabPoint.x, bow.grabPoint.y, bow.grabPoint.z)), bow.stringGrabRadius);
    }
}
