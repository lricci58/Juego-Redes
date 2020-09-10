using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnidadHandler : MonoBehaviour 
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("right")){
            gameObject.transform.Translate(200f * Time.deltaTime, 0, 0);
        }
    }
}
