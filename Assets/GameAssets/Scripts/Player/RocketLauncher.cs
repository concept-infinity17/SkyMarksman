using System.Collections;
using System.Collections.Generic;
using Tarodev;
using UnityEngine;

public class RocketLauncher : MonoBehaviour
{
    [SerializeField] private KeyCode launchInput = KeyCode.Mouse1;
    
    [SerializeField] private GameObject rocketPrefab;
    //range for targeting system
    [SerializeField] private float targetRange;
    
    void Update()
    {
        Target closestTarget = ClosestTargetDetection();
        //launch input
        if (Input.GetKeyDown(launchInput))
        {
            Launch(closestTarget);
        }
    }

    private void Launch(Target closestTarget)
    {
       GameObject rocket = Instantiate(rocketPrefab, transform.position, transform.rotation);
       Missile _missileScript = rocket.GetComponent<Missile>();
       
       // Check if the RocketScript component is not null
       if (_missileScript != null)
       {
           Target _target = closestTarget;
            // Check if the target component is not null
           if (_target != null)
           {
               //set target to closest Target
               _missileScript.SetTarget(_target);
               
           }
           else
           {
               _missileScript.SetTarget(null);
           }
           
       }
      
       
       
       
    }
    
    
    private Target currentClosestTarget = null;
    private Target ClosestTargetDetection()
    {
        Vector3 offsetPosition = transform.position + transform.forward * targetRange;

         // Save collision detection objects in array
        Collider[] enemyColliders = Physics.OverlapSphere(offsetPosition, targetRange);
    
        // Initialize variables to keep track of the closest target and its distance
        Target newClosestTarget = null;
        float closestDistance = Mathf.Infinity;
    
        foreach (var enemyCollider in enemyColliders)
        {
            if (enemyCollider.TryGetComponent<Target>(out Target target))
            {
                // Calculate the distance between the current target and the detecting object
                float distance = Vector3.Distance(transform.position, enemyCollider.transform.position);
    
                // Check if the current target is closer than the previously closest one
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    newClosestTarget = target;
                }
                //Debug.Log("Target found. Ready to launch.");
            }
            else
            {
                //Debug.Log("Target not found. Rocket is destroyed after 5 seconds");
            }
        }
    
        // Check if the closest target has changed
        if (newClosestTarget != currentClosestTarget)
        {
            // Delete the previous TargetIndicator (if any)
            if (currentClosestTarget != null)
            {
                TargetIndicator previousTargetIndicator = currentClosestTarget.GetComponent<TargetIndicator>();
                if (previousTargetIndicator != null)
                {
                    previousTargetIndicator.DeleteTargetIndicator();
                }
            }
    
            // Set the new closest target
            currentClosestTarget = newClosestTarget;
    
            // Create a new TargetIndicator for the new closest target
            if (currentClosestTarget != null)
            {
                TargetIndicator targetIndicator = currentClosestTarget.GetComponent<TargetIndicator>();
                if (targetIndicator != null && !targetIndicator.HasTargetIndicator())
                {
                    targetIndicator.CreateTargetIndicator();
                }
            }
        }
    
        // Return the closest target (could be null if no target is found)
        return newClosestTarget;
    }


}
