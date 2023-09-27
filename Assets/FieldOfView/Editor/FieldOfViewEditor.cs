using UnityEditor;
using UnityEngine;

namespace S_Utils.FoV.Editor
{
    [CustomEditor(typeof(FieldOfView))]
    public class FieldOfViewEditor : UnityEditor.Editor
    {
        void OnSceneGUI()
        {
            FieldOfView fov = (FieldOfView)target;
            Handles.color = Color.white;
            Handles.DrawWireArc(fov.transform.position,Vector3.up, Vector3.forward, 360,fov.viewRadius);

            Vector3 viewAngleA = fov.DirectionFormAngle(fov.viewAngle / 2, false);
            Vector3 viewAngleB = fov.DirectionFormAngle(-fov.viewAngle / 2, false);
            
            Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA*fov.viewRadius);
            Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB*fov.viewRadius);

            Handles.color = Color.red;

            foreach (var target in fov.visibleTargets)
            {
                Handles.DrawLine(fov.transform.position,target.position);
            }
        }
    }
}