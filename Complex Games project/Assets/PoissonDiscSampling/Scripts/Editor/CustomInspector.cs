using UnityEngine;
using UnityEditor;

namespace PoissonDiscSampling
{

    [CustomEditor(typeof(PoissonDiscAllignment))]
    public class CustomInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            ///Poisson Disc Allignment
            PoissonDiscAllignment pda = (PoissonDiscAllignment)target;

            EditorGUILayout.Space();
            pda.centerPosition = EditorGUILayout.Vector3Field("Center Position", pda.centerPosition);

            EditorGUILayout.Space();
            pda.circleArea = GUILayout.Toggle(pda.circleArea, "Fill a circlular area");
            if (pda.circleArea)
            {
                pda.borderRadius = EditorGUILayout.Slider(pda.borderRadius, 1.0f, 1000.0f);
            }
            else
            {
                EditorGUILayout.PrefixLabel("X Diameter");
                pda.XDiameter = EditorGUILayout.FloatField(pda.XDiameter);
                EditorGUILayout.PrefixLabel("Z Diameter");
                pda.ZDiameter = EditorGUILayout.FloatField(pda.ZDiameter);
            }

            if (!pda.circleArea && pda.ZDiameter != 0 && pda.XDiameter != 0 && !pda.placedObjects)
            {

                if (GUILayout.Button("Perform Poisson Disc Allignment"))
                {
                    pda.Execute();
                }
            }
            else if (pda.circleArea && !pda.placedObjects)
            {
                if (GUILayout.Button("Perform Poisson Disc Allignment"))
                {
                    pda.Execute();
                }
            }

            EditorGUILayout.Space();

            if (pda.placedObjects)
            {
                if (GUILayout.Button("Clear All Objects"))
                {
                    pda.ClearObjects();
                }
            }
        }
    }
}
