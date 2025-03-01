using UnityEngine;



namespace Surfaces
{

    public class SurfaceManager : Singleton<SurfaceManager>
    {
        
        protected override void Awake()
        {
            base.Awake();

            int i = 0;
            foreach (ReflectiveSurface s in GetComponentsInChildren<ReflectiveSurface>())
            {
                s.id = i;
                i++;
            }
        }
    }
}