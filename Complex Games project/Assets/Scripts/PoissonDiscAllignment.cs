using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoissonDiscAllignment : MonoBehaviour
{
    [SerializeField] GameObject desiredPrefab;
    GameObject epicenter;

    [Space]
    [SerializeField] int layerNumber = 0;

    [Header("Radius's")]

    [SerializeField] float objectRadius = 5f;
    [SerializeField] float borderRadius = 5f;

    [Space]
    [SerializeField] int attempts = 5;

    [Space]
    [SerializeField] bool mustStop = false;
    [SerializeField] bool oneTime = false;
    
    public void Execute()
    {
        epicenter = this.gameObject;
        StartCoroutine(Perform());
    }

    IEnumerator Perform()
    {
        float y = epicenter.transform.position.y;
        
        List<GameObject> openList = new List<GameObject>();
        openList.Add(epicenter);
        List<GameObject> closedList = new List<GameObject>();
        
        while(!mustStop)
        {
            //for int < attempts
            //check objects around
            //spawn object
            
            SpawnObject(desiredPrefab, FindLocation(y), Quaternion.identity);

            if(oneTime == true)
            {
               mustStop = true;
            }
        }
        yield return null;
    }

    public float GetObjectRadius()
    {
        return objectRadius;
    }

    Vector3 FindLocation(float y)
    {
        var angle = Random.Range(0, 360);
        var x = Mathf.Cos(angle) * objectRadius * 2;
        var z = Mathf.Sin(angle) * objectRadius * 2;
        return new Vector3(x,y,z);
    }

    void SpawnObject(GameObject gameObject, Vector3 position, Quaternion rotation)
    {
        Instantiate(gameObject, position, rotation);
    }
}
