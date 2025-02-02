using UnityEngine;



namespace Surfaces
{
    public class Edge
    {
        public Vector3 pointA;
        public Vector3 pointB;
        public Vector3 direction => pointA - pointB;

        public Edge(Vector3 A, Vector3 B){
            pointA = A;
            pointB = B;
        }
    }
}