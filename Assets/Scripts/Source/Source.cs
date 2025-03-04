using UnityEngine;
using Surfaces;
using UnityEngine.UIElements;
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



        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            GenerateISPositions();
        }



        // Update is called once per frame
        void Update()
        {
            
        }



        private void GenerateISPositions()
        {
            tree = new(SurfaceManager.Instance.N, order, transform.position, WrongSideOfReflector);
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
        }
    }
}