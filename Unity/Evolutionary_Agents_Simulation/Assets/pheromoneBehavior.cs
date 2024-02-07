using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pheromoneBehavior : MonoBehaviour
{

    // Time in seconds before the object is destroyed
    public float evaporationConstant = 5f;
    
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, evaporationConstant);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
