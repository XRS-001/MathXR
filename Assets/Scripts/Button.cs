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
    public SpriteRenderer buttonRenderer;
    public UnityEvent pressEvent;
    public AudioClip pressSound;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void HoverButton()
    {
        if(buttonImage != null)
            buttonImage.CrossFadeAlpha(0.5f, 0.15f, false);
        else
            StartCoroutine(RendererCrossFadeAlpha(0.15f, 0.5f, true));
    }
    IEnumerator RendererCrossFadeAlpha(float time, float alpha, bool hovering)
    {
        Color color = buttonRenderer.color;
        float timer = 0;
        while (timer < time)
        {
            if (hovering)
                color.a = Mathf.Lerp(1, alpha, timer / time);
            else
                color.a = Mathf.Lerp(alpha, 1, timer / time);

            timer += Time.deltaTime;
            buttonRenderer.color = color;
            yield return null;
        }
        yield return null;
    }
    public void UnHoverButton()
    {
        if (buttonImage != null)
            buttonImage.CrossFadeAlpha(1, 0.15f, false);
        else
            if (isActiveAndEnabled)
                StartCoroutine(RendererCrossFadeAlpha(0.15f, 0.5f, false));
    }
    public void PressButton()
    {
        pressEvent.Invoke();
        if(pressSound)
            gameManager.GetComponent<AudioSource>().PlayOneShot(pressSound);
    }
}
