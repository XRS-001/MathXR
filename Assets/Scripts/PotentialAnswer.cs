using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static GameManager;

public class PotentialAnswer : MonoBehaviour
{
    public TextMeshProUGUI numberText;
    public Rigidbody body;
    public bool isAnswer;
    private bool hasHit;

    private void OnCollisionEnter(Collision collision)
    {
        if(isAnswer && !hasHit)
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().game.correctAnswerHit = true;
            AudioSource.PlayClipAtPoint(GameObject.Find("GameManager").GetComponent<GameManager>().game.correctAnswerSound, transform.position, 2);
            hasHit = true;
        }
        else if (!hasHit)
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().game.score--;
            AudioSource.PlayClipAtPoint(GameObject.Find("GameManager").GetComponent<GameManager>().game.incorrectAnswerSound, transform.position, 2);
        }
    }
}
