using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test1 : MonoBehaviour
{
    protected float num1 = 9f;

    // Start is called before the first frame update
    void Start()
    {
        RunSomething();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void RunSomething()
    {
        print("test 1 script with num: " + num1);
        print("Transform: " + transform);
    }
}
