using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ant : MonoBehaviour
{
    //Stats
    public float maxSpeed = 2;                  //Maximum speed the ant can travel
    public float steerStrength = 2;             //Strength the ant can turn
    public float wanderStrength = 0.1f;         //The distance the next point of interest is from the last
    public float pheromoneDistance = 0.2f;      //Distance between each pheromone dropped

    Vector2 position;                          
    Vector2 velocity;                           
    Vector2 desiredDirection;

    float viewAngle = 45f;                      //Angle in which the ant can see
    float viewRadius = 2f;                      //Eye sight distance
    float pickUpRadius = 0.3f;                  //Distance ant can pick up food
    float appliedSteeringForce = 1f;            //Additional force applied to steering force, changed when in focus



    //Pheromone objects
    public List<GameObject> pheromoneTypes = new List<GameObject>();    // 1=Wonder, 2=Food
    private Transform previousPheromone = null;                         //Chanege this to an array

    //Sensors   
    public GameObject[] sensors;                    //Collection of sensors (trigger boxes) attached to this game object
    public SensorResponse[] sensorResponses;        //Data collected from each sensor
    private int bestSensor = 0;
            
    private Transform targetFood = null;            //Food walking towards
    private Transform foodCarrying = null;          //Food carrying with
    private Vector3 carryingFoodOffset = new Vector3(0.3f, 0, 0);
    private bool foodTrailFound = false;


    // Start is called before the first frame update
    void Start()
    {
        //Add sensors to array
        GameObject[] s = { transform.Find("Sensors").GetChild(0).gameObject, transform.Find("Sensors").GetChild(1).gameObject, transform.Find("Sensors").GetChild(2).gameObject };
        sensors = s;
    }

    // Update is called once per frame
    void Update()
    {
        NewDirection();
        DropPheromones();
        UpdateSensor();

        if (foodCarrying != null)
        {
            foodCarrying.transform.localPosition = carryingFoodOffset;
        }
        else
        {
            CheckForFood();
        }
    }

    void NewDirection()
    {
        //If haven't found food, wonder
        if (targetFood == null)
        {
            if (foodTrailFound)
            {
                //Go towards strongest sensor
                desiredDirection = (sensors[bestSensor].transform.position - transform.position).normalized;
                appliedSteeringForce = 20f;
            }

            //Random Direction all the time within the unity circle
            desiredDirection = (desiredDirection + Random.insideUnitCircle * wanderStrength).normalized;
            appliedSteeringForce = 1;
        }

        //turn around to previous pheromone and follow pheromone trails back
        /*else if (turningAround)
        {

        }*/

        //If found food, walk towards it
        else
        {
            //Random Direction all the time within the unity circle
            desiredDirection = (targetFood.position - transform.position).normalized;
            appliedSteeringForce = 8;
        }

        Debug.Log("app force: " + appliedSteeringForce);
        //Add speed to the direction travelling
        Vector2 desiredVelocity = desiredDirection * maxSpeed;
        //Rotation speed
        Vector2 desiredSteeringForce = (desiredVelocity - velocity) * steerStrength;
        Vector2 acceleration = Vector2.ClampMagnitude(desiredSteeringForce * appliedSteeringForce, steerStrength);

        velocity = Vector2.ClampMagnitude(velocity + acceleration * Time.deltaTime, maxSpeed);
        position += velocity * Time.deltaTime;

        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.SetPositionAndRotation(position, Quaternion.Euler(0, 0, angle));
    }

    void DropPheromones()
    {
        //If haven't dropped pheromone yet
        if (previousPheromone == null)
        {
            GameObject newPheromone = Instantiate(pheromoneTypes[0], transform.position, transform.rotation);
            previousPheromone = newPheromone.transform;
        }
        //If pheromone is far enough away
        else if (Vector3.Distance(transform.position, previousPheromone.position) > pheromoneDistance)
        {
            if (foodCarrying != null)
            {
                GameObject newPheromone = Instantiate(pheromoneTypes[1], transform.position, transform.rotation);
                previousPheromone = newPheromone.transform;
            }
            else
            {
                GameObject newPheromone = Instantiate(pheromoneTypes[0], transform.position, transform.rotation);
                previousPheromone = newPheromone.transform;
            }
        }
    }   

    void CheckForFood()
    {
        //If haven't found food yet, look for food ahead
        if (targetFood == null)
        {
            Collider2D[] circle = Physics2D.OverlapCircleAll(transform.position, viewRadius);
            if (circle.Length > 0)
            {
                Transform randomTarget = circle[Random.Range(0, circle.Length)].transform;
                if (randomTarget.tag == "Food")
                {
                    Vector2 directionToFood = (randomTarget.position - transform.position).normalized;

                    if (Vector2.Angle(transform.right, directionToFood) < viewAngle / 2)
                    {
                        targetFood = randomTarget;
                        randomTarget.tag = "TakenFood";
                    }
                }
            }
        }

        //If found food, check if it's in range to be picked up
        else
        {
            Collider2D[] circle = Physics2D.OverlapCircleAll(transform.position, pickUpRadius);
            foreach(var obj in circle)
            {
                if (obj.gameObject.transform == targetFood)
                {
                    PickUpFood();
                    break;
                }
            }
        }
    }

    void PickUpFood()
    {
        //Add target food as child of object
        targetFood.transform.position = transform.position;
        targetFood.parent = transform;
        foodCarrying = targetFood;
        targetFood = null;
        //Remove food as target
        //Change bool to remember that ant is carrying food
    }

    void UpdateSensor()
    {
        //Check if wall ahead
        SensorResponse bestResponse = new SensorResponse(0, 0);
        bestSensor = 0;
        //Check sensors for strongest pheromone level
        for (int i = 0; i < sensors.Length; i++)
        {
            SensorResponse response = sensors[i].GetComponent<SensorHandler>().GetSenses();
            if (i == 0)
            {
                bestResponse = response;
            }
            else
            {
                if (response.wonderStrength > bestResponse.wonderStrength)
                {
                    bestResponse = response;
                    bestSensor = i;
                }
            }
        }
        if (bestResponse.wonderStrength > 0)
        {
            foodTrailFound = true;
        }
        else
        {
            foodTrailFound = false;
        }
    }
}
