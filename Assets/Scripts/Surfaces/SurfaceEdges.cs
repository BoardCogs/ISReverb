using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;



namespace Surfaces
{

    [ExecuteInEditMode]
    public class SurfaceEdges : MonoBehaviour
    {
        private List<Edge> _edges = new();

        
        private void Awake() {
            var corner0 = transform.TransformPoint(new Vector3(5, 0, 5));
            var corner1 = transform.TransformPoint(new Vector3(-5, 0, 5));
            var corner2 = transform.TransformPoint(new Vector3(-5, 0, -5));
            var corner3 = transform.TransformPoint(new Vector3(5, 0, -5));

            _edges = new()
            {
                new Edge(corner0, corner1),
                new Edge(corner1, corner2),
                new Edge(corner2, corner3),
                new Edge(corner3, corner0)
            };
        }


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }


        // Update is called once per frame
        void Update()
        {
            
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