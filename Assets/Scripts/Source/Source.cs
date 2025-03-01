using UnityEngine;



namespace Source
{

    public class Source : MonoBehaviour
    {
        
        public int order = 2;

        public bool generateISs = false;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }


        void OnValidate()
        {
            if (generateISs == true)
            {
                generateISs = false;
            }
        }


        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Gizmos.DrawSphere(transform.position, 0.2f);
        }
    }
}