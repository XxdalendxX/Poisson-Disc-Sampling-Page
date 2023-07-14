using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PoissonDiscAllignment))]
public class CustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ///Poisson Disc Allignment
        PoissonDiscAllignment pda = (PoissonDiscAllignment)target;

        SphereCollider collider = pda.GetComponent<SphereCollider>();
        collider.radius = pda.GetObjectRadius();

        if(GUILayout.Button("Perform Poisson Disc Allignment"))
        {
            pda.Execute();
        }

        if (GUILayout.Button("Clear All Objects"))
        {
            pda.ClearLists();
        }
    }
}
