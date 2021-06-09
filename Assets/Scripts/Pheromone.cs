using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pheromone : MonoBehaviour
{
    public float strength = 0;

    private void FixedUpdate()
    {
        strength -= 0.001f;
        if (strength <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
