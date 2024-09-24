using System.Collections;
using System.Collections.Generic;
using RootMotion.Demos;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class Controller
    {
        public Transform leftController;
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
        public Transform rightController;
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
    public class Game
    {
        public GameObject startUI;
        public GameObject handSelectionUI;
        public GameObject gameModeSelectionUI;
        public GameObject gameUI;
        public Transform[] spawnPoints;
        public PotentialAnswer[] potentialAnswer;
        public TextMeshProUGUI questionText, scoreText;
        public int questionsToAnswer;
        public float timeBetweenPotentialAnswers;
        public float oddsOfAnswer;
        [HideInInspector]
        public int score;
        [HideInInspector]
        public int answer;
        [HideInInspector]
        public bool correctAnswerHit = false;
    }
    public Game game;
    [System.Serializable]
    public class Bow
    {
        public bool leftHanded = false;
        [HideInInspector]
        public bool leftGrabbing, rightGrabbing;
        public float stringGrabRadius;
        public Vector3 grabPoint;
        public GameObject leftHandedBow, rightHandedBow;
        public GameObject arrowToShoot;
        public float arrowStrengthMultiplier;
        public Vector3 leftBowPositionOffset, leftBowRotationOffset, rightBowPositionOffset, rightBowRotationOffset;
        [HideInInspector]
        public BowSettings spawnedBow;
        [HideInInspector]
        public bool leftGrabbingString, rightGrabbingString;
        public float stringDisconnectThreshold;
        public Vector3 stringLeftGrabPositionOffset, stringRightGrabPositionOffset, stringLeftGrabRotationOffset, stringRightGrabRotationOffset;
        public InputActionProperty leftGrabInput, rightGrabInput;
        [HideInInspector]
        public bool handsReachedTarget = false, handsReachingTarget = false, offsetSet = false, hasFiredArrow = false;
        [HideInInspector]
        public Vector3 stringGrabOffset;
        public float stringDistanceTarget;
        [HideInInspector]
        public float shotStrength;
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
        game.startUI.SetActive(false);
        game.handSelectionUI.SetActive(true);

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
        game.handSelectionUI.SetActive(false);
        game.gameModeSelectionUI.SetActive(true);
    }
    public void PickGameMode(string gameModeChosen)
    {
        switch (gameModeChosen)
        {
            case "Addition & Subtraction":
                game.gameModeSelectionUI.SetActive(false);
                game.gameUI.SetActive(true);
                StartCoroutine(RunGame());
                break;

            case "Multiplication":
                game.gameModeSelectionUI.SetActive(false);
                game.gameUI.SetActive(true);
                break;

            case "Division":
                game.gameModeSelectionUI.SetActive(false);
                game.gameUI.SetActive(true);
                break;
        }
        if (bow.leftHanded)
        {
            bow.spawnedBow = Instantiate(bow.leftHandedBow, controller.rightIKTarget).GetComponent<BowSettings>();
            bow.spawnedBow.transform.localPosition = bow.leftBowPositionOffset;
            bow.spawnedBow.transform.localRotation = Quaternion.Euler(bow.leftBowRotationOffset);
            bow.leftGrabbing = true;
        }
        else
        {
            bow.spawnedBow = Instantiate(bow.rightHandedBow, controller.leftIKTarget).GetComponent<BowSettings>();
            bow.spawnedBow.transform.localPosition = bow.rightBowPositionOffset;
            bow.spawnedBow.transform.localRotation = Quaternion.Euler(bow.rightBowRotationOffset);
            bow.rightGrabbing = true;
        }
    }
    IEnumerator RunGame()
    {
        float timer = 0;
        game.answer = Random.Range(1, 101);
        int firstNumberInSum = Random.Range(1, game.answer);
        game.questionText.text = $"{firstNumberInSum} + {game.answer - firstNumberInSum}";
        while (game.score < game.questionsToAnswer)
        {
            timer += Time.deltaTime;
            if(timer > game.timeBetweenPotentialAnswers)
            {
                int randomColor = Random.Range(0, game.potentialAnswer.Length - 1);
                int randomSpawnPoint = Random.Range(0, game.spawnPoints.Length - 1);
                PotentialAnswer spawnedPotentialAnswer = Instantiate(game.potentialAnswer[randomColor], game.spawnPoints[randomSpawnPoint].position, Quaternion.LookRotation(game.spawnPoints[randomSpawnPoint].position - Vector3.zero, Vector3.up)).GetComponent<PotentialAnswer>();
                spawnedPotentialAnswer.body.AddForce(Vector3.up * 100, ForceMode.Force);

                bool isAnswer = Random.Range(1, (int)(1 / game.oddsOfAnswer) + 1) == 1;
                if (isAnswer)
                {
                    spawnedPotentialAnswer.numberText.text = game.answer.ToString();
                }
                else
                {
                    int number = Random.Range(1, 101);
                    spawnedPotentialAnswer.numberText.text = number.ToString();
                }
                timer = 0;
            }
            if(game.correctAnswerHit == true)
            {
                game.correctAnswerHit = false;
                game.score++;
                game.answer = Random.Range(1, 101);
                firstNumberInSum = Random.Range(1, game.answer);
                game.questionText.text = $"{firstNumberInSum} + {game.answer - firstNumberInSum}";
            }
            game.scoreText.text = $"Score: {game.score}";
            yield return null;
        }
        game.gameModeSelectionUI.SetActive(true);
        game.gameUI.SetActive(false);
        yield return null;
    }
    private void Update()
    {
        if(Physics.CheckSphere(controller.rightController.transform.position - (controller.rightController.rotation * bow.grabPoint), bow.stringGrabRadius) || Physics.CheckSphere(controller.leftController.transform.position - (controller.leftController.rotation * new Vector3(-bow.grabPoint.x, bow.grabPoint.y, bow.grabPoint.z)), bow.stringGrabRadius))
        {
            Collider[] colliderRight = Physics.OverlapSphere(controller.rightController.transform.position - (controller.rightController.rotation * bow.grabPoint), bow.stringGrabRadius);
            if (colliderRight.Length == 0)
            {
                Collider[] colliderLeft = Physics.OverlapSphere(controller.leftController.transform.position - (controller.leftController.rotation * new Vector3(-bow.grabPoint.x, bow.grabPoint.y, bow.grabPoint.z)), bow.stringGrabRadius);
                if(colliderLeft.Length > 0)
                {
                    if (colliderLeft[0])
                    {
                        if (colliderLeft[0].name != "StringGrabPoint")
                            colliderLeft = null;
                        else if (bow.leftGrabInput.action.ReadValue<float>() > 0.25f)
                            bow.leftGrabbingString = true;
                    }
                }
            }
            else if (colliderRight[0])
            {
                if (colliderRight[0].name != "StringGrabPoint")
                    colliderRight = null;
                if (colliderRight[0] && bow.rightGrabInput.action.ReadValue<float>() > 0.25f) 
                {
                    bow.rightGrabbingString = true;
                }
            }
        }
        if (bow.rightGrabbingString)
        {
            bow.hasFiredArrow = false;
            bow.spawnedBow.arrowModel.SetActive(true);
            if (!bow.handsReachingTarget)
            {
                Invoke(nameof(HandsReachedTarget), 0.1f);
                bow.handsReachingTarget = true;
            }
            if (!bow.handsReachedTarget)
            {
                controller.rightIKTarget.position = Vector3.Lerp(controller.rightIKTarget.position, bow.spawnedBow.stringArmaturePiece.transform.position - (bow.spawnedBow.stringArmaturePiece.transform.rotation * bow.stringRightGrabPositionOffset), 0.3f);
                controller.rightIKTarget.rotation = Quaternion.Slerp(controller.rightIKTarget.rotation, bow.spawnedBow.stringArmaturePiece.transform.rotation * Quaternion.Euler(bow.stringRightGrabRotationOffset), 0.3f);
            }
            else
            {
                controller.rightIKTarget.position = bow.spawnedBow.stringArmaturePiece.transform.position - (bow.spawnedBow.stringArmaturePiece.transform.rotation * bow.stringRightGrabPositionOffset);
                controller.rightIKTarget.rotation = bow.spawnedBow.stringArmaturePiece.transform.rotation * Quaternion.Euler(bow.stringRightGrabRotationOffset);
            }
            Vector3 handInBowSpace = bow.spawnedBow.stringGrabPoint.transform.InverseTransformPoint(controller.rightController.position);
            handInBowSpace = new Vector3(bow.spawnedBow.stringGrabPoint.transform.position.x, bow.spawnedBow.stringGrabPoint.transform.position.y, handInBowSpace.z);
            if (!bow.offsetSet)
            {
                bow.stringGrabOffset = handInBowSpace - bow.spawnedBow.stringGrabPoint.transform.position;
                bow.offsetSet = true;
            }
            float pullBackValue = Mathf.Lerp(bow.spawnedBow.GetComponent<Animator>().GetFloat("PullBack"), Mathf.Clamp(Mathf.Clamp((bow.spawnedBow.stringGrabPoint.transform.position - (handInBowSpace - bow.stringGrabOffset)).z, 0, float.PositiveInfinity) / bow.stringDistanceTarget, 0, 1), 0.1f);
            bow.spawnedBow.GetComponent<Animator>().SetFloat("PullBack", pullBackValue);
            bow.shotStrength = pullBackValue;
        }
        else if (bow.leftGrabbingString)
        {
            
        }
        if ((bow.rightGrabInput.action.ReadValue<float>() < 0.25f || Vector3.Distance(controller.leftIKTarget.position, controller.leftHandBonePoint.position) > bow.stringDisconnectThreshold) && !bow.leftGrabbingString)
        {
            bow.rightGrabbingString = false;
            if(bow.spawnedBow)
                bow.spawnedBow.GetComponent<Animator>().SetFloat("PullBack", Mathf.Lerp(bow.spawnedBow.GetComponent<Animator>().GetFloat("PullBack"), 0, 0.3f));

            if(bow.handsReachingTarget)
            {
                Invoke(nameof(HandsReachedControllers), 0.1f);
                bow.handsReachingTarget = false;
            }
            if (controller.rightIKTarget && bow.offsetSet && !bow.handsReachedTarget)
            {
                controller.rightIKTarget.localPosition = controller.rightIKTargetStartPosition;
                controller.rightIKTarget.localRotation = controller.rightIKTargetStartRotation;

                if (controller.leftIKTarget)
                    controller.leftIKTarget.localRotation = controller.leftIKTargetStartRotation;

                bow.offsetSet = false;
            }
            if (controller.rightIKTarget && bow.offsetSet && bow.handsReachedTarget)
            {
                controller.rightIKTarget.localPosition = Vector3.Lerp(controller.rightIKTarget.localPosition, controller.rightIKTargetStartPosition, 0.3f);
                controller.rightIKTarget.localRotation = Quaternion.Slerp(controller.rightIKTarget.localRotation, controller.rightIKTargetStartRotation, 0.3f);

                if (controller.leftIKTarget)
                    controller.leftIKTarget.localRotation = Quaternion.Slerp(controller.leftIKTarget.localRotation, controller.leftIKTargetStartRotation, 0.3f);
            }
            if (bow.shotStrength > 0.5f && !bow.hasFiredArrow)
            {
                FireArrow();
                bow.hasFiredArrow = true;
            }
        }

        if ((bow.leftGrabInput.action.ReadValue<float>() < 0.25f || Vector3.Distance(controller.leftIKTarget.position, controller.leftHandBonePoint.position) > bow.stringDisconnectThreshold) && !bow.rightGrabbingString)
        {
            bow.leftGrabbingString = false;
            if (bow.spawnedBow)
                bow.spawnedBow.GetComponent<Animator>().SetFloat("PullBack", Mathf.Lerp(bow.spawnedBow.GetComponent<Animator>().GetFloat("PullBack"), 0, 0.3f));

            if (bow.handsReachingTarget)
            {
                Invoke(nameof(HandsReachedControllers), 0.1f);
                bow.handsReachingTarget = false;
            }
            if (controller.leftIKTarget && bow.offsetSet && !bow.handsReachedTarget)
            {
                controller.leftIKTarget.localPosition = controller.leftIKTargetStartPosition;
                controller.leftIKTarget.localRotation = controller.leftIKTargetStartRotation;

                if (controller.rightIKTarget)
                    controller.rightIKTarget.localRotation = controller.rightIKTargetStartRotation;

                bow.offsetSet = false;
            }
            if (controller.leftIKTarget && bow.offsetSet && bow.handsReachedTarget)
            {
                controller.leftIKTarget.localPosition = Vector3.Lerp(controller.leftIKTarget.localPosition, controller.leftIKTargetStartPosition, 0.3f);
                controller.leftIKTarget.localRotation = Quaternion.Slerp(controller.leftIKTarget.localRotation, controller.leftIKTargetStartRotation, 0.3f);

                if (controller.rightIKTarget)
                    controller.rightIKTarget.localRotation = Quaternion.Slerp(controller.rightIKTarget.localRotation, controller.rightIKTargetStartRotation, 0.3f);
            }
            if (bow.shotStrength > 0.5f && !bow.hasFiredArrow)
            {
                FireArrow();
                bow.hasFiredArrow = true;
            }
        }
    }
    public void FireArrow()
    {
        bow.spawnedBow.arrowModel.SetActive(false);
        Rigidbody spawnedArrow = Instantiate(bow.arrowToShoot, bow.spawnedBow.transform.position, bow.spawnedBow.transform.rotation).GetComponent<Rigidbody>();
        spawnedArrow.AddForce(bow.spawnedBow.arrowModel.transform.forward * (bow.shotStrength * 1000  * bow.arrowStrengthMultiplier), ForceMode.Force);
        Destroy(spawnedArrow.gameObject, 3);
    }
    public void HandsReachedTarget()
    {
        bow.handsReachedTarget = true;
    }
    public void HandsReachedControllers()
    {
        bow.handsReachedTarget = false;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.25f);
        Gizmos.DrawSphere(controller.rightController.transform.position - (controller.rightController.rotation * bow.grabPoint), bow.stringGrabRadius);
        Gizmos.DrawSphere(controller.leftController.transform.position - (controller.leftController.rotation * new Vector3(-bow.grabPoint.x, bow.grabPoint.y, bow.grabPoint.z)), bow.stringGrabRadius);
    }
}
