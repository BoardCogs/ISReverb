using System.Collections.Generic;
using Surfaces;
using UnityEngine;



namespace ImageSources
{

    public class Beam
    {
        private List<Vector3> _points = new();
        private List<Edge> _edges = new();

        public List<Vector3> Points => _points;
        public List<Edge> Edges => _edges;

        public Beam(List<Vector3> points, List<Edge> edges)
        {
            _points = points;
            _edges = edges;
        }

        public void RemovePoint(Vector3 point)
        {
            _points.Remove(point);
        }

        public void AddPoint(Vector3 point)
        {
            _points.Add(point); 
        }

        public void RemoveEdge(Edge edge)
        {
            _edges.Remove(edge);
        }

        public void AddEdge(Vector3 pointA, Vector3 pointB)
        {
            _edges.Add( new(pointA, pointB) );
        }

        // Given a and b, two points connected by an edge, finds the other edge that a belongs to
        public Edge FindOtherEdge(Vector3 a, Vector3 b)
        {
            foreach (var e in _edges)
            {
                if ( ( e.pointA == a || e.pointB == a ) && e.pointB != b && e.pointA != b )
                {
                    return e;
                }
            }

            return null;
        }
    }
}