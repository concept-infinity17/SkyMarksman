using UnityEngine;

namespace Tarodev {
    public class Target : MonoBehaviour, IExplode
    {
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private float _speed = 10;
        private float rotationSpeed = 30;
        [SerializeField] private AudioSource explosionAudioSource;
        [SerializeField] private Transform target;
        public Rigidbody Rb => _rb;

        void Start()
        {
            ChangeDirection();
        }

        void LateUpdate()
        {
            // Move the object forward in its current direction
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(target.position.x,250,target.position.z), _speed * Time.deltaTime);
           
        }

        void ChangeDirection()
        {
            // Generate Vector pointing to target
            Vector3 heading = target.position - transform.position;
            //heading.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(heading);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        public void Explode()
        {
            //spawn an empty gameObject with an audioSource attached that plays the explosion sound and destroys itself afterwards
            Instantiate(explosionAudioSource, transform.position, Quaternion.identity);
            
            //Destroy object
            Destroy(gameObject);
        }

        
    
    }
}