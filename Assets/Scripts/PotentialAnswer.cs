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
        if (!hasHit && collision.gameObject.layer == LayerMask.NameToLayer("Projectile"))
        {
            if (isAnswer)
            {
                GameObject.Find("GameManager").GetComponent<GameManager>().game.correctAnswerHit = true;
                AudioSource.PlayClipAtPoint(GameObject.Find("GameManager").GetComponent<GameManager>().game.correctAnswerSound, transform.position, 2);
                hasHit = true;
            }
            else
            {
                if (GameObject.Find("GameManager").GetComponent<GameManager>().game.score > 0)
                    GameObject.Find("GameManager").GetComponent<GameManager>().game.score--;
                AudioSource.PlayClipAtPoint(GameObject.Find("GameManager").GetComponent<GameManager>().game.incorrectAnswerSound, transform.position, 2);
            }
            body.AddForce(collision.relativeVelocity / 200, ForceMode.Impulse);
            body.drag = 2;
            body.angularDrag = 2;
        }
    }
}
