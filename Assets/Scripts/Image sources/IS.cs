using UnityEngine;



namespace ImageSources
{

    public class IS
    {
        
        // The index of this Image Source in its ISTree
        [ReadOnly] public int id;

        // The position of the Image Source
        [HideInInspector] public Vector3 position = new(0, 0, 0);



        public IS(int x)
        {
            id = x;
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