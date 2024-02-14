using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BulletTarget : MonoBehaviour, HeneGames.IDamageable
{
    
    [SerializeField] private GameObject hitEffectPrefab; //This object is created at the hit point
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private GameObject explosionAudioSource;
    [SerializeField] private float health = 100f;
    

    //IDamageable interface
    public void TakeDamage(float _damageAmount, float _hitForce, Vector3 _hitPosition, Vector3 _hitNormal, out GameObject _hitEffect, out float _causedDamage)
    {
        //Tells the bullet what effect it will create on the hit point
        _hitEffect = hitEffectPrefab;

        //Tells the bullet how much damage it caused
        _causedDamage = _damageAmount;

        //If healt is above zero, subtract it from health
        if (health > 0f)
        {
            health -= _damageAmount;
        }

        //Check if healt is zero or below
        if(health <= 0f)
        {
            Death();
        }
    }

    private void Death()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
            
        }
        //spawn an empty gameObject with an audioSource attached that plays the explosion sound and destroys itself afterwards
        Instantiate(explosionAudioSource, transform.position, Quaternion.identity);
        
        //Here you can put your own logic, now it just destroys the object when it dies
        Destroy(gameObject);
    }
}