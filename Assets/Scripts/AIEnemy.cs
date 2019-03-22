using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIEnemy : MonoBehaviour
{
    protected bool grounded = true;

    public void SetGrounded(bool isGrounded)
    {
        grounded = isGrounded;
    }
}
