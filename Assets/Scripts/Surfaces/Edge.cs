using UnityEngine;



namespace Surfaces
{
    public class Edge
    {
        public Vector3 pointA;
        public Vector3 pointB;
        public Vector3 direction => Vector3.Normalize(pointA - pointB);
        public float length => Vector3.Magnitude(pointA - pointB);

        public Edge(Vector3 a, Vector3 b){
            pointA = a;
            pointB = b;
        }
    }
}