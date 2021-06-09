using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SensorResponse
{
    public float wonderStrength, foodStrength;
    public SensorResponse(float WonderStrength, float FoodStrength)
    {
        wonderStrength = WonderStrength;
        foodStrength = FoodStrength;
    }
}

public class SensorHandler : MonoBehaviour
{
    public float foodStrength = 0;
    public float wonderStrength = 0;
    public GameObject Phero;


    public SensorResponse GetSenses()
    {
        wonderStrength = 0;
        foodStrength = 0;

        foreach (var obj in Physics2D.OverlapCircleAll(transform.position, 0.05f))
        {
            if (obj.name.Contains("FoodPhero"))
            {
                if (obj != null)
                {
                    foodStrength += obj.GetComponent<Pheromone>().strength;
                }
            }
            else if (obj.name.Contains("WonderPhero"))
            {
                Phero = obj.gameObject;
                if (obj != null)
                {
                    wonderStrength += obj.GetComponent<Pheromone>().strength;
                }
            }
        }
        return new SensorResponse(wonderStrength, foodStrength);
    }

}
