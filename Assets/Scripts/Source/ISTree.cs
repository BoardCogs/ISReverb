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

            // Creating the first order ISs
            for (int i = 0; i < _sn ; i++)
            {
                _nodes.Add( new IS( i, -1, i, new( Surfaces[i].Points , Surfaces[i].Edges ) ) );

                var pos = sourcePos;
                    
                pos -= 2 * Vector3.Dot( Surfaces[i].normal , pos - Surfaces[i].origin ) * Surfaces[i].normal;

                _nodes[i].position = pos;

            }

            // Creating all ISs from second order onward
            for (int i = _sn, order = 2 ; order <= _ro ; order++)
            {
                // Sets the first IS of the currently considered order of reflection
                firstNodeOfOrder.Add(i);

                // Checks on all ISs belonging to the previous order, acting as parents for new Image Sources
                for (int p = firstNodeOfOrder[order-1] ; p < firstNodeOfOrder[order] ; p++)
                {
                    // TODO: generate beam planes for the parent only here, to avoid repeating the operation for each child

                    // Iterates on all surfaces, checking if a new IS can be derived from a reflection of the parent on them
                    for (int s = 0 ; s < _sn ; s++)
                    {
                        if ( CreateIS(i, p, s) )
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




        // This function checks all conditions for creating a new Image Source, then creates it if all are respected
        private bool CreateIS(int i, int parent, int surface)
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

            _nodes.Add( new IS(i, parent, surface, new( Surfaces[surface].Points , Surfaces[surface].Edges ) ) );
            _nodes[i].position = pos;

            return true;
        }

    }
}