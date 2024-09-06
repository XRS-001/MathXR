using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GroupUIFadeOut : MonoBehaviour
{
    public Image image;
    public float fadeOutTime;
    public void Start()
    {
        image.CrossFadeAlpha(0, fadeOutTime, false);
    }
}
