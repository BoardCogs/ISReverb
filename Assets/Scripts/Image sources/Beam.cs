using System.Collections.Generic;
using Surfaces;
using UnityEngine;



namespace ImageSources
{

    public class Beam : MonoBehaviour
    {
        private List<Vector3> _points = new();
        private List<Edge> _edges = new();

        public List<Vector3> Points => _points;
        public List<Edge> Edges => _edges;

        

    }
}