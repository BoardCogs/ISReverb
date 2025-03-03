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



        public IS(int i, int p, int s)
        {
            id = i;
            parent = p;
            surface = s;
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