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

        private List<ReflectiveSurface> surfaces => SurfaceManager.Instance.surfaces;

        // The order of reflections for this source
        public int order;

        // Set to true to activate IS generation (only in play mode)
        public bool generateImageSources = false;



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
            if (tree == null || tree.R != order)
                tree = new(SurfaceManager.Instance.N, order);

            Vector3 p;

            for (int i = 0 ; i < tree.Nodes.Count ; i++)
            {
                if (tree.Nodes[i] == null || ( tree.Parent(i) != -1 && tree.Nodes[tree.Parent(i)] == null ) )
                    continue;

                p = tree.Parent(i) != -1 ? tree.Nodes[tree.Parent(i)].position : transform.position;
                
                p -= 2 * Vector3.Dot( surfaces[tree.SurfaceIndex(i)].normal , p - surfaces[tree.SurfaceIndex(i)].origin ) * surfaces[tree.SurfaceIndex(i)].normal;

                tree.Nodes[i].position = p;
            }
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