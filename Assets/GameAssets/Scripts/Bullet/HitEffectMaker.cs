using UnityEngine;

namespace HeneGames
{
    public class HitEffectMaker : MonoBehaviour, IDamageable
    {
        [SerializeField] private GameObject hitEffect;

        public void TakeDamage(float _damageAmount, float _hitForce, Vector3 _hitPosition, Vector3 _hitNormal, out GameObject _hitEffect, out float _damageCaused)
        {
            _hitEffect = hitEffect;
            _damageCaused = 0f;
        }
    }
}