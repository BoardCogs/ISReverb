using UnityEngine;
using Surfaces;
using ImageSources;
using System.Collections.Generic;
using System;



namespace Source
{

    public class Source : MonoBehaviour
    {
        // The Image Sources tree
        private ISTree tree;


        [Tooltip("The maximum order of reflection to be computed")]
        public int order;

        [Tooltip("Set to true to activate IS generation (only in play mode)")]
        public bool generateImageSources = false;

        [Tooltip("Set to true to visualize ISs (performance heavy)")]
        public bool drawImageSources = false;

        [Header("Optimizations")]

        [Tooltip("Set true to remove all ISs that fall on the front side of their reflecting surface")]
        public bool WrongSideOfReflector = true;
        public bool BeamTracing = true;
        public bool BeamClipping = true;

        [Header("Debug")]

        public bool drawPlaneProjection = true;

        public int checkNode = 0;

        [ReadOnly] public int parentNode = 0;



        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            GenerateISPositions();
        }



        private void GenerateISPositions()
        {
            tree = new(SurfaceManager.Instance.N, order, transform.position, WrongSideOfReflector, BeamTracing, BeamClipping);
        }



        void OnValidate()
        {
            if (generateImageSources == true)
            {
                GenerateISPositions();
                generateImageSources = false;
            }
        }



        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            // Draws Source
            Gizmos.DrawSphere(transform.position, 0.5f);

            // Draws Image Sources
            if (drawImageSources == true)
            {
                Gizmos.color = Color.green;

                if (tree != null)
                {
                    foreach (var node in tree.Nodes)
                    {
                        if (node != null)
                            Gizmos.DrawSphere(node.position, 0.5f);
                    }
                }
            }

            
            // Draws beam tracing and clipping process to debug
            if (checkNode != 0 && checkNode >= SurfaceManager.Instance.N && checkNode < tree.Nodes.Count)
            {
                // Draws the resulting beam projection on the reflector

                Gizmos.color = Color.red;

                IS node = tree.Nodes[checkNode];

                // Displays the parent node index as a readonly field
                parentNode = node.parent;

                Gizmos.DrawSphere(node.position, 0.7f);

                foreach (var edge in node.beamPoints.Edges)
                {
                    Gizmos.DrawLine(edge.pointA, edge.pointB);
                }

                // Creates plane on which the reflector lies

                Plane plane = new(node.beamPoints.Points[0], node.beamPoints.Points[1], node.beamPoints.Points[2]);

                // Draws the parent related gizmos

                Gizmos.color = Color.blue;

                IS nodeParent = tree.Nodes[node.parent];

                Gizmos.DrawSphere(nodeParent.position, 0.7f);

                // Draws parent beam points
                foreach (var edge in nodeParent.beamPoints.Edges)
                {
                    Gizmos.DrawLine(edge.pointA, edge.pointB);
                }

                List<Vector3> intersections = new();
                List<int> checks = new();
                Vector3 intersection;

                // Saves projection intersections on reflector plane
                foreach (var point in nodeParent.beamPoints.Points)
                {
                    int result = LinePlaneIntersection(out intersection, nodeParent.position, point-nodeParent.position, plane.normal, node.beamPoints.Points[0]);

                    if (result == -1)
                    {
                        intersections.Add(point + (point - nodeParent.position).normalized * 50);
                    }
                    else
                    {
                        intersections.Add(intersection);
                    }
                    
                    checks.Add(result);
                }

                // Draws projection beams
                foreach (var point in intersections)
                {
                    Gizmos.DrawLine(nodeParent.position, point + (point - nodeParent.position).normalized * 50);
                }

                // Draws projection of beam points and beam edges upon the reflector plane
                if (drawPlaneProjection)
                {
                foreach (var edge in nodeParent.beamPoints.Edges)
                    {
                        int indexA = nodeParent.beamPoints.Points.IndexOf(edge.pointA);
                        int indexB = nodeParent.beamPoints.Points.IndexOf(edge.pointB);

                        if (checks[indexA] == 1 && checks[indexB] == 1)
                        {
                            Gizmos.DrawLine(intersections[indexA], intersections[indexB]);
                        }
                    }
                }
            }
        }



        // Returns true if a plane and segment intersect, point of intersection is in output in the variable intersection
        public static int LinePlaneIntersection(out Vector3 intersection, Vector3 linePoint, Vector3 lineVec, Vector3 planeNormal, Vector3 planePoint, double epsilon = 1e-6)
        {
            float length;
            float dotNumerator;
            float dotDenominator;
            intersection = Vector3.zero;

            //calculate the distance between the linePoint and the line-plane intersection point
            dotNumerator = Vector3.Dot(planePoint - linePoint, planeNormal);
            dotDenominator = Vector3.Dot(lineVec.normalized, planeNormal);

            // Checks that plane and line are not parallel
            if ( Math.Abs(dotDenominator) > epsilon)
            {
                length = dotNumerator / dotDenominator;

                intersection = linePoint + lineVec.normalized * length;

                return length > 0 ? 1 : -1;
            }
            else
            {
                // The line and plane are parallel (nothing to do)
                return 0;
            }
        }
    }
}