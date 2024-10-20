using System.Collections;
using System.Collections.Generic;
using RootMotion.Demos;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms.Impl;
using static GameManager;

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
    public GripManager gripManager;
    public Animator[] leftAnimators;
    public Animator[] rightAnimators;
    [System.Serializable]
    public class Game
    {
        public GameObject gameUIEnglish;
        public GameObject weaponMenuEnglish;
        public GameObject gameUIIrish;
        public GameObject weaponMenuIrish;
        public bool leftHanded = false;
        public enum Language { english, irish }
        public Language language;
        public GameObject englishUI;
        public GameObject irishUI;
        public Transform[] spawnPoints;
        public PotentialAnswer[] potentialAnswer;
        public AudioClip potentialAnswerSound;
        public TextMeshProUGUI questionText, scoreText;
        public int questionsToAnswer;
        public float answersPerSecond;
        public float oddsOfAnswer;
        public float potentialAnswerDragMultiplier;
        [HideInInspector]
        public int score;
        [HideInInspector]
        public int answer;
        [HideInInspector]
        public bool correctAnswerHit = false;
        public AudioClip correctAnswerSound;
        public AudioClip incorrectAnswerSound;
        public AudioSource[] gameMusic;
        public AudioSource lobbyMusic;
        public float musicFadeTime;
    }
    public Game game;
    [System.Serializable]
    public class Bow
    {
        [HideInInspector]
        public bool leftGrabbing, rightGrabbing;
        [HideInInspector]
        public Quaternion initialOffset;
        public float stringGrabRadius;
        public Vector3 grabPoint;
        public AudioClip grabSound;
        public GameObject bowItem;
        public GameObject arrowToShoot;
        public float arrowStrengthMultiplier;
        public Vector3 leftBowPositionOffset, leftBowRotationOffset, rightBowPositionOffset, rightBowRotationOffset;
        [HideInInspector]
        public BowSettings spawnedBow;
        [HideInInspector]
        public bool leftGrabbingString, rightGrabbingString;
        [HideInInspector]
        public bool canGrabString = true;
        public float stringDisconnectThreshold;
        public AudioClip bowReleaseSound;
        public Vector3 stringLeftGrabPositionOffset, stringRightGrabPositionOffset, stringLeftGrabRotationOffset, stringRightGrabRotationOffset;
        public InputActionProperty leftGrabInput, rightGrabInput;
        [HideInInspector]
        public bool handsReachedTarget = false, handsReachingTarget = false, offsetSet = false, hasFiredArrow = false;
        [HideInInspector]
        public Vector3 stringGrabOffset;
        public float stringDistanceTarget;
        [HideInInspector]
        public float shotStrength; 
        public Transform[] leftGrip;
        public Transform[] rightGrip;
    }
    public Bow bow;
    [System.Serializable]
    public class Gun
    {
        [HideInInspector]
        public bool leftGrabbing, rightGrabbing;
        public GameObject gunItem;
        public GameObject bulletToShoot;
        public Vector3 bulletShootPoint;
        public AudioClip fireSound;
        public bool canShoot = true;
        public float bulletStrengthMultiplier;
        public Vector3 leftPositionOffset, leftRotationOffset, rightPositionOffset, rightRotationOffset;
        [HideInInspector]
        public GameObject spawnedGun;
        public InputActionProperty leftShootInput, rightShootInput;
        public Transform[] leftGrip;
        public Transform[] rightGrip;
    }
    public Gun gun;
    public enum GameModeSelection { additionAndSubtraction, multiplication, division }
    private GameModeSelection gameMode;
    public enum ChangeableGameValues { spawnRate, questions, speed }
    private GameModeSelection changeableGameValues;
    public enum WeaponChosen { bow, gun, shuriken }
    private WeaponChosen weaponChosen;
    private void Start()
    {
        if(game.language == Game.Language.english)
            game.englishUI.SetActive(true);
        else
            game.irishUI.SetActive(true);
    }
    public void StartGame()
    {
        bow.canGrabString = true;
        controller.leftHandControllerMesh.SetActive(false);
        controller.rayOffsetLeft.localPosition = controller.rayCharacterLeftHandOffset;
        controller.rightHandControllerMesh.SetActive(false);
        controller.rayOffsetRight.localPosition = controller.rayCharacterRightHandOffset;
        playerMesh.SetActive(true);
        VRIKCalibration.Calibrate();

        controller.leftIKTarget = GameObject.Find("Left Hand IK Target").transform;
        controller.leftIKTargetStartPosition = controller.leftIKTarget.localPosition;
        controller.leftIKTargetStartRotation = controller.leftIKTarget.localRotation;

        controller.rightIKTarget = GameObject.Find("Right Hand IK Target").transform;
        controller.rightIKTargetStartPosition = controller.rightIKTarget.localPosition;
        controller.rightIKTargetStartRotation = controller.rightIKTarget.localRotation;
    }
    public void ChangeValues(ChangeableGameValues valueToChange, double value, TextMeshProUGUI text)
    {
        switch(valueToChange)
        {
            case ChangeableGameValues.spawnRate:
                if (game.answersPerSecond + (float)value > 0)
                {
                    game.answersPerSecond += (float)value;
                    text.text = game.answersPerSecond.ToString("0.0");
                }
                break;

            case ChangeableGameValues.questions:
                if(game.questionsToAnswer + (int)value > 0)
                {
                    game.questionsToAnswer += (int)value;
                    text.text = game.questionsToAnswer.ToString();
                }
                break;

            case ChangeableGameValues.speed:
                if (game.potentialAnswerDragMultiplier + (int)value > 0.3f && game.potentialAnswerDragMultiplier + (int)value < 5)
                {
                    game.potentialAnswerDragMultiplier += (float)value;
                    text.text = (float.Parse(text.text) + (float)value).ToString("0.0");
                }
                break;
        }
    }
    public void HandPicked(bool isLeftHanded)
    {
        if (isLeftHanded)
            game.leftHanded = true;
    }
    public void PickWeapon(string weaponChosenString)
    {
        switch (weaponChosenString)
        {
            case "Bow":
                weaponChosen = WeaponChosen.bow;
                break;

            case "Gun":
                weaponChosen = WeaponChosen.gun;
                break;

            case "Shuriken":
                weaponChosen = WeaponChosen.shuriken;
                break;
        }
    }
    public void PickGameMode(string gameModeChosen)
    {
        switch (gameModeChosen)
        {
            case "Addition & Subtraction":
                StartCoroutine(RunGame(gameModeChosen));
                break;

            case "Multiplication":
                StartCoroutine(RunGame(gameModeChosen));
                break;

            case "Division":
                StartCoroutine(RunGame(gameModeChosen));
                break;
        }
        switch(weaponChosen)
        {
            case WeaponChosen.bow:
                if (!bow.spawnedBow)
                {
                    if (game.leftHanded)
                    {
                        bow.spawnedBow = Instantiate(bow.bowItem, controller.rightIKTarget).GetComponent<BowSettings>();
                        bow.spawnedBow.transform.localPosition = bow.leftBowPositionOffset;
                        bow.spawnedBow.transform.localRotation = Quaternion.Euler(bow.leftBowRotationOffset);
                        bow.leftGrabbing = true;
                        gripManager.handBonesTarget = bow.rightGrip;
                        gripManager.handAnimators = rightAnimators;
                        gripManager.GripWithRight(false);
                    }
                    else
                    {
                        bow.spawnedBow = Instantiate(bow.bowItem, controller.leftIKTarget).GetComponent<BowSettings>();
                        bow.spawnedBow.transform.localPosition = bow.rightBowPositionOffset;
                        bow.spawnedBow.transform.localRotation = Quaternion.Euler(bow.rightBowRotationOffset);
                        bow.rightGrabbing = true;
                        gripManager.handBonesTarget = bow.leftGrip;
                        gripManager.handAnimators = leftAnimators;
                        gripManager.GripWithLeft(false);
                    }
                }
                break;
            case WeaponChosen.gun:
                if (!gun.spawnedGun)
                {
                    if (game.leftHanded)
                    {
                        gun.spawnedGun = Instantiate(gun.gunItem, controller.leftIKTarget);
                        gun.spawnedGun.transform.localPosition = gun.leftPositionOffset;
                        gun.spawnedGun.transform.localRotation = Quaternion.Euler(gun.leftRotationOffset);
                        gun.leftGrabbing = true;
                        gripManager.handBonesTarget = gun.leftGrip;
                        gripManager.handAnimators= leftAnimators;
                        gripManager.GripWithLeft(false);
                    }
                    else
                    {
                        gun.spawnedGun = Instantiate(gun.gunItem, controller.rightIKTarget);
                        gun.spawnedGun.transform.localPosition = gun.rightPositionOffset;
                        gun.spawnedGun.transform.localRotation = Quaternion.Euler(gun.rightRotationOffset);
                        gun.rightGrabbing = true;
                        gripManager.handBonesTarget = gun.rightGrip;
                        gripManager.handAnimators = rightAnimators;
                        gripManager.GripWithRight(false);
                    }
                }
                break;
            case WeaponChosen.shuriken:
                if (!bow.spawnedBow)
                {
                    if (game.leftHanded)
                    {
                        bow.spawnedBow = Instantiate(bow.bowItem, controller.rightIKTarget).GetComponent<BowSettings>();
                        bow.spawnedBow.transform.localPosition = bow.leftBowPositionOffset;
                        bow.spawnedBow.transform.localRotation = Quaternion.Euler(bow.leftBowRotationOffset);
                        bow.leftGrabbing = true;
                    }
                    else
                    {
                        bow.spawnedBow = Instantiate(bow.bowItem, controller.leftIKTarget).GetComponent<BowSettings>();
                        bow.spawnedBow.transform.localPosition = bow.rightBowPositionOffset;
                        bow.spawnedBow.transform.localRotation = Quaternion.Euler(bow.rightBowRotationOffset);
                        bow.rightGrabbing = true;
                    }
                }
                break;
        }
    }
    IEnumerator RunGame(string gameModeChosen)
    {
        float timer = 0;
        GenerateAnswer(gameModeChosen);
        game.score = 0;
        int randomMusic = Random.Range(0, game.gameMusic.Length - 1);
        StartCoroutine(FadeMusic(game.gameMusic[randomMusic], game.lobbyMusic, 1, 0.3f));
        while (game.score < game.questionsToAnswer)
        {
            timer += Time.deltaTime;
            if(timer > 1 / game.answersPerSecond)
            {
                int randomColor = Random.Range(0, game.potentialAnswer.Length - 1);
                int randomSpawnPoint = Random.Range(0, game.spawnPoints.Length - 1);
                PotentialAnswer spawnedPotentialAnswer = Instantiate(game.potentialAnswer[randomColor], game.spawnPoints[randomSpawnPoint].position, Quaternion.LookRotation(game.spawnPoints[randomSpawnPoint].position - Vector3.zero, Vector3.up)).GetComponent<PotentialAnswer>();
                Destroy(spawnedPotentialAnswer.gameObject, 10);
                AudioSource.PlayClipAtPoint(game.potentialAnswerSound, game.spawnPoints[randomSpawnPoint].position, 0.25f);
                spawnedPotentialAnswer.body.drag = 6 / game.potentialAnswerDragMultiplier;
                spawnedPotentialAnswer.body.AddForce(Vector3.up * 90 * Mathf.Pow(spawnedPotentialAnswer.body.drag, 0.8f), ForceMode.Force);

                bool isAnswer = Random.Range(1, (int)(1 / game.oddsOfAnswer) + 1) == 1;
                if (isAnswer)
                {
                    spawnedPotentialAnswer.isAnswer = true;
                    spawnedPotentialAnswer.numberText.text = game.answer.ToString();
                }
                else
                {
                    if(gameModeChosen != "Division")
                    {
                        int number = Random.Range(1, 101);
                        if (number == game.answer)
                            spawnedPotentialAnswer.isAnswer = true;
                        spawnedPotentialAnswer.numberText.text = number.ToString();
                    }
                    else
                    {
                        int number = Random.Range(1, 31);
                        if(number == game.answer)
                            spawnedPotentialAnswer.isAnswer = true;
                        spawnedPotentialAnswer.numberText.text = number.ToString();
                    }
                }
                timer = 0;
            }
            if(game.correctAnswerHit == true)
            {
                GenerateAnswer(gameModeChosen);
                game.correctAnswerHit = false;
                game.score++;
            }
            if(game.language == Game.Language.english)
                game.scoreText.text = $"Score: {game.score}";
            else
                game.scoreText.text = $"Scór: {game.score}";
            yield return null;
        }
        StartCoroutine(FadeMusic(game.lobbyMusic, game.gameMusic[randomMusic], 0.3f, 1));
        switch (weaponChosen)
        {
            case WeaponChosen.bow:
                if (game.leftHanded)
                {
                    bow.leftGrabbing = false;
                    gripManager.handAnimators = rightAnimators;
                    gripManager.GripWithRight(true);
                }
                else
                {
                    bow.rightGrabbing = false;
                    gripManager.handAnimators = leftAnimators;
                    gripManager.GripWithLeft(true);
                }
                Destroy(bow.spawnedBow);
                break;

            case WeaponChosen.gun:
                if (game.leftHanded)
                {
                    gun.leftGrabbing = false;
                    gripManager.handAnimators = leftAnimators;
                    gripManager.GripWithLeft(true);
                }
                else
                {
                    gun.rightGrabbing = false;
                    gripManager.handAnimators = rightAnimators;
                    gripManager.GripWithRight(true);
                }
                Destroy(gun.spawnedGun);
                break;

            case WeaponChosen.shuriken:
                //Destroy(shuriken.spawnedShuriken);
                break;
        }
        if (game.language == Game.Language.english)
        {
            game.gameUIEnglish.SetActive(false);
            game.weaponMenuEnglish.SetActive(true);
        }
        else
        {
            game.gameUIIrish.SetActive(false);
            game.weaponMenuIrish.SetActive(true);
        }
        yield return null;
    }
    IEnumerator FadeMusic(AudioSource fadeIn, AudioSource fadeOut, float volumeFadeOut, float volumeFadeIn)
    {
        float timer = 0;
        while (timer < game.musicFadeTime)
        {
            timer += Time.deltaTime;
            fadeOut.volume = Mathf.Lerp(volumeFadeOut, 0, timer / game.musicFadeTime);
            yield return null;
        }
        fadeOut.Stop();
        fadeOut.volume = volumeFadeOut;
        fadeIn.Play();
        fadeIn.volume = 0;
        timer = 0;
        while (timer < game.musicFadeTime)
        {
            timer += Time.deltaTime;
            fadeIn.volume = Mathf.Lerp(0, volumeFadeIn, timer / game.musicFadeTime);
            yield return null;
        }
        yield return null;
    }
    void GenerateAnswer(string gameModeChosen)
    {
        int firstNumberInSum = 0;
        switch (gameModeChosen)
        {
            case "Addition & Subtraction":
                if (Random.Range(1, 3) == 1)
                {
                    game.answer = Random.Range(1, 101);
                    firstNumberInSum = Random.Range(1, game.answer);
                    game.questionText.text = $"{firstNumberInSum} + {game.answer - firstNumberInSum}";
                }
                else
                {
                    game.answer = Random.Range(1, 101);
                    firstNumberInSum = Random.Range(game.answer + 1, 101);
                    game.questionText.text = $"{firstNumberInSum} - {firstNumberInSum - game.answer}";
                }
                break;

            case "Multiplication":
                while (true)
                {
                    game.answer = Random.Range(1, 101);
                    List<int> factors = new List<int>();
                    for (int i = 1; i < game.answer; i++)
                    {
                        if ((double)((float)game.answer / (float)i) % 1 == 0)
                        {
                            factors.Add(i);
                        }
                    }
                    if (factors.Count > 1)
                    {
                        int randomPoint = Random.Range(1, factors.Count - 1);
                        game.questionText.text = $"{factors[randomPoint]} x {game.answer / factors[randomPoint]}";
                        break;
                    }
                }
                break;

            case "Division":
                while (true)
                {
                    game.answer = Random.Range(1, 33);
                    List<int> factors = new List<int>();
                    for (int i = 1; i < game.answer; i++)
                    {
                        if ((double)((float)game.answer / (float)i) % 1 == 0)
                        {
                            factors.Add(i);
                        }
                    }
                    if (factors.Count > 1)
                    {
                        int randomPoint = Random.Range(1, factors.Count - 1);
                        game.questionText.text = $"{game.answer * factors[randomPoint]} / {factors[randomPoint]}";
                        break;
                    }
                }
                break;
        }
    }
    void CanShoot()
    {
        gun.canShoot = true;
    }
    private void Update()
    {
        if(gun.leftGrabbing && gun.canShoot)
        {
            bool triggered = gun.leftShootInput.action.WasPressedThisFrame();
            if (triggered)
            {
                Rigidbody spawnedBullet = Instantiate(gun.bulletToShoot).GetComponent<Rigidbody>();
                spawnedBullet.transform.position = gun.spawnedGun.transform.TransformPoint(gun.bulletShootPoint);
                spawnedBullet.transform.rotation = gun.spawnedGun.transform.rotation;
                spawnedBullet.AddForce(gun.spawnedGun.transform.right * gun.bulletStrengthMultiplier, ForceMode.Force);
                gun.canShoot = false;
                Destroy(spawnedBullet.gameObject, 5);
                Invoke(nameof(CanShoot), 0.25f);
                AudioSource.PlayClipAtPoint(gun.fireSound, spawnedBullet.transform.position, 0.05f);
            }
        }
        else if (gun.canShoot && gun.rightGrabbing)
        {
            bool triggered = gun.rightShootInput.action.WasPressedThisFrame();
            if (triggered)
            {
                Rigidbody spawnedBullet = Instantiate(gun.bulletToShoot).GetComponent<Rigidbody>();
                spawnedBullet.transform.position = gun.spawnedGun.transform.TransformPoint(gun.bulletShootPoint);
                spawnedBullet.transform.rotation = gun.spawnedGun.transform.rotation;
                spawnedBullet.AddForce(gun.spawnedGun.transform.right * gun.bulletStrengthMultiplier, ForceMode.Force);
                gun.canShoot = false;
                Destroy(spawnedBullet.gameObject, 5);
                Invoke(nameof(CanShoot), 0.25f);
                AudioSource.PlayClipAtPoint(gun.fireSound, spawnedBullet.transform.position, 0.05f);
            }
        }
        if(bow.canGrabString)
        {
            if (Physics.CheckSphere(controller.rightController.transform.position - (controller.rightController.rotation * bow.grabPoint), bow.stringGrabRadius) || Physics.CheckSphere(controller.leftController.transform.position - (controller.leftController.rotation * new Vector3(-bow.grabPoint.x, bow.grabPoint.y, bow.grabPoint.z)), bow.stringGrabRadius))
            {
                Collider[] colliderRight = Physics.OverlapSphere(controller.rightController.transform.position - (controller.rightController.rotation * bow.grabPoint), bow.stringGrabRadius);
                if (colliderRight.Length == 0)
                {
                    Collider[] colliderLeft = Physics.OverlapSphere(controller.leftController.transform.position - (controller.leftController.rotation * new Vector3(-bow.grabPoint.x, bow.grabPoint.y, bow.grabPoint.z)), bow.stringGrabRadius);
                    if (colliderLeft.Length > 0)
                    {
                        if (colliderLeft[0])
                        {
                            if (colliderLeft[0].name != "StringGrabPoint")
                                colliderLeft = null;
                            else if (bow.leftGrabInput.action.ReadValue<float>() > 0.25f)
                            {
                                if (!bow.leftGrabbingString)
                                    AudioSource.PlayClipAtPoint(bow.grabSound, controller.leftController.transform.position - (controller.leftController.rotation * bow.grabPoint), 0.15f);
                                bow.leftGrabbingString = true;
                                bow.initialOffset = Quaternion.Inverse(Quaternion.LookRotation(controller.leftController.position - controller.rightController.position, controller.rightController.up) * Quaternion.Euler(0, 90, -90)) * controller.rightIKTarget.rotation;
                            }
                        }
                    }
                }
                else if (colliderRight[0])
                {
                    if (colliderRight[0].name != "StringGrabPoint")
                        colliderRight = null;
                    if (bow.rightGrabInput.action.ReadValue<float>() > 0.25f)
                    {
                        if (!bow.rightGrabbingString)
                            AudioSource.PlayClipAtPoint(bow.grabSound, controller.rightController.transform.position - (controller.rightController.rotation * bow.grabPoint), 0.15f);
                        bow.rightGrabbingString = true;
                        bow.initialOffset = Quaternion.Inverse(Quaternion.LookRotation(controller.rightController.position - controller.leftController.position, controller.leftController.up) * Quaternion.Euler(0, -90, 90)) * controller.leftIKTarget.rotation;
                    }
                }
            }
        }
        if (bow.rightGrabbingString)
        {
            bow.hasFiredArrow = false;
            bow.spawnedBow.arrowModel.SetActive(true);
            controller.leftIKTarget.rotation = Quaternion.Slerp(controller.leftIKTarget.rotation, Quaternion.LookRotation(controller.rightController.position - controller.leftController.position, controller.leftController.up) * Quaternion.Euler(0, -90, 90), 0.7f);
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
            Vector3 handInBowSpace = bow.spawnedBow.stringGrabPoint.transform.TransformPoint(bow.spawnedBow.stringGrabPoint.transform.rotation * (bow.spawnedBow.stringGrabPoint.transform.rotation * new Vector3(0, bow.spawnedBow.stringGrabPoint.transform.localPosition.y, Mathf.Clamp(bow.spawnedBow.stringGrabPoint.transform.InverseTransformPoint(controller.rightController.position).z, -bow.stringDistanceTarget, float.PositiveInfinity))));
            if (!bow.offsetSet)
            {
                bow.stringGrabOffset = handInBowSpace - bow.spawnedBow.stringGrabPoint.transform.position;
                bow.offsetSet = true;
            }
            Vector3 targetInBowSpace = bow.spawnedBow.stringGrabPoint.transform.TransformPoint(bow.spawnedBow.stringGrabPoint.transform.rotation * bow.spawnedBow.stringGrabPoint.transform.rotation * new Vector3(0, bow.spawnedBow.stringGrabPoint.transform.localPosition.y, -bow.stringDistanceTarget));
            float pullBackValue = Mathf.Clamp(Vector3.Distance(handInBowSpace.normalized, targetInBowSpace.normalized) / -bow.stringDistanceTarget + 1, 0, 1);
            bow.spawnedBow.GetComponent<Animator>().SetFloat("PullBack", pullBackValue);
            bow.shotStrength = pullBackValue;
        }
        else if (bow.leftGrabbingString)
        {
            bow.hasFiredArrow = false;
            bow.spawnedBow.arrowModel.SetActive(true);
            controller.rightIKTarget.rotation = Quaternion.Slerp(controller.rightIKTarget.rotation, Quaternion.LookRotation(controller.leftController.position - controller.rightController.position, controller.rightController.up) * Quaternion.Euler(0, 90, -90), 0.7f);
            if (!bow.handsReachingTarget)
            {
                Invoke(nameof(HandsReachedTarget), 0.1f);
                bow.handsReachingTarget = true;
            }
            if (!bow.handsReachedTarget)
            {
                controller.leftIKTarget.position = Vector3.Lerp(controller.leftIKTarget.position, bow.spawnedBow.stringArmaturePiece.transform.position - (bow.spawnedBow.stringArmaturePiece.transform.rotation * bow.stringLeftGrabPositionOffset), 0.3f);
                controller.leftIKTarget.rotation = Quaternion.Slerp(controller.leftIKTarget.rotation, bow.spawnedBow.stringArmaturePiece.transform.rotation * Quaternion.Euler(bow.stringLeftGrabRotationOffset), 0.3f);
            }
            else
            {
                controller.leftIKTarget.position = bow.spawnedBow.stringArmaturePiece.transform.position - (bow.spawnedBow.stringArmaturePiece.transform.rotation * bow.stringLeftGrabPositionOffset);
                controller.leftIKTarget.rotation = bow.spawnedBow.stringArmaturePiece.transform.rotation * Quaternion.Euler(bow.stringLeftGrabRotationOffset);
            }
            Vector3 handInBowSpace = bow.spawnedBow.stringGrabPoint.transform.TransformPoint(bow.spawnedBow.stringGrabPoint.transform.rotation * (bow.spawnedBow.stringGrabPoint.transform.rotation * new Vector3(0, bow.spawnedBow.stringGrabPoint.transform.localPosition.y, Mathf.Clamp(bow.spawnedBow.stringGrabPoint.transform.InverseTransformPoint(controller.leftController.position).z, -bow.stringDistanceTarget, float.PositiveInfinity))));
            if (!bow.offsetSet)
            {
                bow.stringGrabOffset = handInBowSpace - bow.spawnedBow.stringGrabPoint.transform.position;
                bow.offsetSet = true;
            }
            Vector3 targetInBowSpace = bow.spawnedBow.stringGrabPoint.transform.TransformPoint(bow.spawnedBow.stringGrabPoint.transform.rotation * bow.spawnedBow.stringGrabPoint.transform.rotation * new Vector3(0, bow.spawnedBow.stringGrabPoint.transform.localPosition.y, -bow.stringDistanceTarget));
            float pullBackValue = Mathf.Clamp(Vector3.Distance(handInBowSpace.normalized, targetInBowSpace.normalized) / -bow.stringDistanceTarget + 1, 0, 1);
            bow.spawnedBow.GetComponent<Animator>().SetFloat("PullBack", pullBackValue);
            bow.shotStrength = pullBackValue;
        }
        if ((bow.rightGrabInput.action.ReadValue<float>() < 0.25f || Vector3.Distance(controller.leftIKTarget.position, controller.leftHandBonePoint.position) > bow.stringDisconnectThreshold) && !bow.leftGrabbingString)
        {
            if (bow.shotStrength > 0.5f && !bow.hasFiredArrow)
            {
                FireArrow();
                bow.hasFiredArrow = true;
                bow.canGrabString = false;
                Invoke(nameof(CanGrabString), 0.1f);
            }
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
                controller.rightIKTarget.rotation = controller.rightIKTargetStartRotation;
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
        }

        if ((bow.leftGrabInput.action.ReadValue<float>() < 0.25f || Vector3.Distance(controller.leftIKTarget.position, controller.leftHandBonePoint.position) > bow.stringDisconnectThreshold) && !bow.rightGrabbingString)
        {
            if (bow.shotStrength > 0.5f && !bow.hasFiredArrow)
            {
                FireArrow();
                bow.hasFiredArrow = true;
                bow.canGrabString = false;
                Invoke(nameof(CanGrabString), 0.1f);
            }
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
                controller.leftIKTarget.rotation = controller.leftIKTargetStartRotation;
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
        }
    }
    void CanGrabString()
    {
        bow.canGrabString = true;
    }
    public void FireArrow()
    {
        bow.spawnedBow.arrowModel.SetActive(false);
        Rigidbody spawnedArrow = Instantiate(bow.arrowToShoot, bow.spawnedBow.transform.position, bow.spawnedBow.transform.rotation).GetComponent<Rigidbody>();
        spawnedArrow.AddForce(bow.spawnedBow.arrowModel.transform.forward * (bow.shotStrength * 1000  * bow.arrowStrengthMultiplier), ForceMode.Force);
        Destroy(spawnedArrow.gameObject, 3);
        AudioSource.PlayClipAtPoint(bow.bowReleaseSound, spawnedArrow.position, 1.25f);
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
