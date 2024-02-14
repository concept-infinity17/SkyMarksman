using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    private AudioSource source;
    public AudioClip explosionClip;
    
    // Skript dient dazu die Audiodatei nach dem Abspielen der Explosion zu zerstören
    void Start()
    {
        source = GetComponent<AudioSource>();

        if (source != null)
        {
           explosionClip  = source.clip;
        }
        
        Invoke(nameof(Death), explosionClip.length + 0.5f);
    }

    void Death()
    {
        Destroy(gameObject);
    }
   
}
