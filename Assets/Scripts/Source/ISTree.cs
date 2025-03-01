using System.Collections.Generic;
using UnityEngine;
using ImageSources;



namespace Source
{

    public class ISTree
    {

        // Number of surfaces
        private int _n;

        // Maximum order of reflections
        private int _r;

        // All nodes of the tree, each one identifying an Image Source
        private List<IS> _nodes = new();

        public List<IS> Nodes => _nodes;

        public int R => _r;


        // Creates a tree of Image Sources
        // N = number of surfaces
        // R = maximum order of reflections
        public ISTree(int n, int r)
        {
            _n = n;
            _r = r;

            for (int i = 0 ; i < SumPow(_n, _r) ; i++)
            {
                // If the node i has a parent, and their surface's index is the same, this reflection is impossible and no IS is created
                // Otherwise, new IS created
                if ( Parent(i) != -1 && SurfaceIndex(i) == SurfaceIndex(Parent(i)) )
                {
                    _nodes.Add(null);
                }
                else
                {
                    // TODO: here it needs to instantiate a new prefab, having attached the IS component
                    _nodes.Add(new IS(i));
                }
            }
        }





        // Given and index x, returns the order of reflection for that IS
        public int Order(int x)
        {
            int d = -1;
            int i = 0;

            while (d < x && i <= _r)
            {
                i++;
                d += IntPow(_n, i);
            }

            return i;
        }


        // Returns the index of the parent of the IS number x
        public int Parent(int x)
        {
            if (x < _n)
                return -1;

            // Each node is in a batch of exactly _N nodes, each representing a reflection on a different surface from the surface of the parent node
            // The reflection from a surface to itself is included to easily compute order, parent and surface of each node, but is set to null during the tree's construction
            // b = the numeral of the batch in which x resides wrt all the batches of the same order of x
            int b = ( x - SumPow(_n, Order(x) - 1) ) / _n;

            // Returns the numeral of the parent, obtained by adding s to the total number of nodes before the order of reflection of the parent
            return SumPow(_n, Order(x) - 2) + b;
        }


        // Returns the index of the surface on which the IS number x is reflected last
        public int SurfaceIndex(int x)
        {
            // Simply the module between the index and the number of surfaces
            return x % _n;
        }






        // Calculates f to the power of p as an integer
        private int IntPow(int f, int p)
        {
            int x = 1;

            for(int i = 0 ; i < p ; i++)
                x *= f;

            return x;
        }

        // Calculates the sum of f to the power of i, for i = 1, 2, ..., p
        private int SumPow(int f, int p)
        {
            int r = 0;

            for (int i = 1 ; i <= p ; i++)
                r += IntPow(f, i);

            return r;
        }
    }
}