using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PoissonDiscAllignment : MonoBehaviour
{
    #region MainVariables
    [SerializeField] GameObject desiredPrefab;
    [SerializeField] Transform desiredInstantiateLocation;
    [SerializeField] string targetLayerName = "";
    [SerializeField] string checkAgainstLayerName = "";

    [Header("Object radius")]
    [SerializeField] float objectRadius = 5f;
    [SerializeField, Tooltip("A multiplier put onto the object radius for it's maximum value"), Range(2, 6)] float maxObjectRadiusMultiplier = 5f;

    [Space]
    [SerializeField] int totalAttempts = 5;

    [Header("Placement of objects")]
    [SerializeField] float maxAscent = 2f;
    [SerializeField] float maxDescent = 1f;
    [SerializeField] float maxSlope = 0f;

    [Space]
    [SerializeField] List<GameObject> allObjects = new List<GameObject>();

    [Space]
    ///public variables used in the custom inspector
    [HideInInspector] public Vector3 centerPosition;
    [HideInInspector] public bool circleArea = true;
    [HideInInspector] public float borderRadius = 5f;
    [HideInInspector] public float XDiameter = 0.0f;
    [HideInInspector] public float ZDiameter = 0.0f;


    ///private variables
    LayerMask groundLayerMask;
    LayerMask targetLayerMask;
    float pushOut;

    bool colliderCheck = false;
    bool borderCheck = false;
    bool raycastCheck = false;
    bool slopeCheck =  false;
    #endregion

    /// called from the custom inspector to start the algorithm
    #region ExecuteFunction
    public void Execute()
    {
        groundLayerMask = LayerMask.GetMask(targetLayerName);
        targetLayerMask = LayerMask.GetMask(checkAgainstLayerName);

        pushOut = desiredPrefab.GetComponentInChildren<Transform>().position.y - desiredPrefab.transform.position.y;

        PerformPDS();
    }
    #endregion

    /// Clears all objects to save effeciency and trouble
    #region ClearObjects
    public void ClearObjects()
    {
        int obj = allObjects.Count;
        for (int i = 0; i < obj - 1; i++)
        {
            DestroyImmediate(allObjects[1]);
            allObjects.RemoveAt(1);
        }
        GameObject finalObject = allObjects[0];
        allObjects.RemoveAt(0);
        GameObject.DestroyImmediate(finalObject);
    }
    #endregion

    /// Main Functions that performs the algorithm
    #region MainFunctions
    void PerformPDS()
    {
        //creates core variables
        float yPos = centerPosition.y;
        List<Vector3> openList = new List<Vector3>();
        HashSet<Vector3> closedList = new HashSet<Vector3>();

        bool firstObject;

        firstObject = PlaceFirstObject(yPos, openList);

        if (!firstObject)
            return;

        int iterations = 0;
        bool mustStop = false;
        while (!mustStop)
        {
            if (openList.Count < 1)
            {
                //if open list is empty then break the loop
                mustStop = true;
                break;
            }
            Vector3 currentObjectPos = openList[0];

            int fails = 0;
            while (iterations <= totalAttempts)
            {
                //finds a location to perform checks within that meets position critera
                Vector3 position = FindLocation(yPos, currentObjectPos);

                RaycastHit hit;
                if (Physics.Raycast(GetMaxAscentPos(position), Vector3.down, out hit, GetRayCheckDistance(), groundLayerMask))
                {
                    PerformChecks(hit);
                }

                if (CheckIfCanBePlaced() == true)
                {
                    GameObject spawnedObject = PlaceObject(hit);
                    openList.Add(spawnedObject.transform.position);
                    allObjects.Add(spawnedObject);
                    iterations = 0;
                }
                else
                {
                    //check failed and fail counter and iteration counter goes up
                    fails++;
                    iterations++;
                }
            }
            iterations = 0;
            fails = 0;
            //removes transform from open list and transfers it to the closed list
            openList.RemoveAt(0);
            closedList.Add(currentObjectPos);
        }
        Debug.Log("PDS complete");
    }

    bool PlaceFirstObject(float yPos, List<Vector3> openList)
    {
        bool gotFirstObject = false;
        int attempt = 0;

        while (!gotFirstObject)
        {
            Vector3 position = FindFirstPosition(yPos);

            Debug.Log(position);

            RaycastHit hit;
            //check raycast as problem exists here
            if (Physics.Raycast(GetMaxAscentPos(position), Vector3.down, out hit, GetRayCheckDistance(), groundLayerMask))
            {
                PerformChecks(hit);
            }

            Debug.Log(hit);

            if (CheckIfCanBePlaced())
            {
                GameObject spawnedObject = PlaceObject(hit);
                openList.Add(spawnedObject.transform.position);
                allObjects.Add(spawnedObject);
                gotFirstObject = true;
                break;
            }
            else
                attempt++;

            if (attempt > totalAttempts)
            {
                Debug.Log("Failed to find an available location");
                break;
            }
        }
        return gotFirstObject;
    }
    GameObject PlaceObject(RaycastHit hit)
    {
        //places object in world and assigns collider radius
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        Vector3 spawnPosition = hit.point;
        GameObject spawnedObject;

        if (desiredInstantiateLocation != null)
            spawnedObject = Instantiate(desiredPrefab, spawnPosition, rotation, desiredInstantiateLocation);
        else
            spawnedObject = Instantiate(desiredPrefab, spawnPosition, rotation);

        SphereCollider sCollider = spawnedObject.GetComponent<SphereCollider>();
        sCollider.radius = objectRadius;
        return spawnedObject;
    }

    Vector3 FindFirstPosition(float yPos)
    {
        Vector3 position;
        float min = 0;
        float max = 0;
        if (!circleArea)
        {
            min = centerPosition.x - (XDiameter / 2);
            max = centerPosition.x + (ZDiameter / 2);
        }
        else
        {
            min = centerPosition.x - (borderRadius);
            max = centerPosition.x + (borderRadius);
        }

        var range = Random.Range(min, max);
        var angle = Random.Range(0, 360);
        position.x = centerPosition.x + Mathf.Cos(angle) * range;
        position.y = yPos;
        position.z = centerPosition.z + Mathf.Sin(angle) * range;

        return position;
    }
    Vector3 FindLocation(float y, Vector3 objectTransform)
    {
        float min = objectRadius * 2;
        float max = objectRadius * maxObjectRadiusMultiplier;
        var range = Random.Range(min, max);
        var angle = Random.Range(0, 360);
        var x = objectTransform.x + Mathf.Cos(angle) * range;
        var z = objectTransform.z + Mathf.Sin(angle) * range;
        return new Vector3(x, y, z);
    }

    #endregion

    /// Miscilaneous Functions to help perform
    #region MiscelaneousFunctions
    Vector3 GetMaxAscentPos(Vector3 pos)
    {
        Vector3 fromPos = new Vector3(pos.x, pos.y + maxAscent, pos.z);
        return fromPos;
    }

    float GetRayCheckDistance()
    {
        float distance = maxAscent + maxDescent;
        return distance;
    }

    public int GetObjectCount()
    {
        return allObjects.Count;
    }

    public float GetObjectRadius()
    {
        return objectRadius;
    }
    #endregion

    #region Checks
    void PerformChecks(RaycastHit hit)
    {
        raycastCheck = true;

        if (circleArea)
            CheckPositionInBorder(hit.point, centerPosition);
        else
            CheckPositionInBorder(hit.point, centerPosition);

        Collider[] colliders = Physics.OverlapSphere(hit.point, objectRadius, targetLayerMask, QueryTriggerInteraction.Collide);
        ColliderCheck(colliders.Length);

        if (Vector3.Angle(hit.normal, Vector3.up) <= maxSlope)
            slopeCheck = true;
    }

    void ColliderCheck(int colliderLength)
    {
        if (colliderLength < 1)
        {
            colliderCheck = true;
        }
    }

    void CheckPositionInBorder(Vector3 position, Vector3 corePosition)
    {
        if(circleArea)
        {
            float distance = Mathf.Sqrt(Mathf.Pow(position.x - corePosition.x, 2) + Mathf.Pow(position.z - corePosition.z, 2));
            float radius = Mathf.Sqrt(Mathf.Pow(position.x, 2) + Mathf.Pow(position.z, 2));
            if (distance <= borderRadius)
            {
                borderCheck = true;
            }
        }
        else
        {
            float XMax = corePosition.x + (XDiameter / 2);
            float XMin = corePosition.x - (XDiameter / 2);
            float ZMax = corePosition.z + (ZDiameter / 2);
            float ZMin = corePosition.z - (ZDiameter / 2);

            if (position.x <= XMax && position.x >= XMin)
            {
                if (position.z <= ZMax && position.z >= ZMin)
                {
                    borderCheck = true;
                }
            }
        }
    }

    bool CheckIfCanBePlaced()
    {
        if(colliderCheck && borderCheck && raycastCheck && slopeCheck)
        {
            colliderCheck = false;
            borderCheck = false;
            raycastCheck = false;
            slopeCheck = false;
            return true;
        }

        colliderCheck = false;
        borderCheck = false;
        raycastCheck = false;
        slopeCheck = false;

        return false;
    }
    #endregion

    #region Gizmos
    void OnDrawGizmosSelected()
    {
        Vector3 corePos = gameObject.transform.position;
        if (!circleArea)
        {
            float xDist = (XDiameter / 2);
            float zDist = (ZDiameter / 2);
            Vector3 pointA = new Vector3(centerPosition.x - xDist, centerPosition.y, centerPosition.z - zDist);
            Vector3 pointB = new Vector3(centerPosition.x - xDist, centerPosition.y, centerPosition.z + zDist);
            Vector3 pointC = new Vector3(centerPosition.x + xDist, centerPosition.y, centerPosition.z + zDist);
            Vector3 pointD = new Vector3(centerPosition.x + xDist, centerPosition.y, centerPosition.z - zDist);
            Gizmos.DrawLine(pointA, pointB);
            Gizmos.DrawLine(pointB, pointC);
            Gizmos.DrawLine(pointC, pointD);
            Gizmos.DrawLine(pointD, pointA);
        }
        else
        {
            Gizmos.DrawWireSphere(centerPosition, borderRadius);
        }

        Vector3 fromPos = new Vector3(centerPosition.x, centerPosition.y + maxAscent, centerPosition.z);
        Vector3 toPos = new Vector3(centerPosition.x, centerPosition.y - maxDescent, centerPosition.z);
        Gizmos.DrawLine(fromPos, toPos);
    }
    #endregion
}
