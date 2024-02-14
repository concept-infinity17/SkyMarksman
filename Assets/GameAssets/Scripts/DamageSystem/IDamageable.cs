using UnityEngine;

namespace HeneGames
{
    public interface IDamageable
    {
        void TakeDamage(float _damageAmount, float _hitForce, Vector3 _hitPosition, Vector3 _hitNormal, out GameObject _hitEffect, out float _damageCaused);
    }
}