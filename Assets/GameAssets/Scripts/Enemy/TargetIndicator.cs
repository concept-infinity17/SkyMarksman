using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetIndicator : MonoBehaviour
{
    [SerializeField] private GameObject targetIndicatorPrefab;


    public void CreateTargetIndicator()
    {
        GameObject targetIndicator = Instantiate(targetIndicatorPrefab, transform.position + Vector3.up * 16, targetIndicatorPrefab.transform.rotation);
        targetIndicator.transform.parent = transform;
    }

    public bool HasTargetIndicator()
    {
        string tagToFind = "TargetIndicator";

        // Check if the child with the specified tag exists
        Transform child = transform.Find(tagToFind);

        if (child != null)
        {
            //Debug.Log("Child with tag '" + tagToFind + "' found!");
            // You can access the child here using 'child'
            return true;
        }
        else
        {
            //Debug.Log("Child with tag '" + tagToFind + "' not found.");
            return false;
        }
    }

    public void DeleteTargetIndicator()
    {
        string tagToDelete = "TargetIndicator";
        GameObject[] childObjects = GameObject.FindGameObjectsWithTag(tagToDelete);

        // Iterate through each child object and destroy it
        foreach (GameObject child in childObjects)
        {
            // Ensure the child object is a direct child of the current GameObject
            if (child.transform.parent == transform)
            {
                Destroy(child);
            }
        }
    }
}
