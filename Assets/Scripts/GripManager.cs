using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GripManager : MonoBehaviour
{
    public Transform[] handBonesLeft;
    private List<Quaternion> handBonesLeftRotationsStart = new List<Quaternion>();
    public Transform[] handBonesLeftOriginal;
    public Transform[] handBonesRight;
    private List<Quaternion> handBonesRightRotationsStart = new List<Quaternion>();
    public Transform[] handBonesRightOriginal;


    [HideInInspector]
    public Transform[] handBonesTarget;
    public float gripTime;
    [HideInInspector]
    public Animator[] handAnimators;

    public void GripWithLeft(bool grippingOut)
    {
        for (int i = 0; i < handBonesLeft.Length; i++)
        {
            handBonesLeftRotationsStart.Add(handBonesLeft[i].localRotation);
        }
        if (!grippingOut)
        {
            StartCoroutine(GripRoutine(handBonesLeft, handBonesLeftRotationsStart, handBonesTarget, grippingOut));
        }
        else
        {
            StartCoroutine(GripRoutine(handBonesLeft, handBonesLeftRotationsStart, handBonesLeftOriginal, grippingOut));
        }
    }
    public void GripWithRight(bool grippingOut)
    {
        for (int i = 0; i < handBonesRight.Length; i++)
        {
            handBonesRightRotationsStart.Add(handBonesRight[i].localRotation);
        }
        if(!grippingOut)
        {
            StartCoroutine(GripRoutine(handBonesRight, handBonesRightRotationsStart, handBonesTarget, grippingOut));
        }
        else
        {
            StartCoroutine(GripRoutine(handBonesRight, handBonesRightRotationsStart, handBonesRightOriginal, grippingOut));
        }
    }
    IEnumerator GripRoutine(Transform[] bones, List<Quaternion> startRotations, Transform[] targetRotation, bool grippingOut)
    {
        handAnimators[0].enabled = grippingOut;
        handAnimators[1].enabled = grippingOut;
        float timer = 0;
        while (timer < gripTime)
        {
            for(int i = 0; i < bones.Length; i++)
            {
                bones[i].localRotation = Quaternion.Slerp(startRotations[i], targetRotation[i].localRotation, timer / gripTime);
            }
            timer += Time.deltaTime;
            yield return null;
        }
        yield return null;
    }
}
