using System.Collections.Generic;
using System.ComponentModel.Design;
using Surfaces;
using UnityEngine;



namespace ImageSources
{

    public class IS
    {
        
        // The index of this Image Source in its ISTree
        [ReadOnly] public int index;

        // The order of this IS
        [ReadOnly] public int order;

        // The index of this Image Source's parent
        [ReadOnly] public int parent;

        // The index of this Image Source's reflector
        [ReadOnly] public int surface;

        // The position of the Image Source
        [HideInInspector] public Vector3 position = new(0, 0, 0);

        // The points and edjes resulting from beam tracing on this surface's reflector from its parent IS
        [HideInInspector] public Beam beamPoints;

        // If false, the IS should have been removed
        [ReadOnly] public bool valid;

        // If true, the IS has a reflection path that reaches the listener
        [ReadOnly] public bool hasPath = true;

        // The reflection path followed by sound to reach the listener
        public List<Vector3> path = new();



        public IS(int i, int o, int p, int s, Beam beam, bool v = true)
        {
            index = i;
            order = o;
            parent = p;
            surface = s;
            beamPoints = beam;
            valid = v;
            hasPath = true;
            path = new();
        }



        public void SetPath(bool b, List<Vector3> p)
        {
            hasPath = b;
            path = p;
        }
    }
}