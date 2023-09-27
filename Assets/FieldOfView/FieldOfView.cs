using System;
using System.Collections.Generic;
using UnityEngine;

namespace S_Utils.FoV
{
    public class FieldOfView : MonoBehaviour
    {
        public float viewRadius;
        [Range(0,360)]
        public float viewAngle;

        public LayerMask targetLayer;
        public LayerMask obstacleLayer;

        [HideInInspector] public List<Transform> visibleTargets = new List<Transform>();
        public float meshResolution;
        public int edgeResolveIterations;
        public float edgeDstThreshold;
        
        public MeshFilter viewMeshFilter;
        private Mesh viewMesh;

        private void Start()
        {
            viewMesh = new Mesh ();
            viewMesh.name = "View Mesh";
            viewMeshFilter.mesh = viewMesh;
            InvokeRepeating(nameof(FindVisibleTargets),0.2f,0.2f);
        }

        private void LateUpdate()
        {
            DrawFieldOfView();
        }

        void FindVisibleTargets()
        {
            visibleTargets.Clear();
            Collider[] targetsInRadius = Physics.OverlapSphere(transform.position, viewRadius, targetLayer);

            for (int i = 0; i < targetsInRadius.Length; i++)
            {
                Transform target = targetsInRadius[i].transform;
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                
                if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, target.position);

                    if (!Physics.Raycast(transform.position, dirToTarget, distanceToTarget, obstacleLayer))
                    {
                        visibleTargets.Add(target);
                    }
                }
            }
        }

        void DrawFieldOfView()
        {
            int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);

            float stepAngleSize = viewAngle / stepCount;
            List<Vector3> viewPoints = new List<Vector3>();

            ViewCastInfo oldViewCast = new ViewCastInfo();
            for (int i = 0; i <= stepCount; i++)
            {
                float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
                ViewCastInfo viewCastInfo = ViewCast(angle);
                viewPoints.Add(viewCastInfo.Point);
                oldViewCast = viewCastInfo;
                if (i > 0)
                {
                    bool edgeDstThresholdExceeded =
                        Mathf.Abs(oldViewCast.Distance - viewCastInfo.Distance) > edgeDstThreshold;
                    if (oldViewCast.Hit != viewCastInfo.Hit || (oldViewCast.Hit && viewCastInfo.Hit && edgeDstThresholdExceeded))
                    {
                        EdgeInfo edge = FindEdge(oldViewCast, viewCastInfo);
                        if(edge.PointA!= Vector3.zero)
                            viewPoints.Add(edge.PointA);
                        if(edge.PointB != Vector3.zero)
                            viewPoints.Add(edge.PointB);
                    }
                }
            }

            int vertexCount = viewPoints.Count + 1;
            Vector3[] vertices = new Vector3[vertexCount];
            int[] triangles = new int[(vertexCount-2) * 3];

            vertices [0] = Vector3.zero;
            for (int i = 0; i < vertexCount - 1; i++) {
                vertices [i + 1] = transform.InverseTransformPoint(viewPoints [i]);

                if (i < vertexCount - 2) {
                    triangles [i * 3] = 0;
                    triangles [i * 3 + 1] = i + 1;
                    triangles [i * 3 + 2] = i + 2;
                }
            }

            viewMesh.Clear ();

            viewMesh.vertices = vertices;
            viewMesh.triangles = triangles;
            viewMesh.RecalculateNormals ();
        }

        ViewCastInfo ViewCast(float globalAngel)
        {
            Vector3 dir = DirectionFormAngle(globalAngel, true);
            RaycastHit hit;

            if (Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleLayer))
            {
                return new ViewCastInfo(true, hit.point, hit.distance, globalAngel);
            }
            else
            {
                
                return new ViewCastInfo(false, transform.position+dir*viewRadius, viewRadius, globalAngel);
            }
        }

        EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
        {
            float minAngle = minViewCast.Angle;
            float maxAngle = maxViewCast.Angle;
            Vector3 minPoint = Vector3.zero;
            Vector3 maxPoint = Vector3.zero;

            for (int i = 0; i < edgeResolveIterations; i++)
            {
                float angle = (minAngle + maxAngle) / 2;
                ViewCastInfo newViewCastInfo = ViewCast(angle);
                
                bool edgeDstThresholdExceeded =
                    Mathf.Abs(minViewCast.Distance - newViewCastInfo.Distance) > edgeDstThreshold;
                if (newViewCastInfo.Hit == minViewCast.Hit && !edgeDstThresholdExceeded)
                {
                    minAngle = angle;
                    minPoint = newViewCastInfo.Point;
                }
                else
                {
                    maxAngle = angle;
                    maxPoint = newViewCastInfo.Point;
                }

                
            }
            return new EdgeInfo(minPoint, maxPoint);
        }
        
        public Vector3 DirectionFormAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
            {
                angleInDegrees += transform.eulerAngles.y;
            }
            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }

        public struct ViewCastInfo
        {
            public bool Hit;
            public Vector3 Point;
            public float Distance;
            public float Angle;

            public ViewCastInfo(bool hit, Vector3 point, float distance, float angle)
            {
                Hit = hit;
                Point = point;
                Distance = distance;
                Angle = angle;
            }
        }
        
        public struct EdgeInfo
        {
            public Vector3 PointA;
            public Vector3 PointB;

            public EdgeInfo(Vector3 pointA, Vector3 pointB)
            {
                PointA = pointA;
                PointB = pointB;
            }
        }
    }
}