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

        public void AddEdge(Edge edge)
        {
            _edges.Add(edge);
        }

        // Finds the other point connected with an edge to a, given b as the already connected one
        public Vector3 FindOtherEdgePoint(Vector3 a, Vector3 b)
        {
            foreach (var e in _edges)
            {
                if ( e.pointA == a && e.pointB != b )
                {
                    _points.Remove(a);
                    _edges.Remove(e);
                    return e.pointB;
                }

                if ( e.pointB == a && e.pointA != b )
                {
                    _points.Remove(a);
                    _edges.Remove(e);
                    return e.pointA;
                }
            }

            return Vector3.zero;
        }
    }
}