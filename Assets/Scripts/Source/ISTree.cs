using System.Collections.Generic;
using UnityEngine;
using ImageSources;
using Surfaces;
using System;



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

        public bool _beamTracing;

        public bool _beamClipping;

        private int _beam = 0;

        private int _realISs = 0;

        private bool _debugBeamTracing;

        private List<ReflectiveSurface> Surfaces => SurfaceManager.Instance.surfaces;

        public List<IS> Nodes => _nodes;

        public int R => _ro;





        // Creates a tree of Image Sources
        // N = number of surfaces
        // R = maximum order of reflections
        public ISTree(int n, int r, Vector3 sourcePos, bool wrongSideOfReflector, bool beamTracing, bool beamClipping, bool debugBeamTracing)
        {
            if (r == 0)
                return;
            
            _sn = n;
            _ro = r;
            _wrongSideOfReflector = wrongSideOfReflector;
            _beamTracing = beamTracing;
            _beamClipping = beamClipping;
            _debugBeamTracing = debugBeamTracing;

            List<int> firstNodeOfOrder = new()
            {
                0,
                0
            };

            List<Plane> projectionPlanes = null;

            // Creating the first order ISs
            for (int i = 0; i < _sn ; i++, _realISs++)
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
                    if (!_nodes[p].active)
                        continue;

                    // TODO: generate beam planes for the parent only here, to avoid repeating the operation for each child
                    projectionPlanes = CreateProjectionPlanes( _nodes[p].position, _nodes[p].beamPoints );

                    // Iterates on all surfaces, checking if a new IS can be derived from a reflection of the parent on them
                    for (int s = 0 ; s < _sn ; s++)
                    {
                        if ( CreateIS(i, p, s, projectionPlanes) )
                            i++;
                    }
                }
            }

            /*
            Sending to console a debug message showing the total number of ISs created and the amount saved by optimization.
            The number of not generated ISs is only the tip of the iceberg: their children would have also been generated.
            */
            Debug.Log(  "IS generation over\n" +
                        "Total number of ISs generated: " + _realISs + "\n" +
                        "Optimizations:\n" +
                        " - No reflection on same surface twice in a row: " + _noDouble + " ISs removed\n" +
                        " - Wrong side of reflector: " + _wrongSide + " ISs removed\n" +
                        " - Beam tracing" + (_beamClipping ? " + clipping" : "") + ": " + _beam + " ISs removed"
                     );
        }




        // This function checks all conditions for creating a new Image Source, then creates it if all are respected
        private bool CreateIS(int i, int parent, int surface, List<Plane> projectionPlanes)
        {
            // 1
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



            // 2
            // Checking that the IS is not on the wrong side of the reflector, standing on the opposite side of the surface's normal
            if ( _wrongSideOfReflector && Vector3.Dot( Surfaces[surface].normal , pos - Surfaces[surface].origin ) >= 0 )
            {
                _wrongSide++;
                return false;
            }



            // 3
            // Checking that reflections from parent to this surface are possible

            Beam beam = new( Surfaces[surface].Points , Surfaces[surface].Edges );

            if (_beamTracing)
            {
                Vector3 intersection;
                Vector3 secondIntersection;
                Vector3 inPoint;
                Vector3 outPoint;
                Vector3 otherPoint;
                Edge otherEdge;
                List<Edge> blackList = new();
                int e = 0;

                
                // For all 
                foreach (Plane plane in projectionPlanes)
                {
                    blackList.Clear();
                    e = 0;

                    while (e < beam.Edges.Count)
                    {
                        Edge edge = beam.Edges[e];

                        // Checks if the edge intersects the plane
                        if ( !blackList.Contains(edge) && LinePlaneIntersection( out intersection, edge.pointA, edge.pointB - edge.pointA, plane.normal, _nodes[parent].position ) )
                        {
                            bool doNothing = false;
                            bool intersectionOnExtreme = false;

                            // Check if intersection is on the edge extremes
                            if ((intersection - edge.pointA).magnitude <= 0.02f)
                            {
                                if ( Vector3.Dot( plane.normal, (edge.pointB - _nodes[parent].position).normalized ) >= 0 )
                                {
                                    doNothing = true;
                                }
                                else
                                {
                                    intersectionOnExtreme = true;
                                }
                            }
                            else if ((intersection - edge.pointB).magnitude <= 0.02f)
                            {
                                if ( Vector3.Dot( plane.normal, (edge.pointA - _nodes[parent].position).normalized ) >= 0 )
                                {
                                    doNothing = true;
                                }
                                else
                                {
                                    intersectionOnExtreme = true;
                                }
                            }


                            if (!doNothing)
                            {
                                // The point that is on the correct semispace of the plane (to be kept)
                                inPoint = Vector3.Dot( plane.normal, (edge.pointA - _nodes[parent].position).normalized ) > Vector3.Dot( plane.normal, (edge.pointB - _nodes[parent].position).normalized ) ? edge.pointA : edge.pointB;
                                // The point that is on the other semispace of the plane (to be removed)
                                outPoint = inPoint == edge.pointA ? edge.pointB : edge.pointA;

                                // Finds the other edge that the point to be removed belongs to
                                otherEdge = beam.FindOtherEdge(outPoint, inPoint);

                                if (otherEdge != null)
                                {

                                    // The other point to which outPoint is connected
                                    otherPoint = otherEdge.pointA == outPoint ? otherEdge.pointB : otherEdge.pointA;

                                    // Checks if the other edge has also intersections with the same plane
                                    if (LinePlaneIntersection( out secondIntersection, otherEdge.pointA, otherEdge.pointB - otherEdge.pointA, plane.normal, _nodes[parent].position))
                                    {
                                        // The second intersection is near the other point
                                        if ((secondIntersection - otherPoint).magnitude <= 0.02f)
                                        {
                                            if (intersectionOnExtreme)
                                            {
                                                beam.RemovePoint(outPoint);

                                                beam.RemoveEdge(edge);
                                                beam.RemoveEdge(otherEdge);

                                                blackList.Add( beam.AddEdge(inPoint, otherPoint) );
                                            }
                                            else
                                            {
                                                beam.RemovePoint(outPoint);

                                                beam.RemoveEdge(edge);
                                                beam.RemoveEdge(otherEdge);

                                                beam.AddPoint(intersection);

                                                blackList.Add( beam.AddEdge(intersection, inPoint) );
                                                blackList.Add( beam.AddEdge(intersection, otherPoint) );
                                            }
                                        }
                                        // The second intersection is near the outpoint
                                        else if ((intersection - outPoint).magnitude <= 0.02f)
                                        {
                                            blackList.Add(edge);
                                        }
                                        // The second intersection is not on the edge extremes
                                        else
                                        {
                                            if (intersectionOnExtreme)
                                            {
                                                beam.RemovePoint(outPoint);

                                                beam.RemoveEdge(edge);
                                                beam.RemoveEdge(otherEdge);

                                                beam.AddPoint(secondIntersection);

                                                blackList.Add( beam.AddEdge(inPoint, secondIntersection) );
                                                blackList.Add( beam.AddEdge(secondIntersection, otherPoint) );
                                            }
                                            else
                                            {
                                                beam.RemovePoint(outPoint);

                                                beam.RemoveEdge(edge);
                                                beam.RemoveEdge(otherEdge);

                                                beam.AddPoint(intersection);
                                                beam.AddPoint(secondIntersection);

                                                blackList.Add( beam.AddEdge(intersection, inPoint) );
                                                blackList.Add( beam.AddEdge(intersection, secondIntersection) );
                                                blackList.Add( beam.AddEdge(secondIntersection, otherPoint) );
                                            }
                                        }
                                    }
                                    // No second intersection on the other edge
                                    else
                                    {
                                        if (intersectionOnExtreme)
                                        {
                                            beam.RemovePoint(outPoint);
                                            
                                            beam.RemoveEdge(edge);
                                            beam.RemoveEdge(otherEdge);

                                            beam.AddEdge(inPoint, otherPoint);
                                        }
                                        else
                                        {
                                            beam.RemovePoint(outPoint);
                                            
                                            beam.RemoveEdge(edge);
                                            beam.RemoveEdge(otherEdge);

                                            beam.AddPoint(intersection);

                                            beam.AddEdge(intersection, inPoint);
                                            beam.AddEdge(intersection, otherPoint);
                                        }
                                    }

                                    e = 0;

                                }
                                else
                                {
                                    blackList.Add(edge);
                                    e++;
                                }
                            }
                            else
                            {
                                blackList.Add(edge);
                                e++;
                            }
                        }
                        else
                        {
                            blackList.Add(edge);
                            e++;
                        }
                    }
                }

                // If the resulting projection consists of 2 or less points (it's just a line or a point), no IS created
                if (beam.Points.Count <= 2 || beam.Edges.Count <= 2)
                {
                    _beam++;

                    if (_debugBeamTracing)
                    {
                        _nodes.Add( new IS(i, parent, surface, beam, false ) );
                        _nodes[i].position = pos;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                // Checking, for each point resulting from the projection, if it is in the correct semispace of all planes
                foreach (Vector3 point in beam.Points)
                {
                    foreach (Plane plane in projectionPlanes)
                    {
                        // If a point of the projection falls out of a semispace of the projection plane, then no IS is created
                        if ( Vector3.Dot( (point - _nodes[parent].position).normalized , plane.normal) < -0.05f )
                        {
                            _beam++;

                            if (_debugBeamTracing)
                            {
                                _nodes.Add( new IS(i, parent, surface, beam, false ) );
                                _nodes[i].position = pos;
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
            }



            // IS is created and its position is given
            if (_beamClipping)
                _nodes.Add( new IS(i, parent, surface, beam ) );
            else
                _nodes.Add( new IS(i, parent, surface, new( Surfaces[surface].Points , Surfaces[surface].Edges ) ) );

            _nodes[i].position = pos;

            _realISs++;

            return true;
        }



        // Given an IS position and the portion of the surface on which it needs to be projected, returns the set of planes passing from the IS to each edge
        private List<Plane> CreateProjectionPlanes(Vector3 position, Beam beam)
        {
            List<Plane> planes = new();
            foreach (Edge e in beam.Edges)
            {
                var normal = Vector3.Cross(e.pointA - position, e.pointB - position);

                normal = CheckNormal(normal, e.pointA, e.pointB, beam.Points) ? normal : -normal;

                planes.Add( new Plane( normal, position) );
            }

            return planes;
        }



        // Given a vector, an edge and a set of points (supposedly forming a convex polygon), checks if said vector is pointing in the direction of all points
        private bool CheckNormal(Vector3 normal, Vector3 pointA, Vector3 pointB, List<Vector3> points)
        {   
            foreach(Vector3 point in points)
            {
                if (point != pointA && point != pointB)
                    return Vector3.Dot( (point - pointA).normalized , normal) >= 0;
            }

            return true;
        }



        // Returns true if a plane and segment intersect, point of intersection is in output in the variable intersection
        public static bool LinePlaneIntersection(out Vector3 intersection, Vector3 linePoint, Vector3 lineVec, Vector3 planeNormal, Vector3 planePoint, double epsilon = 1e-6)
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

                if (length <= 0)
                    intersection = linePoint;
                else if (length >= lineVec.magnitude)
                    intersection = linePoint + lineVec;

                return length >= -0.02f && length <= lineVec.magnitude + 0.02f;
            }
            else
            {
                // The line and plane are parallel (nothing to do)
                return false;
            }
        }


    }
}