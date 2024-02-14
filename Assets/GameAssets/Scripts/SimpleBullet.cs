using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeneGames
{
    [RequireComponent(typeof(AudioSource))]
    public class SimpleBullet : MonoBehaviour
    {
        [Header("Debug bullet things")]
        [SerializeField] private bool debug;

        [Header("Bullet settings")]
        [SerializeField] private BulletHitMode bulletHitMode;
        [SerializeField] private LayerMask bulletsHitMask;

        [Tooltip("Defines random starting angle, larger value is less accurate")]
        [Range(0f, 25f)]
        [SerializeField] private float accuracy = 0f;

        [Tooltip("Determines how much damage the bullet does to the damage interface")]
        [Range(0f, 500f)]
        [SerializeField] private float damage = 200f;

        [Tooltip("Determines when the bullet reaches its maximum distance")]
        [Range(0f, 2500f)]
        [SerializeField] private float range = 1500f;

        [Tooltip("Determines what percentage of damage is lost when the bullet reaches its maximum distance")]
        [Range(0f, 100f)]
        [SerializeField] private float damageDropPercent = 0f;

        [Range(0f, 2500f)]
        [SerializeField] private float bulletSpeed = 500f;

        [Tooltip("This multiplier affects the hitforce variable")]
        [Range(1, 5)]
        [SerializeField] private int hitForceMultiplier = 1;

        [Tooltip("When the bullet hits an object with a rigid body component, the bullet gives this amount of force to the object")]
        [Range(0f, 5000f)]
        [SerializeField] private float hitForce = 100f;

        [Tooltip("How many seconds does it take after a collision to destroy this object")]
        [Range(0.5f, 10f)]
        [SerializeField] private float destoryWhenHitTime = 1f;

        [Tooltip("Once the bullet is created, one of these audios is played randomly. Pitch is also always a little different, so there can also be only one audio clip on the list and it’s not too repetitive")]
        [Header("Random sounds")]
        [SerializeField] private AudioClip[] muzzleFlashAudios;

        [Header("Bullet flying part object")]
        [SerializeField] private GameObject bulletMovingPart;

        [Header("Muzzle flash light (Not required)")]

        [Tooltip("Bigger value is faster")]
        [Range(0.01f, 0.5f)]
        [SerializeField] private float muzzleLightDimmingSpeed = 0.1f;

        [SerializeField] private Light muzzleLight;

        [Tooltip("When a bullet collides with an object in to specified layer, object defined for the layer is created")]
        [SerializeField] private List<BulletHitLayer> layerEffects = new List<BulletHitLayer>();

        private AudioSource audioSource;
        private LayerMask layerMask;
        private Vector3 bulletPreviousPos;
        private Vector3 bulletDistanceRayEndPoint;
        private Vector3 offsetRotation;
        private GameObject damageInterfaceEffect;
        private bool hitSometing;
        private bool destroyWhenSoundEnd;
        private float currentRange;
        private float bulletTravelTime;

        private enum BulletHitMode
        {
            BulletDestroyByHit__EffectNotStickToObject,
            BulletDestroyByHit__EffectStickToObject,
            BulledAndEffectStickToObject,
        }

        public void Start()
        {
            audioSource = GetComponent<AudioSource>();
            bulletPreviousPos = bulletMovingPart.transform.position;

            //Play sound
            PlayBulletSound();

            //Accuraty
            float _xRotRandom = Random.Range(-accuracy, accuracy);
            float _yRotRandom = Random.Range(-accuracy, accuracy);
            transform.rotation *= Quaternion.Euler(transform.localRotation.x + _xRotRandom, transform.localRotation.y + _yRotRandom, transform.localRotation.z);
        }

        public void FixedUpdate()
        {
            //Dimm muzzle light
            if (muzzleLight != null)
            {
                float _deltaFloat = Time.deltaTime * 100f;
                muzzleLight.intensity = Mathf.Lerp(muzzleLight.intensity, 0f, muzzleLightDimmingSpeed * _deltaFloat);
            }

            //Update bullet movement and other things
            if (bulletMovingPart != null)
            {
                BulletUpdate();
            }

            //Destroy bullet when sound end
            if (destroyWhenSoundEnd && !audioSource.isPlaying)
            {
                Destroy(gameObject);
            }
        }

        #region Can be modify easily

        private float CalculateDamageDrop(float _damage, float _damageDropPercent, float _hitDistance, float _maxRange)
        {
            float _damageDropPercent2 = _damageDropPercent * 0.01f;
            float _damageToLost = _damage * _damageDropPercent2;
            float _remainingPercent = _hitDistance / _maxRange;
            float _invertPercent = 1f - _remainingPercent;
            float _calculatedDamageLost = Mathf.Lerp(_damageToLost, 0f, _invertPercent);
            float _calculatedDamage = _damage - _calculatedDamageLost;

            return _calculatedDamage;
        }

        private void PlayBulletSound()
        {
            if (muzzleFlashAudios.Length == 0)
                return;

            float _randomPitch = Random.Range(0.8f, 1.2f);
            int _randomClipIndex = Random.Range(0, muzzleFlashAudios.Length);
            AudioClip _randomClip = muzzleFlashAudios[_randomClipIndex];

            audioSource.pitch = _randomPitch;
            audioSource.PlayOneShot(_randomClip);
        }

        private void RemoveBullet()
        {
            //Dont destroy bullet if sound is still playing
            if (!audioSource.isPlaying)
            {
                Destroy(gameObject);
            }
            else
            {
                destroyWhenSoundEnd = true;
            }
        }

        private void BulletHit()
        {

        }

        #endregion

        #region Variables

        public float CurrentAccuracy()
        {
            return accuracy;
        }

        public float CurrentDamage()
        {
            return damage;
        }

        public float CurrentRange()
        {
            return range;
        }

        public float CurrentDamageDropPercent()
        {
            return damageDropPercent;
        }

        public float CurrentSpeed()
        {
            return bulletSpeed;
        }

        public int CurrentHitForceMultipler()
        {
            return hitForceMultiplier;
        }

        public float CurrentHitForce()
        {
            return hitForce * (float)hitForceMultiplier;
        }

        public void ChangeAccuracy(float _newAccuracy)
        {
            accuracy = _newAccuracy;
        }

        public void ChangeDamage(float _newDamage)
        {
            accuracy = _newDamage;
        }

        public void ChangeSpeed(float _newSpeed)
        {
            bulletSpeed = _newSpeed;
        }

        public void ChangeHitForce(int _newForceMultipler, float _newHitForce)
        {
            hitForceMultiplier = _newForceMultipler;
            hitForce = _newHitForce;
        }

        #endregion

        #region Bullet functionality things

        private void BulletUpdate()
        {
            //Bullet previous position
            if (bulletMovingPart != null)
            {
                bulletPreviousPos = bulletMovingPart.transform.position;
            }

            //Bullet distance ray
            Ray bulletDistanceRay = new Ray(transform.position, transform.forward);
            RaycastHit bulletDistanceRayHitInfo;

            //Make ray which determines the direction of the bullet
            if (Physics.Raycast(bulletDistanceRay, out bulletDistanceRayHitInfo, range, layerMask))
            {
                //Debug
                Debug.DrawLine(bulletDistanceRay.origin, bulletDistanceRayHitInfo.point, Color.green);
                bulletDistanceRayEndPoint = bulletDistanceRayHitInfo.point;
            }
            else
            {
                //If out of distace draw red line
                bulletDistanceRayEndPoint = bulletDistanceRay.GetPoint(range);
                Debug.DrawLine(bulletDistanceRay.origin, bulletDistanceRayEndPoint, Color.red);
            }

            //Move bullet
            if (!hitSometing)
            {
                currentRange += Time.fixedDeltaTime * bulletSpeed;
                bulletMovingPart.transform.position = bulletDistanceRay.GetPoint(currentRange);
            }

            //Bullet reached the maximum distance and get destroyed
            if (currentRange > range && !hitSometing)
            {
                hitSometing = true;
                bulletMovingPart.transform.position = bulletDistanceRayEndPoint;
                bulletMovingPart.SetActive(false);
                RemoveBullet();
            }

            //Check if bullett hit someting
            RaycastHit[] bulletHits = Physics.RaycastAll(new Ray(bulletPreviousPos, (bulletMovingPart.transform.position - bulletPreviousPos).normalized), (bulletMovingPart.transform.position - bulletPreviousPos).magnitude, bulletsHitMask);

            //Bullet hit someting
            if (bulletHits.Length > 0)
            {
                int lastColliderIndex = ClosestColliderInt(bulletHits);

                //Take damage
                if (bulletHits[lastColliderIndex].collider.GetComponent<IDamageable>() != null && !hitSometing)
                {
                    //Move bullet to hit point
                    bulletMovingPart.transform.position = bulletHits[lastColliderIndex].point;

                    //Set hit distance
                    float _hitDistance = Vector3.Distance(transform.position, bulletMovingPart.transform.position);

                    //Calculate damage drop
                    damage = CalculateDamageDrop(damage, damageDropPercent, _hitDistance, range);

                    //Damage interface
                    IDamageable _heneDamageable = bulletHits[lastColliderIndex].collider.GetComponent<IDamageable>();

                    _heneDamageable.TakeDamage(damage, CurrentHitForce(), bulletMovingPart.transform.position, bulletHits[lastColliderIndex].normal, out damageInterfaceEffect, out float _damageCaused);

                    //Debug damage
                    if (debug)
                    {
                        Debug.Log("Bullet caused damage by: " + _damageCaused);
                    }

                    //Bullet hit effect
                    BulletExplode(damageInterfaceEffect, bulletHits[lastColliderIndex].normal, bulletHits[lastColliderIndex].collider.transform, bulletHits[lastColliderIndex].point);

                    //Hit someting boolean
                    hitSometing = true;
                }
                else //Bullet hit ground or someting else
                {
                    //Move bullet to hit point
                    bulletMovingPart.transform.position = bulletHits[lastColliderIndex].point;

                    //Bullet hit effect by layer name
                    string _layerName = LayerMask.LayerToName(bulletHits[lastColliderIndex].collider.gameObject.layer);
                    BulletExplode(LayerEffectPrefab(_layerName), bulletHits[lastColliderIndex].normal, bulletHits[lastColliderIndex].collider.transform, bulletHits[lastColliderIndex].point);

                    //Hit someting boolean
                    hitSometing = true;
                }
            }
        }

        private void BulletExplode(GameObject _effect, Vector3 _colliderNormal, Transform _hitTransform, Vector3 _hitPosition)
        {
            BulletHit();

            //Remove bullet holder
            Invoke(nameof(RemoveBullet), destoryWhenHitTime);

            //Remove bullet by time
            EffectRemover _bulletVisual = bulletMovingPart.GetComponent<EffectRemover>();
            _bulletVisual.DestroyEffect(destoryWhenHitTime);

            //If the object that was hit contains rigid body, add force to it
            if (_hitTransform.GetComponent<Rigidbody>() != null)
            {
                Rigidbody _rb = _hitTransform.GetComponent<Rigidbody>();
                _rb.AddForceAtPosition((bulletMovingPart.transform.position - transform.position).normalized * CurrentHitForce(), _hitPosition);
            }

            //If root object contains rigid body, add force to it
            if (_hitTransform.root.GetComponent<Rigidbody>() != null)
            {
                Rigidbody _rb = _hitTransform.root.GetComponent<Rigidbody>();
                _rb.AddForceAtPosition((bulletMovingPart.transform.position - transform.position).normalized * CurrentHitForce(), _hitPosition);
            }

            //If no effect
            if (_effect == null)
            {
                //Bullet behaviours
                if (bulletHitMode == BulletHitMode.BulletDestroyByHit__EffectNotStickToObject)
                {
                    bulletMovingPart.SetActive(false);
                }
                else if (bulletHitMode == BulletHitMode.BulledAndEffectStickToObject)
                {
                    bulletMovingPart.transform.parent = _hitTransform;
                }
                else if (bulletHitMode == BulletHitMode.BulletDestroyByHit__EffectStickToObject)
                {
                    bulletMovingPart.SetActive(false);
                }

                return;
            }

            //Effect rotation
            Quaternion _normalRotation = Quaternion.FromToRotation(Vector3.forward, _colliderNormal);

            //Instansiate effect
            var _instansiateEffect = Instantiate(_effect, bulletMovingPart.transform.position, _normalRotation);

            //Remove bullet effect by time
            _instansiateEffect.AddComponent<EffectRemover>();
            EffectRemover _bulletEffectVisual = _instansiateEffect.GetComponent<EffectRemover>();
            _bulletEffectVisual.DestroyEffect(destoryWhenHitTime);

            //Bullet behaviours
            if (bulletHitMode == BulletHitMode.BulletDestroyByHit__EffectNotStickToObject)
            {
                bulletMovingPart.SetActive(false);
            }
            else if (bulletHitMode == BulletHitMode.BulledAndEffectStickToObject)
            {
                bulletMovingPart.transform.parent = _hitTransform;
                _instansiateEffect.transform.parent = _hitTransform;
            }
            else if (bulletHitMode == BulletHitMode.BulletDestroyByHit__EffectStickToObject)
            {
                bulletMovingPart.SetActive(false);
                _instansiateEffect.transform.parent = _hitTransform;
            }
        }

        private GameObject LayerEffectPrefab(string _layerName)
        {
            for (int i = 0; i < layerEffects.Count; i++)
            {
                if (layerEffects[i].LayerName() == _layerName)
                {
                    return layerEffects[i].HitObject();
                }
            }

            return null;
        }

        private int ClosestColliderInt(RaycastHit[] _raycastHits)
        {
            float _closestDistance = 99999999999f;
            float[] _allDistances = new float[_raycastHits.Length];

            for (int i = 0; i < _raycastHits.Length; i++)
            {
                //Calculate distance to shooter
                float _distanceToShooter = Vector3.Distance(_raycastHits[i].point, transform.position);

                //Add distance to distance array
                _allDistances[i] = _distanceToShooter;

                //Set closest distance
                if (_closestDistance > _distanceToShooter)
                {
                    _closestDistance = _distanceToShooter;
                }
            }

            //Return closest distance from distance array
            for (int i = 0; i < _allDistances.Length; i++)
            {
                if (_allDistances[i] == _closestDistance)
                {
                    if (debug)
                    {
                        Debug.Log("Bullet hit object: " + _raycastHits[i].collider.transform.gameObject.name);
                    }

                    return i;
                }
            }

            return 99999;
        }

        #endregion
    }

    [System.Serializable]
    public class BulletHitLayer
    {
        [SerializeField] private LayerMask hitLayer;
        [SerializeField] private GameObject hitObject;

        public string LayerName()
        {
            return GetSelectedNameFromLayermask(hitLayer);
        }

        public string GetSelectedNameFromLayermask(LayerMask _layerMask)
        {
            int layer = (int)Mathf.Log(_layerMask.value, 2);
            string _layerName = LayerMask.LayerToName(layer);
            return _layerName;
        }

        public GameObject HitObject()
        {
            return hitObject;
        }
    }
}