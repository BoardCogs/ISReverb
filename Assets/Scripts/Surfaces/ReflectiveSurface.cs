using System.Collections.Generic;
using UnityEngine;



namespace Surfaces
{

    [ExecuteInEditMode]
    [RequireComponent (typeof (Collider))]
    public class ReflectiveSurface : MonoBehaviour
    {

        private List<Vector3> _points = new();
        private List<Edge> _edges = new();

        public List<Vector3> Points => new(_points);
        public List<Edge> Edges => new(_edges);
        
        public Vector3 normal => transform.TransformPoint(0, 1, 0) - origin;
        public Vector3 origin => transform.TransformPoint(0, 0, 0);

        [HideInInspector] public int colliderID;

        [ReadOnly] public int id;
        
        private void Awake() {
            _points = new()
            {
                transform.TransformPoint(new Vector3(5, 0, 5)),
                transform.TransformPoint(new Vector3(-5, 0, 5)),
                transform.TransformPoint(new Vector3(-5, 0, -5)),
                transform.TransformPoint(new Vector3(5, 0, -5))
            };

            _edges = new()
            {
                new Edge(_points[0], _points[1]),
                new Edge(_points[1], _points[2]),
                new Edge(_points[2], _points[3]),
                new Edge(_points[3], _points[0])
            };

            colliderID = GetComponent<Collider>().GetInstanceID();
        }


        // Called each frame to draw gizmos
        private void OnDrawGizmos() {
            Gizmos.color = Color.blue;

            foreach (var edge in _edges) {
                Gizmos.DrawLine(edge.pointA, edge.pointB);
            }
        }
    }

}