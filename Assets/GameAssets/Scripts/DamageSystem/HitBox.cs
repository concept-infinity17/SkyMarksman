using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeneGames
{
    [RequireComponent(typeof(Collider))]
    public class HitBox : MonoBehaviour, IDamageable
    {
        [SerializeField] private float damageMultiplier = 1f;

        [SerializeField] private GameObject damageEffectPrefab;

        private float damageReceived;

        /// <summary>
        /// Each time when this is called it also resets the accumulated damage from the hitbox
        /// </summary>
        public bool HitBoxHasDamage(out float _receivedDamage)
        {
            if(damageReceived > 0f)
            {
                _receivedDamage = damageReceived;
                damageReceived = 0f;
                return true;
            }

            _receivedDamage = 0f;
            return false;
        }

        public void TakeDamage(float _damageAmount, float _hitForce, Vector3 _hitPosition, Vector3 _hitNormal, out GameObject _hitEffect, out float _damageCaused)
        {
            //Tells the bullet what effect it will create on the hit point
            _hitEffect = damageEffectPrefab;

            //Tells the bullet how much damage it caused
            _damageCaused = _damageAmount * damageMultiplier;

            //Caused damage stored to damageReceived variable
            damageReceived += _damageAmount * damageMultiplier;
        }

        public void SetEffectPrefab(GameObject _effectPrefab)
        {
            if (damageEffectPrefab == null)
            {
                damageEffectPrefab = _effectPrefab;
            }
        }

        public void ChangeEffectPrefab(GameObject _effectPrefab)
        {
            damageEffectPrefab = _effectPrefab;
        }
    }
}