using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeHandler : MonoBehaviour
{
    public int NumberOfAnts;
    public List<GameObject> Ants;
    public GameObject AntObject;

    IEnumerator Start()
    {
        for (int j = 1; j < NumberOfAnts; j++)
        {
            GameObject ant = Instantiate(AntObject, transform.position, transform.rotation);
            Ants.Add(ant);

            yield return new WaitForSeconds(0.1f);
        }
    }
}