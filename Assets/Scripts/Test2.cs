using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test2 : Test1
{
    public Test2()
    {
        num1 = 5f;
    }

    void Update()
    {
        RunSomething();
    }

    protected void RunSomething()
    {
        print("working");
    }
}
