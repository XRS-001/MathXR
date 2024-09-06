using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Button : MonoBehaviour
{
    private GameManager gameManager;
    public Image buttonImage;
    public UnityEvent pressEvent;
    public AudioClip pressSound;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void HoverButton()
    {
        buttonImage.CrossFadeAlpha(0.5f, 0.15f, false);
    }
    public void UnHoverButton()
    {
        buttonImage.CrossFadeAlpha(1, 0.15f, false);
    }
    [ContextMenu("Call PressButton")]
    public void PressButton()
    {
        pressEvent.Invoke();
        if(pressSound)
            gameManager.GetComponent<AudioSource>().PlayOneShot(pressSound);
    }
}
