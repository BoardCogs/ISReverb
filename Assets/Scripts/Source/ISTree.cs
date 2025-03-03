using System.Collections.Generic;
using UnityEngine;
using ImageSources;



namespace Source
{

    public class ISTree
    {

        // Number of surfaces
        private int _sn;

        // Maximum order of reflections
        private int _ro;

        // All nodes of the tree, each one identifying an Image Source
        private List<IS> _nodes = new();

        public List<IS> Nodes => _nodes;

        public int R => _ro;


        // Creates a tree of Image Sources
        // N = number of surfaces
        // R = maximum order of reflections
        public ISTree(int n, int r)
        {
            if (r == 0)
                return;
            
            _sn = n;
            _ro = r;
            List<int> firstNodeOfOrder = new()
            {
                0,
                0
            };

            for (int i = 0; i < _sn ; i++)
            {
                _nodes.Add(new IS(i, -1, i));
            }

            for (int i = _sn, order = 2 ; order <= _ro ; order++)
            {
                firstNodeOfOrder.Add(i);

                for (int p = firstNodeOfOrder[order-1] ; p < firstNodeOfOrder[order] ; p++)
                {
                    for (int s = 0 ; s < _sn ; s++)
                    {
                        if ( CreationCondition(s, p) )
                        {
                            _nodes.Add(new IS(i, p, s));
                            i++;
                        }
                    }
                }
            }
        }




        // Condition for the creation of a new Image Source
        private bool CreationCondition(int s, int p)
        {
            return s != _nodes[p].surface;
        }

    }
}