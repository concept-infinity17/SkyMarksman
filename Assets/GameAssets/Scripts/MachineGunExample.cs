using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGunExample : MonoBehaviour
{
    [SerializeField] private KeyCode shootInput = KeyCode.Mouse0;

    [SerializeField] private Transform barrel;

    [SerializeField] private GameObject bulletPrefab;

    [Range(0.05f, 0.15f)]
    [SerializeField] private float fireRate = 0.1f;

    private float fireRateTimer = 0f;

    private void Update()
    {
        //Shoot input
        if (Input.GetKey(shootInput))
        {
            Shoot();
        }

        //Fire rate timer
        if (fireRateTimer > 0f)
        {
            fireRateTimer -= Time.deltaTime;
        }
    }

    public void Shoot()
    {
        if (fireRateTimer <= 0f)
        {
            Instantiate(bulletPrefab, barrel.transform.position, barrel.transform.rotation);

            fireRateTimer = fireRate;
        }
    }
}
