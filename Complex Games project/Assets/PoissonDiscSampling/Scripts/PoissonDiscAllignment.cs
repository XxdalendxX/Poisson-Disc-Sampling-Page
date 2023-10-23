using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PrefabObject
{
    public GameObject desiredPrefab;
    public float objectRadius = 5f;

    [HideInInspector]
    public int count;
}
public class SampleData
{
    public Vector3 position;
    public float objectRadius = 5f;

    public SampleData(Vector3 pos, float objRadius)
    {
        position = pos;
        objectRadius = objRadius;
    }
}

public class PoissonDiscAllignment : MonoBehaviour
{
    #region MainVariables
    [SerializeField] PrefabObject[] prefabs;

    [SerializeField] Transform desiredInstantiateLocation;
    [SerializeField] string targetLayerName = "";
    [SerializeField] string checkAgainstLayerName = "";

    [Space]
    [SerializeField, Tooltip("A multiplier put onto the object radius for it's maximum value"), Range(2, 6)] float maxObjectRadiusMultiplier = 5f;

    [Space]
    [SerializeField] int totalAttempts = 5;

    [Header("Placement of objects")]
    [SerializeField] float maxAscent = 2f;
    [SerializeField] float maxDescent = 1f;
    [SerializeField] float maxSlope = 0f;

    [Space]
    ///public variables used in the custom inspector
    [HideInInspector] public Vector3 centerPosition;
    [HideInInspector] public bool circleArea = true;
    [HideInInspector] public float borderRadius = 5f;
    [HideInInspector] public float XDiameter = 0.0f;
    [HideInInspector] public float ZDiameter = 0.0f;
    [HideInInspector] public bool placedObjects = false;


    ///private variables
    Transform _parentGameObject;
    
    LayerMask groundLayerMask;
    LayerMask targetLayerMask;

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

        if (desiredInstantiateLocation != null)
        {
            _parentGameObject = new GameObject().transform;
            _parentGameObject.transform.parent = desiredInstantiateLocation; 
        }
        else
            _parentGameObject = new GameObject().transform;

        _parentGameObject.name = "PoissonDiscSamplingObject";

        PerformPDS();
    }
    #endregion

    /// Clears all objects to save effeciency and trouble
    #region ClearObjects
    public void ClearObjects()
    {
        DestroyImmediate(_parentGameObject.gameObject);
        placedObjects = false;
    }

    #endregion

    /// Main Functions that performs the algorithm
    #region MainFunctions
    void PerformPDS()
    {
        //creates core variables
        List<SampleData> openList = new List<SampleData>();
        HashSet<SampleData> closedList = new HashSet<SampleData>();

        ResetPrefabCounts();

        bool firstObject;

        firstObject = PlaceFirstObject(openList);

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
            SampleData currentObjectData = openList[0];

            int fails = 0;
            while (iterations <= totalAttempts)
            {
                PrefabObject prefab = GetPrefabObject();

                //finds a location to perform checks within that meets position critera
                Vector3 position = FindLocation(currentObjectData, prefab.objectRadius);



                RaycastHit hit;
                if (Physics.Raycast(GetMaxAscentPos(position), Vector3.down, out hit, GetRayCheckDistance(), groundLayerMask))
                {

                    PerformChecks(hit, prefab.objectRadius);
                }

                if (CheckIfCanBePlaced() == true)
                {
                    GameObject spawnedObject = PlaceObject(hit, prefab);
                    SampleData newSampleObject = new SampleData(position, prefab.objectRadius);
                    openList.Add(newSampleObject);
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
            closedList.Add(currentObjectData);
        }
        placedObjects = true;

        Debug.Log("PDS complete");
    }

    bool PlaceFirstObject(List<SampleData> openList)
    {
        bool gotFirstObject = false;
        int attempt = 0;

        while (!gotFirstObject)
        {
            Vector3 position = FindFirstPosition();

            PrefabObject prefab = GetPrefabObject();

            RaycastHit hit;
            if (Physics.Raycast(GetMaxAscentPos(position), Vector3.down, out hit, GetRayCheckDistance(), groundLayerMask))
            {
                PerformChecks(hit, prefab.objectRadius);
            }

            if (CheckIfCanBePlaced())
            {
                GameObject spawnedObject = PlaceObject(hit, prefab);
                SampleData newSampleData = new SampleData(position, prefab.objectRadius);
                openList.Add(newSampleData);
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
    GameObject PlaceObject(RaycastHit hit, PrefabObject prefabObject)
    {
        //places referenceObject in world and assigns collider radius
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        Vector3 spawnPosition = hit.point;
        GameObject spawnedObject;

        spawnedObject = Instantiate(prefabObject.desiredPrefab, spawnPosition, rotation, _parentGameObject);

        spawnedObject.name = prefabObject.desiredPrefab.name + "_" + prefabObject.count;
        prefabObject.count++;
        SphereCollider sCollider = spawnedObject.GetComponent<SphereCollider>();
        sCollider.radius = prefabObject.objectRadius;
        return spawnedObject;
    }

    Vector3 FindFirstPosition()
    {
        Vector3 position;
        position.x = Random.Range(centerPosition.x - (XDiameter / 2), centerPosition.x + (XDiameter / 2));
        position.y = centerPosition.y;
        position.z = Random.Range(centerPosition.z - (ZDiameter / 2), centerPosition.z + (ZDiameter / 2));

        return position;
    }
    Vector3 FindLocation(SampleData referenceObject, float objectRadius)
    {
        float min = referenceObject.objectRadius + objectRadius;
        float max = referenceObject.objectRadius + (objectRadius * maxObjectRadiusMultiplier);
        float range = Random.Range(min, max);
        float angle = Random.Range(0, 360) * Mathf.PI / 180;
        float x = referenceObject.position.x + Mathf.Cos(angle) * range;
        float z = referenceObject.position.z + Mathf.Sin(angle) * range;
        return new Vector3(x, referenceObject.position.y, z);
    }

    #endregion

    /// Miscilaneous Functions to help perform
    #region MiscelaneousFunctions
    PrefabObject GetPrefabObject()
    {
        return prefabs[Random.Range(0, prefabs.Length)];
    }

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

    void ResetPrefabCounts()
    {
        foreach (PrefabObject prefab in prefabs)
        {
            prefab.count = 1;
        }
    }
    #endregion

    #region Checks
    void PerformChecks(RaycastHit hit, float objectRadius)
    {
        raycastCheck = true;

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
