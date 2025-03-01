using System.Collections.Generic;
using UnityEngine;



namespace Surfaces
{

    public class SurfaceManager : Singleton<SurfaceManager>
    {
        public List<ReflectiveSurface> surfaces;

        public int N;

        protected override void Awake()
        {
            base.Awake();

            surfaces = new ( GetComponentsInChildren<ReflectiveSurface>() );

            int i = 0;
            foreach (ReflectiveSurface s in surfaces)
            {
                s.id = i;
                i++;
            }

            N = i;
        }
    }
}