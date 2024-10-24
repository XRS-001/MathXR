using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GripManager : MonoBehaviour
{
    public Transform[] handBonesLeft;
    public Transform[] handBonesLeftOriginal;
    public Transform[] handBonesRight;
    public Transform[] handBonesRightOriginal;


    [HideInInspector]
    public Transform[] handBonesTarget;
    public float gripTime;
    [HideInInspector]
    public Animator[] handAnimators;

    public void GripWithLeft(bool grippingOut)
    {
        if (!grippingOut)
        {
            StartCoroutine(GripRoutine(handBonesLeft, handBonesLeftOriginal, handBonesTarget, grippingOut));
        }
        else
        {
            StartCoroutine(GripRoutine(handBonesLeft, handBonesTarget, handBonesLeftOriginal, grippingOut));
        }
    }
    public void GripWithRight(bool grippingOut)
    {
        if(!grippingOut)
        {
            StartCoroutine(GripRoutine(handBonesRight, handBonesRightOriginal, handBonesTarget, grippingOut));
        }
        else
        {
            StartCoroutine(GripRoutine(handBonesRight, handBonesTarget, handBonesRightOriginal, grippingOut));
        }
    }
    IEnumerator GripRoutine(Transform[] bones, Transform[] startRotations, Transform[] targetRotation, bool grippingOut)
    {
        handAnimators[0].enabled = grippingOut;
        handAnimators[1].enabled = grippingOut;
        float timer = 0;
        while (timer < gripTime)
        {
            for(int i = 0; i < bones.Length; i++)
            {
                bones[i].localRotation = Quaternion.Slerp(startRotations[i].localRotation, targetRotation[i].localRotation, timer / gripTime);
            }
            timer += Time.fixedDeltaTime;
            yield return null;
        }
        yield return null;
    }
}
