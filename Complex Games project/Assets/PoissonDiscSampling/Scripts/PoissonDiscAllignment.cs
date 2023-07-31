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
    [SerializeField] int layerNumber = 0;

    [Header("Object radius")]
    [SerializeField] float objectRadius = 5f;
    [SerializeField, Tooltip("A multiplier put onto the object radius for it's maximum value"), Range(2, 6)] float maxObjectRadiusMultiplier = 5f;

    [Space]
    [SerializeField] int attempts = 5;

    [Header("Placement of objects")]
    [SerializeField] float maxAscent = 2f;
    [SerializeField] float maxDescent = 1f;
    [SerializeField] float maxSlope = 0f;

    [Space]
    [SerializeField] List<GameObject> allObjects = new List<GameObject>();

    [Space]
    ///public variables used in the custom inspector
    [HideInInspector]public bool isCore = true;
    [HideInInspector] public Vector3 centerPosition;
    [HideInInspector] public bool circleArea = true;
    [HideInInspector] public float borderRadius = 5f;
    [HideInInspector] public float XDiameter = 0.0f;
    [HideInInspector] public float ZDiameter = 0.0f;


    ///private variables
    int layerMask;
    GameObject epicenter;
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
        layerMask = LayerMask.GetMask(LayerMask.LayerToName(layerNumber));

        pushOut = desiredPrefab.GetComponentInChildren<Transform>().position.y - desiredPrefab.transform.position.y;
        
        if(isCore)
        {
            epicenter = this.gameObject;

            if (circleArea)
            {
                PerformCoreCircle();
            }
            else
            {
                PerformCoreSquare();
            }
        }
        else
        {
            if (circleArea)
            {
                PerformCorelessCircle();
            }
            else
            {
                PerformCorelessSquare();
            }
        }
    }
    #endregion

    /// Clears all objects to save effeciency and trouble
    #region ClearLists
    public void ClearLists()
    {
        if (isCore)
        {
            int obj = allObjects.Count;
            for (int i = 0; i < obj - 1; i++)
            {
                DestroyImmediate(allObjects[1]);
                allObjects.RemoveAt(1);
            }
            allObjects.RemoveAt(0);
        }
        else
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
    }
    #endregion

    /// Main Functions that performs the algorithm
    #region MainFunctions
    void PerformCoreCircle()
    {
        //creates core variables
        float y = epicenter.transform.position.y;
        int iterations = 0;
        List<Vector3> openList = new List<Vector3>();
        List<Vector3> closedList = new List<Vector3>();
        bool mustStop = false;

        //assigns core object in open list and all objects list 
        openList.Add(epicenter.transform.position);
        allObjects.Add(epicenter);

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
            while (iterations <= attempts)
            {
                //finds a location
                Vector3 position = FindLocation(y, currentObjectPos);
                
                //Raycast check, collider check and slope check
                RaycastHit hit;
                if(Physics.Raycast(GetMaxAscentPos(position), Vector3.down, out hit , GetRayCheckDistance(), LayerMask.GetMask("Ground")))
                {
                    raycastCheck = true;


                    CheckPosInCircleBorder(hit.point, epicenter.transform.position);

                    Collider[] colliders = Physics.OverlapSphere(hit.point, objectRadius, layerMask, QueryTriggerInteraction.Collide);
                    ColliderCheck(colliders.Length);

                    if (Vector3.Angle(hit.normal, Vector3.up) <= maxSlope)
                        slopeCheck = true;

                }

                if (CheckToBePlaced() == true)
                {
                    //places object in world and assigns collider radius
                    Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                    Vector3 spawnPosition = hit.point;
                    GameObject spawnedObject = Instantiate(desiredPrefab, spawnPosition, rotation);
                    SphereCollider sCollider = spawnedObject.GetComponent<SphereCollider>();
                    sCollider.radius = objectRadius;


                    //adds object transform and gameObject to openList and allObjects respectively and resets the iteration counter  
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

    void PerformCoreSquare()
    {
        //creates core variables
        float y = epicenter.transform.position.y;
        int iterations = 0;
        List<Vector3> openList = new List<Vector3>();
        List<Vector3> closedList = new List<Vector3>();
        bool mustStop = false;

        //assigns core object in open list and all objects list 
        openList.Add(epicenter.transform.position);
        allObjects.Add(epicenter);

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
            while (iterations <= attempts)
            {
                //finds a location to perform checks within that meets position critera
                Vector3 position = FindLocation(y, currentObjectPos);

                RaycastHit hit;
                if (Physics.Raycast(GetMaxAscentPos(position), Vector3.down, out hit, GetRayCheckDistance(), LayerMask.GetMask("Ground")))
                {
                    raycastCheck = true;


                    CheckPosInQuadBorder(hit.point, epicenter.transform.position);

                    Collider[] colliders = Physics.OverlapSphere(hit.point, objectRadius, layerMask, QueryTriggerInteraction.Collide);
                    ColliderCheck(colliders.Length);

                    if (Vector3.Angle(hit.normal, Vector3.up) <= maxSlope)
                        slopeCheck = true;

                }

                if (CheckToBePlaced())
                {
                    //places object in world and assigns collider radius
                    Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                    Vector3 spawnPosition = hit.point;
                    GameObject spawnedObject = Instantiate(desiredPrefab, spawnPosition, rotation);
                    SphereCollider sCollider = spawnedObject.GetComponent<SphereCollider>();
                    sCollider.radius = objectRadius;

                    //adds object transform and gameObject to openList and allObjects respectively and resets the iteration counter  
                    openList.Add(spawnedObject.transform.position);
                    allObjects.Add(spawnedObject);
                    iterations = 0;
                }
                else
                {
                    //position check failed and fail counter and iteration counter goes up
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

    void PerformCorelessCircle()
    {
        //creates core variables
        float y = centerPosition.y;
        int iterations = 0;
        List<Vector3> openList = new List<Vector3>();
        List<Vector3> closedList = new List<Vector3>();
        bool mustStop = false;

        //Creates and assigns starting object in open list and all objects list if there is an available slot
        bool gotFirstObject = false;
        int attempt = 0;
        while (!gotFirstObject)
        {
            Vector3 position;
            float min = centerPosition.x - (borderRadius);
            float max = centerPosition.x + (borderRadius);
            var range = Random.Range(min, max);
            var angle = Random.Range(0, 360);
            position.x = centerPosition.x + Mathf.Cos(angle) * range;
            position.y = y;
            position.z = centerPosition.z + Mathf.Sin(angle) * range;
            position.y = y;
            position.z = Random.Range(centerPosition.z - (borderRadius), centerPosition.z + (ZDiameter / 2));
            
            RaycastHit hit;
            if (Physics.Raycast(GetMaxAscentPos(position), Vector3.down, out hit, GetRayCheckDistance(), LayerMask.GetMask("Ground")))
            {
                raycastCheck = true;


                CheckPosInCircleBorder(hit.point, centerPosition);

                Collider[] colliders = Physics.OverlapSphere(hit.point, objectRadius, layerMask, QueryTriggerInteraction.Collide);
                ColliderCheck(colliders.Length);

                if (Vector3.Angle(hit.normal, Vector3.up) <= maxSlope)
                    slopeCheck = true;

            }

            if (CheckToBePlaced())
            {
                //places object in world and assigns collider radius
                Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                Vector3 spawnPosition = hit.point;
                GameObject spawnedObject = Instantiate(desiredPrefab, spawnPosition, rotation);
                SphereCollider sCollider = spawnedObject.GetComponent<SphereCollider>();
                sCollider.radius = objectRadius;
                openList.Add(spawnedObject.transform.position);
                allObjects.Add(spawnedObject);
                gotFirstObject = true;
            }
            else
            {
                attempt++;
            }

            if(attempt > attempts)
            {
                gotFirstObject = true;
                Debug.Log("Failed to find an available location");
            }
            
        }

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
            while (iterations <= attempts)
            {
                //finds a location to perform checks within that meets position critera
                Vector3 position = FindLocation(y, currentObjectPos);

                RaycastHit hit;
                if (Physics.Raycast(GetMaxAscentPos(position), Vector3.down, out hit, GetRayCheckDistance(), LayerMask.GetMask("Ground")))
                {
                    raycastCheck = true;


                    CheckPosInCircleBorder(hit.point, centerPosition);

                    Collider[] colliders = Physics.OverlapSphere(hit.point, objectRadius, layerMask, QueryTriggerInteraction.Collide);
                    ColliderCheck(colliders.Length);

                    if (Vector3.Angle(hit.normal, Vector3.up) <= maxSlope)
                        slopeCheck = true;

                }

                if (CheckToBePlaced() == true)
                {
                    //places object in world and assigns collider radius
                    Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                    Vector3 spawnPosition = hit.point;
                    GameObject spawnedObject = Instantiate(desiredPrefab, spawnPosition, rotation);
                    SphereCollider sCollider = spawnedObject.GetComponent<SphereCollider>();
                    sCollider.radius = objectRadius;


                    //adds object transform and gameObject to openList and allObjects respectively and resets the iteration counter  
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

    void PerformCorelessSquare()
    {
        //creates core variables
        float y = centerPosition.y;
        int iterations = 0;
        List<Vector3> openList = new List<Vector3>();
        List<Vector3> closedList = new List<Vector3>();
        bool mustStop = false;

        //Creates and assigns starting object in open list and all objects list if there is an available slot
        bool gotFirstObject = false;
        int attempt = 0;
        while (!gotFirstObject)
        {
            Vector3 position;
            float min = centerPosition.x - (XDiameter/2);
            float max = centerPosition.x + (ZDiameter/2);
            var range = Random.Range(min, max);
            var angle = Random.Range(0, 360);
            position.x = centerPosition.x + Mathf.Cos(angle) * range;
            position.y = y;
            position.z = centerPosition.z + Mathf.Sin(angle) * range;
            position.y = y;
            position.z = Random.Range(centerPosition.z - (borderRadius), centerPosition.z + (ZDiameter / 2));

            RaycastHit hit;
            if (Physics.Raycast(GetMaxAscentPos(position), Vector3.down, out hit, GetRayCheckDistance(), LayerMask.GetMask("Ground")))
            {
                raycastCheck = true;


                CheckPosInQuadBorder(hit.point, centerPosition);

                Collider[] colliders = Physics.OverlapSphere(hit.point, objectRadius, layerMask, QueryTriggerInteraction.Collide);
                ColliderCheck(colliders.Length);

                if (Vector3.Angle(hit.normal, Vector3.up) <= maxSlope)
                    slopeCheck = true;

            }

            if (CheckToBePlaced())
            {
                //places object in world and assigns collider radius
                Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                Vector3 spawnPosition = hit.point;
                GameObject spawnedObject = Instantiate(desiredPrefab, spawnPosition, rotation);
                SphereCollider sCollider = spawnedObject.GetComponent<SphereCollider>();
                sCollider.radius = objectRadius;
                openList.Add(spawnedObject.transform.position);
                allObjects.Add(spawnedObject);
                gotFirstObject = true;
            }
            else
            {
                attempt++;
            }

            if (attempt > attempts)
            {
                gotFirstObject = true;
                Debug.Log("Failed to find an available location");
            }

        }

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
            while (iterations <= attempts)
            {
                //finds a location to perform checks within that meets position critera
                Vector3 position = FindLocation(y, currentObjectPos);

                RaycastHit hit;
                if (Physics.Raycast(GetMaxAscentPos(position), Vector3.down, out hit, GetRayCheckDistance(), LayerMask.GetMask("Ground")))
                {
                    raycastCheck = true;


                    CheckPosInQuadBorder(hit.point, centerPosition);

                    Collider[] colliders = Physics.OverlapSphere(hit.point, objectRadius, layerMask, QueryTriggerInteraction.Collide);
                    ColliderCheck(colliders.Length);

                    if (Vector3.Angle(hit.normal, Vector3.up) <= maxSlope)
                        slopeCheck = true;

                }

                if (CheckToBePlaced() == true)
                {
                    //places object in world and assigns collider radius
                    Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                    Vector3 spawnPosition = hit.point;
                    GameObject spawnedObject = Object.Instantiate(desiredPrefab, spawnPosition, rotation);
                    SphereCollider sCollider = spawnedObject.GetComponent<SphereCollider>();
                    sCollider.radius = objectRadius;


                    //adds object transform and gameObject to openList and allObjects respectively and resets the iteration counter  
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
    #endregion

    /// Miscilaneous Functions to help perform
    #region MiscelaneousFunctions
  
    Vector3 FindLocation(float y, Vector3 objectTransform)
    {
        float min = objectRadius * 2;
        float max = objectRadius * maxObjectRadiusMultiplier;
        var range = Random.Range(min, max);
        var angle = Random.Range(0, 360);
        var x = objectTransform.x + Mathf.Cos(angle) * range;
        var z = objectTransform.z + Mathf.Sin(angle) * range;
        return new Vector3(x,y,z);
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
    void ColliderCheck(int colliderLength)
    {
        if (colliderLength < 1)
        {
            colliderCheck = true;
        }
    }

    void CheckPosInCircleBorder(Vector3 position, Vector3 corePosition)
    {
        float distance = Mathf.Sqrt(Mathf.Pow(position.x - corePosition.x, 2) + Mathf.Pow(position.z - corePosition.z, 2));
        float radius = Mathf.Sqrt(Mathf.Pow(position.x, 2) + Mathf.Pow(position.z, 2));
        if (distance <= borderRadius)
        {
            borderCheck = true;
        }
    }

    void CheckPosInQuadBorder(Vector3 position, Vector3 corePos)
    {
        float XMax = corePos.x + (XDiameter / 2);
        float XMin = corePos.x - (XDiameter / 2);
        float ZMax = corePos.z + (ZDiameter / 2);
        float ZMin = corePos.z - (ZDiameter / 2);

        if (position.x <= XMax && position.x >= XMin)
        {
            if (position.z <= ZMax && position.z >= ZMin)
            {
                borderCheck = true;
            }
        }
    }

    bool CheckToBePlaced()
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
            if(isCore)
            {
                float xDist = (XDiameter / 2);
                float zDist = (ZDiameter / 2);
                Vector3 pointA = new Vector3(corePos.x - xDist, corePos.y, corePos.z - zDist);
                Vector3 pointB = new Vector3(corePos.x - xDist, corePos.y, corePos.z + zDist);
                Vector3 pointC = new Vector3(corePos.x + xDist, corePos.y, corePos.z + zDist);
                Vector3 pointD = new Vector3(corePos.x + xDist, corePos.y, corePos.z - zDist);
                Gizmos.DrawLine(pointA, pointB);
                Gizmos.DrawLine(pointB, pointC);
                Gizmos.DrawLine(pointC, pointD);
                Gizmos.DrawLine(pointD, pointA);
            }
            else
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
        }

        if(circleArea)
        {
            if(isCore)
            {
                Gizmos.DrawWireSphere(gameObject.transform.position, borderRadius);
            }
            else
            {
                Gizmos.DrawWireSphere(centerPosition, borderRadius);
            }
        }

        if(isCore)
        {
            
            Vector3 fromPos = new Vector3(corePos.x, corePos.y + maxAscent, corePos.z);
            Vector3 toPos = new Vector3(corePos.x, corePos.y - maxDescent, corePos.z);
            Gizmos.DrawLine(fromPos, toPos);
        }
        else
        {
            Vector3 fromPos = new Vector3(centerPosition.x, centerPosition.y + maxAscent, centerPosition.z);
            Vector3 toPos = new Vector3(centerPosition.x, centerPosition.y - maxDescent, centerPosition.z);
            Gizmos.DrawLine(fromPos, toPos);
        }
    }

    #endregion
}
