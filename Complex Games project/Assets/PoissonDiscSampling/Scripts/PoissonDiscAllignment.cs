using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoissonDiscAllignment : MonoBehaviour
{
    [SerializeField] GameObject desiredPrefab;
    [SerializeField] int layerNumber = 0;

    [Header("Object radius")]
    [SerializeField] float objectRadius = 5f;
    [SerializeField, Tooltip("A multiplier put onto the object radius for it's maximum value"), Range(2, 6)] float maxObjectRadiusMultiplier = 5f;

    [Space]
    [SerializeField] int attempts = 5;

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

    /// called from the custom inspector to start the algorithm
    #region ExecuteFunction
    public void Execute()
    {
        layerMask = LayerMask.GetMask(LayerMask.LayerToName(layerNumber));
        
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
                //finds a location to perform checks within that meets position critera
                Vector3 position = FindLocation(y, currentObjectPos);
                Collider[] colliders = Physics.OverlapSphere(position, objectRadius, layerMask, QueryTriggerInteraction.Collide);
                if (colliders.Length == 0 && CheckPosInCircleBorder(position, epicenter.transform.position) == true)
                {
                    //places object in world and assigns collider radius
                    GameObject spawnedObject = SpawnObject(desiredPrefab, position, Quaternion.identity);
                    SphereCollider sCollider = spawnedObject.GetComponent<SphereCollider>();
                    sCollider.radius = objectRadius;

                    //adds object transform and gameObject to openList and allObjects respectively and resets the iteration counter  
                    openList.Add(spawnedObject.transform.position);
                    allObjects.Add(spawnedObject);
                    iterations = 0;
                }
                if (colliders.Length > 0)
                {
                    //position check failed and fail counter and iteration counter goes up
                    fails++;
                    iterations++;
                }
            }
            //provides details on open list, closed list and the amount of checks that failed while the object was going through the loop
            Debug.Log("Placement test failed: " + fails + ", || Open List count: " + openList.Count + " || Closed List count: " + closedList.Count );
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
                Collider[] colliders = Physics.OverlapSphere(position, objectRadius, layerMask, QueryTriggerInteraction.Collide);
                if (colliders.Length == 0 && CheckPosInQuadBorder(position, epicenter.transform.position) == true)
                {
                    //places object in world and assigns collider radius
                    GameObject spawnedObject = SpawnObject(desiredPrefab, position, Quaternion.identity);
                    SphereCollider sCollider = spawnedObject.GetComponent<SphereCollider>();
                    sCollider.radius = objectRadius;

                    //adds object transform and gameObject to openList and allObjects respectively and resets the iteration counter  
                    openList.Add(spawnedObject.transform.position);
                    allObjects.Add(spawnedObject);
                    iterations = 0;
                }
                if (colliders.Length > 0)
                {
                    //position check failed and fail counter and iteration counter goes up
                    fails++;
                    iterations++;
                }
            }
            //provides details on open list, closed list and the amount of checks that failed while the object was going through the loop
            Debug.Log("Placement test failed: " + fails + ", || Open List count: " + openList.Count + " || Closed List count: " + closedList.Count);
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
            position.x = Random.Range(centerPosition.x - (XDiameter/2) , centerPosition.x + (XDiameter / 2));
            position.y = y;
            position.z = Random.Range(centerPosition.z - (ZDiameter / 2), centerPosition.z + (ZDiameter / 2));
            Collider[] colliders = Physics.OverlapSphere(position, objectRadius, layerMask, QueryTriggerInteraction.Collide);
            if (CheckPosInCircleBorder(position, centerPosition) == true)
            {
                openList.Add(position);
                GameObject spawnedObject = SpawnObject(desiredPrefab, position, Quaternion.identity);
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
                Collider[] colliders = Physics.OverlapSphere(position, objectRadius, layerMask, QueryTriggerInteraction.Collide);
                if (colliders.Length == 0 && CheckPosInCircleBorder(position, centerPosition) == true)
                {
                    //places object in world and assigns collider radius
                    GameObject spawnedObject = SpawnObject(desiredPrefab, position, Quaternion.identity);
                    SphereCollider sCollider = spawnedObject.GetComponent<SphereCollider>();
                    sCollider.radius = objectRadius;

                    //adds object transform and gameObject to openList and allObjects respectively and resets the iteration counter  
                    openList.Add(spawnedObject.transform.position);
                    allObjects.Add(spawnedObject);
                    iterations = 0;
                }
                if (colliders.Length > 0)
                {
                    //position check failed and fail counter and iteration counter goes up
                    fails++;
                    iterations++;
                }
            }
            //provides details on open list, closed list and the amount of checks that failed while the object was going through the loop
            Debug.Log("Placement test failed: " + fails + ", || Open List count: " + openList.Count + " || Closed List count: " + closedList.Count);
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

        //assigns core object in open list and all objects list 
        bool gotFirstObject = false;
        int attempt = 0;
        while (!gotFirstObject)
        {
            Vector3 position;
            position.x = Random.Range(centerPosition.x - (XDiameter / 2), centerPosition.x + (XDiameter / 2));
            position.y = y;
            position.z = Random.Range(centerPosition.z - (ZDiameter / 2), centerPosition.z + (ZDiameter / 2));
            Collider[] colliders = Physics.OverlapSphere(position, objectRadius, layerMask, QueryTriggerInteraction.Collide);
            if (CheckPosInCircleBorder(position, centerPosition) == true)
            {
                openList.Add(position);
                GameObject spawnedObject = SpawnObject(desiredPrefab, position, Quaternion.identity);
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
                Collider[] colliders = Physics.OverlapSphere(position, objectRadius, layerMask, QueryTriggerInteraction.Collide);
                if (colliders.Length == 0 && CheckPosInQuadBorder(position, centerPosition) == true)
                {
                    //places object in world and assigns collider radius
                    GameObject spawnedObject = SpawnObject(desiredPrefab, position, Quaternion.identity);
                    SphereCollider sCollider = spawnedObject.GetComponent<SphereCollider>();
                    sCollider.radius = objectRadius;

                    //adds object transform and gameObject to openList and allObjects respectively and resets the iteration counter  
                    openList.Add(spawnedObject.transform.position);
                    allObjects.Add(spawnedObject);
                    iterations = 0;
                }
                if (colliders.Length > 0)
                {
                    //position check failed and fail counter and iteration counter goes up
                    fails++;
                    iterations++;
                }
            }
            //provides details on open list, closed list and the amount of checks that failed while the object was going through the loop
            Debug.Log("Placement test failed: " + fails + ", || Open List count: " + openList.Count + " || Closed List count: " + closedList.Count);
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
    public float GetObjectRadius()
    {
        return objectRadius;
    }

    bool CheckPosInCircleBorder(Vector3 position, Vector3 corePosition)
    {
        float distance = Mathf.Sqrt(Mathf.Pow(position.x - corePosition.x,2) + Mathf.Pow(position.z - corePosition.z,2));
        float radius = Mathf.Sqrt(Mathf.Pow(position.x, 2) + Mathf.Pow(position.z, 2));
        if(distance <= borderRadius)
        {
            return true;
        }
        return false;
    }

    bool CheckPosInQuadBorder(Vector3 position, Vector3 corePos)
    {
        float XMax = corePos.x + (XDiameter / 2);
        float XMin = corePos.x - (XDiameter / 2);
        float ZMax = corePos.z + (ZDiameter / 2);
        float ZMin = corePos.z - (ZDiameter / 2);

        if (position.x <= XMax && position.x >= XMin)
        {
            if (position.z <= ZMax && position.z >= ZMin)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

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

    GameObject SpawnObject(GameObject gameObject, Vector3 position, Quaternion rotation)
    {
        return Instantiate(gameObject, position, rotation);
    }

    public int GetObjectCount()
    {
        return allObjects.Count;
    }
    #endregion
}
