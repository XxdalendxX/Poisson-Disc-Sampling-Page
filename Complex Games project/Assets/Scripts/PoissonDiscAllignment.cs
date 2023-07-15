using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoissonDiscAllignment : MonoBehaviour
{
    [SerializeField] GameObject desiredPrefab;
    GameObject epicenter;

    [Space]
    [SerializeField] int layerNumber = 0;
    int layerMask;

    [Header("Radius's")]

    [SerializeField] float objectRadius = 5f;
    [SerializeField] float borderRadius = 5f;

    [Space]
    [SerializeField, Tooltip("A multiplier put onto the object radius for it's maximum value"), Range(2, 6)] float maxObjectRadiusMultiplier = 5f;

    [Space]
    [SerializeField] int attempts = 5;
    
    [Space]
    [SerializeField] List<GameObject> allObjects = new List<GameObject>();

    public void Execute()
    {
        epicenter = this.gameObject;
        layerMask = LayerMask.GetMask(LayerMask.LayerToName(layerNumber));
        //StartCoroutine(CoroutinePerform());
        Perform();
    }

    public void ClearLists()
    {
        int obj = allObjects.Count;
        for (int i = 0; i < obj - 1; i++)
        {
            DestroyImmediate(allObjects[1]);
            allObjects.RemoveAt(1);
        }
        allObjects.RemoveAt(0);
    }

    IEnumerator CoroutinePerform()
    {
        float y = epicenter.transform.position.y;
        int iterations = 0;
        List<Vector3> openList = new List<Vector3>();
        List<Vector3> closedList = new List<Vector3>();

        openList.Add(epicenter.transform.position);
        allObjects.Add(epicenter);

        bool mustStop = false;

        while(!mustStop)
        {
            if (openList.Count < 1)
            {
                mustStop = true;
                break;
            }
            Vector3 currentObjectPos = openList[0];

            int fails = 0;
            while (iterations <= attempts)
            {
                
                Vector3 position = FindLocation(y, currentObjectPos);
                Collider[] colliders = Physics.OverlapSphere(position, objectRadius, layerMask , QueryTriggerInteraction.Collide);
                if(colliders.Length == 0 && CheckPosInBorder(position) == true)
                {
                    GameObject spawnedObject = SpawnObject(desiredPrefab, position, Quaternion.identity);
                    SphereCollider sCollider = spawnedObject.GetComponent<SphereCollider>();
                    sCollider.radius = objectRadius;
                    openList.Add(spawnedObject.transform.position);
                    allObjects.Add(spawnedObject);
                    iterations = 0;
                }
                if(colliders.Length > 0)
                {
                    fails++;
                    iterations++;
                }
            }
            Debug.Log("Placement test failed " + fails + " times");
            iterations = 0;
            fails = 0;
            openList.RemoveAt(0);
            closedList.Add(currentObjectPos);
            yield return null;
            //yield return new WaitForSeconds(0.01f);
        }
        Debug.Log("PDS complete");
        yield return null;
    }

    void Perform()
    {
        float y = epicenter.transform.position.y;
        int iterations = 0;
        List<Vector3> openList = new List<Vector3>();
        List<Vector3> closedList = new List<Vector3>();

        openList.Add(epicenter.transform.position);
        allObjects.Add(epicenter);

        bool mustStop = false;

        while (!mustStop)
        {
            if (openList.Count < 1)
            {
                mustStop = true;
                break;
            }
            Vector3 currentObjectPos = openList[0];

            int fails = 0;
            while (iterations <= attempts)
            {

                Vector3 position = FindLocation(y, currentObjectPos);
                Collider[] colliders = Physics.OverlapSphere(position, objectRadius, layerMask, QueryTriggerInteraction.Collide);
                if (colliders.Length == 0 && CheckPosInBorder(position) == true)
                {
                    GameObject spawnedObject = SpawnObject(desiredPrefab, position, Quaternion.identity);
                    SphereCollider sCollider = spawnedObject.GetComponent<SphereCollider>();
                    sCollider.radius = objectRadius;
                    openList.Add(spawnedObject.transform.position);
                    allObjects.Add(spawnedObject);
                    iterations = 0;
                }
                if (colliders.Length > 0)
                {
                    fails++;
                    iterations++;
                }
            }
            Debug.Log("Placement test failed: " + fails + ", || Open List count: " + openList.Count + " || Closed List count: " + closedList.Count );
            iterations = 0;
            fails = 0;
            openList.RemoveAt(0);
            closedList.Add(currentObjectPos);
        }
        Debug.Log("PDS complete");
    }

    public float GetObjectRadius()
    {
        return objectRadius;
    }

    bool CheckPosInBorder(Vector3 position)
    {
        float radius = Mathf.Sqrt(Mathf.Pow(position.x, 2) + Mathf.Pow(position.z, 2));
        if(radius <= borderRadius)
        {
            return true;
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
}
