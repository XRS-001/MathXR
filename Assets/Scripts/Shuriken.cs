using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuriken : MonoBehaviour
{
    public bool canCollide;
    private void OnCollisionEnter(Collision collision)
    {
        Invoke(nameof(DelayCanCollide), 0.1f);
    }
    void DelayCanCollide()
    {
        canCollide = false;
    }
}
