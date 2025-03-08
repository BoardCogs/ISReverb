using UnityEngine;
using Surfaces;
using ImageSources;
using System.Collections.Generic;



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

        public int nodeCheck = 0;



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

            Gizmos.DrawSphere(transform.position, 0.5f);

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

            

            if (nodeCheck != 0 && nodeCheck >= SurfaceManager.Instance.N)
            {
                Gizmos.color = Color.red;

                IS node = tree.Nodes[nodeCheck];

                Gizmos.DrawSphere(node.position, 0.7f);

                foreach (var edge in node.beamPoints.Edges) {
                    Gizmos.DrawLine(edge.pointA, edge.pointB);
                }



                Gizmos.color = Color.blue;

                IS nodeParent = tree.Nodes[node.parent];

                Gizmos.DrawSphere(nodeParent.position, 0.7f);

                foreach (var edge in nodeParent.beamPoints.Edges) {
                    Gizmos.DrawLine(edge.pointA, edge.pointB);
                }

                foreach (var point in nodeParent.beamPoints.Points)
                {
                    Gizmos.DrawLine(point, point + (point-nodeParent.position).normalized * 50);
                }
            }
        }
    }
}