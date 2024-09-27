using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
            hasHit = true;
        }
    }
}
