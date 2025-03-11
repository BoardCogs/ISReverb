using System.Collections.Generic;
using System.ComponentModel.Design;
using Surfaces;
using UnityEngine;



namespace ImageSources
{

    public class IS
    {
        
        // The index of this Image Source in its ISTree
        [ReadOnly] public int id;

        // The index of this Image Source's parent
        [ReadOnly] public int parent;

        // The index of this Image Source's reflector
        [ReadOnly] public int surface;

        // The position of the Image Source
        [HideInInspector] public Vector3 position = new(0, 0, 0);

        // The points and edjes resulting from beam tracing on this surface's reflector from its parent IS
        [HideInInspector] public Beam beamPoints;

        // If false, the IS should have been removed
        [ReadOnly] public bool active;



        public IS(int i, int p, int s, Beam beam, bool a = true)
        {
            id = i;
            parent = p;
            surface = s;
            beamPoints = beam;
            active = a;
        }


        /*
        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            Gizmos.DrawSphere(position, 0.1f);
        }
        */
    }
}