using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;



namespace Surfaces
{

    [ExecuteInEditMode]
    public class SurfaceEdges : MonoBehaviour
    {
        private List<Vector3> _points = new();
        private List<Edge> _edges = new();

        public List<Vector3> Points => _points;
        public List<Edge> Edges => _edges;

        
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