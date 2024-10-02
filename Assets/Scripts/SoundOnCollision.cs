using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundOnCollision : MonoBehaviour
{
    bool hasHit;
    bool canHit;
    public AudioClip hitSound;
    public float hitVolume;
    public float timeUntilCanHit;
    private void Start()
    {
        Invoke(nameof(WaitUntilCanHit), timeUntilCanHit);   
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!hasHit && canHit)
        {
            AudioSource.PlayClipAtPoint(hitSound, collision.GetContact(0).point, hitVolume);
            hasHit = true;
        }
    }
    void WaitUntilCanHit()
    {
        canHit = true;
    }
}
