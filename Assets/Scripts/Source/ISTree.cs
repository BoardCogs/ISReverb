using System.Collections.Generic;
using UnityEngine;
using ImageSources;
using Surfaces;
using Unity.Mathematics;



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

        private int _noDouble = 0;

        private bool _wrongSideOfReflector;

        private int _wrongSide = 0;

        private List<ReflectiveSurface> Surfaces => SurfaceManager.Instance.surfaces;

        public List<IS> Nodes => _nodes;

        public int R => _ro;





        // Creates a tree of Image Sources
        // N = number of surfaces
        // R = maximum order of reflections
        public ISTree(int n, int r, Vector3 sourcePos, bool wrongSideOfReflector)
        {
            if (r == 0)
                return;
            
            _sn = n;
            _ro = r;
            _wrongSideOfReflector = wrongSideOfReflector;

            List<int> firstNodeOfOrder = new()
            {
                0,
                0
            };

            for (int i = 0; i < _sn ; i++)
            {
                _nodes.Add(new IS(i, -1, i));

                var pos = sourcePos;
                    
                pos -= 2 * Vector3.Dot( Surfaces[i].normal , pos - Surfaces[i].origin ) * Surfaces[i].normal;

                _nodes[i].position = pos;

            }

            for (int i = _sn, order = 2 ; order <= _ro ; order++)
            {
                firstNodeOfOrder.Add(i);

                for (int p = firstNodeOfOrder[order-1] ; p < firstNodeOfOrder[order] ; p++)
                {
                    for (int s = 0 ; s < _sn ; s++)
                    {
                        if ( CreationCondition(i, p, s) )
                            i++;
                    }
                }
            }

            /*
            Sending to console a debug message showing the total number of ISs created and the amount saved by optimization.
            The number of not generated ISs is only the tip of the iceberg: their children would have also been generated.
            */
            Debug.Log(  "IS generation over\n" +
                        "Total number of ISs generated: " + _nodes.Count + "\n" +
                        "Optimizations:\n" +
                        " - No reflection on same surface twice in a row: " + _noDouble + " ISs removed\n" +
                        " - Wrong side of reflector: " + _wrongSide + " ISs removed"
                     );
        }




        // Condition for the creation of a new Image Source
        private bool CreationCondition(int i, int parent, int surface)
        {
            // Checking that no IS is created identifying a reflection on the same surface twice in a row
            // This is because a double reflection is impossible assuming flat surfaces
            if ( surface == _nodes[parent].surface )
            {
                _noDouble++;
                return false;
            }

            // Computing the position of the new IS by mirroring its parent along the reflecting surface
            var pos = _nodes[parent].position;
            pos -= 2 * Vector3.Dot( Surfaces[surface].normal , pos - Surfaces[surface].origin ) * Surfaces[surface].normal;

            // Checking that the IS is not on the wrong side of the reflector, standing on the opposite side of the surface's normal
            if ( _wrongSideOfReflector && Vector3.Dot( Surfaces[surface].normal , pos - Surfaces[surface].origin ) >= 0 )
            {
                _wrongSide++;
                return false;
            }
            

            // IS is created and its position is given

            _nodes.Add(new IS(i, parent, surface));
            _nodes[i].position = pos;

            return true;
        }

    }
}