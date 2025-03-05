using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject target;

    public LayerMask layerMask;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Raycast result: " + Physics.Raycast(transform.position, target.transform.position - transform.position, Vector3.Magnitude(target.transform.position - transform.position), layerMask) );
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
