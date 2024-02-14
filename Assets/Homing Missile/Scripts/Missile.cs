using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Tarodev {
    
    public class Missile : MonoBehaviour {
        [Header("REFERENCES")] 
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private Target _target;
        [SerializeField] private GameObject _explosionPrefab;
        [SerializeField] private GameObject _explosionSound;

        [Header("MOVEMENT")] 
        [SerializeField] private float _speed = 15;
        [SerializeField] private float _rotateSpeed = 95;

        [Header("PREDICTION")] 
        [SerializeField] private float _maxDistancePredict = 100;
        [SerializeField] private float _minDistancePredict = 5;
        [SerializeField] private float _maxTimePrediction = 5;
        private Vector3 _standardPrediction, _deviatedPrediction;

        [Header("DEVIATION")] 
        [SerializeField] private float _deviationAmount = 50;
        [SerializeField] private float _deviationSpeed = 2;
        
        [Header("MECHANICS")]
        //collider variable to pause it at launching
        [SerializeField] private Collider myCollider;
        [SerializeField] private float explosionTimer = 3f;
        
        //keeps track if there is a target
        private bool isTargetNull;
        private bool continueCoroutine = true;

        private void Start()
        {
            isTargetNull = GetTarget() == null;
            PauseCollider(2f);
            TimedExplosion();
        }

        private void FixedUpdate() {
            _rb.velocity = transform.forward * _speed;

            var leadTimePercentage = Mathf.InverseLerp(_minDistancePredict, _maxDistancePredict, Vector3.Distance(transform.position, _target.transform.position));

            if (!isTargetNull)
            {
                //stops Timed Explosion
                continueCoroutine = false;
                
                //Handles Rocket flight path
                PredictMovement(leadTimePercentage);
                AddDeviation(leadTimePercentage);
                RotateRocket();
            }
        }

        private void PredictMovement(float leadTimePercentage) {
            var predictionTime = Mathf.Lerp(0, _maxTimePrediction, leadTimePercentage);

            _standardPrediction = _target.Rb.position + _target.Rb.velocity * predictionTime;
        }

        private void AddDeviation(float leadTimePercentage) {
            var deviation = new Vector3(Mathf.Cos(Time.time * _deviationSpeed), 0, 0);
            
            var predictionOffset = transform.TransformDirection(deviation) * _deviationAmount * leadTimePercentage;

            _deviatedPrediction = _standardPrediction + predictionOffset;
        }

        private void RotateRocket() {
            var heading = _deviatedPrediction - transform.position;

            var rotation = Quaternion.LookRotation(heading);
            _rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, rotation, _rotateSpeed * Time.deltaTime));
        }

        private void OnCollisionEnter(Collision collision) {
            if(_explosionPrefab) Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            if (collision.transform.TryGetComponent<IExplode>(out var ex)) ex.Explode();
   
            Destroy(gameObject);
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _standardPrediction);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(_standardPrediction, _deviatedPrediction);
        }

        //avoid rocket colliding with plane
        private void PauseCollider(float time)
        {
            myCollider.enabled = false;
            StartCoroutine(Delay(() => myCollider.enabled = true, time));
        }

        IEnumerator Delay(Action _callback, float _time)
        {
            yield return new WaitForSeconds(_time);
            _callback?.Invoke();
        }
        
        
        //Delayed Death after time when no target 
        private void TimedExplosion()
        {
            StartCoroutine(DelayedDeath());
        }

        private IEnumerator DelayedDeath()
        {
            if (continueCoroutine)
            {
                //wait for set number of seconds
                yield return new WaitForSeconds(explosionTimer);
            
                //instantiate explosion effect/sound of rocket and destroy gameObject
                if(_explosionPrefab) Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
                if (_explosionSound) Instantiate(_explosionSound, transform.position, Quaternion.identity);
                
                Destroy(gameObject);
            }
        }
        
        //Target Handling
        public void SetTarget(Target setTarget)
        {
            if (setTarget == null)
            {
                isTargetNull = true;
            }
            else
            {
                _target = setTarget;
                isTargetNull = false;
            }
            
        }

        private Target GetTarget()
        {
            return _target;
        }
    }
}