using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
[System.Serializable]
public class IndexFingerBones
{
    public Transform index1;
    public Transform index2;
    public Transform index3;
}
public class HandAnimator : MonoBehaviour
{
    public GameManager gameManager;
    public bool isLeftHand;
    [Header("Input")]
    public InputActionProperty trigger;
    public InputActionProperty triggerTouch;
    public InputActionProperty grip;
    public InputActionProperty thumb;

    [Header("Animation")]
    public Animator handAnimator;
    public Animator thumbAnimator;

    private float triggeredValue;
    private bool thumbTouched = false;
    private bool triggerTouched;

    //private GrabPhysics grabPhysics;
    [Header("Index Finger")]
    public IndexFingerBones indexFingerBones;
    Quaternion originalIndex1Rotation;
    Quaternion originalIndex2Rotation;
    Quaternion originalIndex3Rotation;
    public IndexFingerBones grabbedIndexFingerBones;
    private void Start()
    {
        originalIndex1Rotation = indexFingerBones.index1.localRotation;
        originalIndex2Rotation = indexFingerBones.index2.localRotation;
        originalIndex3Rotation = indexFingerBones.index3.localRotation;
    }
    // Update is called once per frame
    void Update()
    {
        FingerInputs();
    }
    void FingerInputs()
    {
        //trigger
        float triggerValue = trigger.action.ReadValue<float>();
        float triggerTouchValue = triggerTouch.action.ReadValue<float>();

        if (triggerTouchValue > 0f && !triggerTouched) { StartCoroutine(TriggerTouch()); }

        else if (triggerTouchValue > 0f && triggerTouched) { triggeredValue = 0.5f; }

        else if (triggerTouched) { StartCoroutine(TriggerUnTouch()); }

        if (!triggerTouched && triggerTouchValue == 0f) { triggeredValue = 0f; }

        if (triggerValue > 0f) { triggeredValue = Mathf.Lerp(0.5f, 1f, triggerValue); }

        //thumb
        float thumbTouch = thumb.action.ReadValue<float>();
        if (thumbTouch > 0f)
        {
            thumbTouched = true;
            thumbAnimator.Play("Thumb", 0);
        }
        else if (thumbTouched)
        {
            thumbTouched = false;
            thumbAnimator.Play("ThumbReverse", 0);
        }
        if (isLeftHand && gameManager.bow.leftGrabbingString || !isLeftHand && gameManager.bow.rightGrabbingString)
        {
            handAnimator.SetFloat("Trigger", 1);
            handAnimator.SetFloat("Grip", 1);
        }
        else
        {
            handAnimator.SetFloat("Trigger", triggeredValue);
            float gripValue = grip.action.ReadValue<float>();
            handAnimator.SetFloat("Grip", gripValue);
        }
    }

    //smoothly transition the blend value of the trigger
    public IEnumerator TriggerTouch()
    {
        float timer = 0;

        while (timer < 0.1f)
        {
            float triggerTouchValue = triggerTouch.action.ReadValue<float>();
            if (triggerTouchValue > 0f && triggerTouchValue < 0.5f)
            {
                triggeredValue = Mathf.Lerp(0, 0.5f, timer / 0.07f);
                handAnimator.SetFloat("Trigger", triggeredValue);
            }
            else
            {
                break;
            }
            timer += Time.deltaTime;
            yield return null;
        }
        triggerTouched = true;
    }
    public IEnumerator TriggerUnTouch()
    {
        float timer = 0;

        while (timer < 0.1f)
        {
            float triggerTouchValue = triggerTouch.action.ReadValue<float>();
            if (triggerTouchValue == 0f)
            {
                triggeredValue = Mathf.Lerp(0.5f, 0, timer / 0.07f);
                handAnimator.SetFloat("Trigger", triggeredValue);
            }
            else
            {
                break;
            }
            timer += Time.deltaTime;
            yield return null;
        }
        triggerTouched = false;
    }
}
