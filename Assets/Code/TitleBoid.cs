using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering;


public class TitleBoid : MonoBehaviour
{
    private const float G = 500f;
    private const float EPSILON = 0.0001f;

    public static GameObject[] body;
    BodyProperty[] bp;
    public static int numberOfSphere = 80;
    public GameObject bees;

    public Vector3 boxCenter = new Vector3(0, 0, 0); // Center of the rectangular box
    public float boxWidth = 80f;   // Width of the box along the X-axis
    public float boxHeight = 45f;   // Height of the box along the Y-axis
    public float boxDepth = 12f;    // Depth of the box along the Z-axis

    [Range(0f, 20f)]
    public float limitVelocity = 11.5f;
    [Range(0f, 10f)]
    public float limitAccelo = 6f;
    [Range(0f, 5f)]
    public float separationForce = 1.2f;
    [Range(0f, 4f)]
    public float separationDistance = 1.2f;
    [Range(0f, 20f)]
    public float homeForce = 20f;
    [Range(4f, 20f)]
    public float cohesionForce = 4.6f;
    [Range(0f, 10f)]
    public float alignmentForce = 1f;

    private float time;

    struct BodyProperty
    {
        public float mass;
        public Vector3 velocity;
        public Vector3 acceleration;
    }

    void Start()
    {
        bp = new BodyProperty[numberOfSphere];
        body = new GameObject[numberOfSphere];

        // Calculate half dimensions for spawn range
        float halfBoxWidth = boxWidth / 2f;
        float halfBoxHeight = boxHeight / 2f;
        float halfBoxDepth = boxDepth / 2f;

        for (int i = 0; i < numberOfSphere; i++)
        {
            body[i] = GameObject.Instantiate(bees);

            // Generate random position within rectangular box bounds
            float xPos = Random.Range(boxCenter.x - halfBoxWidth, boxCenter.x + halfBoxWidth);
            float yPos = Random.Range(boxCenter.y - halfBoxHeight, boxCenter.y + halfBoxHeight);
            float zPos = Random.Range(boxCenter.z - halfBoxDepth, boxCenter.z + halfBoxDepth);
            body[i].transform.position = new Vector3(xPos, yPos, zPos);
            body[i].transform.localScale = new Vector3(.5f, .5f, .5f);

            // Set random initial velocity within limits
            bp[i].velocity = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * limitVelocity;
            bp[i].mass = 10;
        }
    }

    void Update()
    {
        time += Time.deltaTime;
        for (int i = 0; i < numberOfSphere; i++)
        {
            bp[i].acceleration = Vector3.zero;

            // Apply Cohesion and Alignment forces
            bp[i].acceleration += Cohesion(i);
            bp[i].acceleration += Alignment(i);

            // Loop through other boids to apply gravity and separation
            for (int j = i + 1; j < numberOfSphere; j++)
            {
                Vector3 distance = body[j].transform.position - body[i].transform.position;
                float m1 = bp[i].mass;
                float m2 = bp[j].mass;
                Vector3 gravity = CalculatePull(distance, m1, m2);

                if (distance.sqrMagnitude > separationDistance)
                {
                    bp[i].acceleration += gravity / m1;
                    bp[j].acceleration -= gravity / m2;
                }
                else
                {
                    bp[i].acceleration -= gravity / m1 * separationForce;
                    bp[j].acceleration += gravity / m2 * separationForce;
                }
            }

            // Limit acceleration
            if (bp[i].acceleration.sqrMagnitude > limitAccelo)
            {
                bp[i].acceleration = bp[i].acceleration.normalized * limitAccelo;
            }

            // Add home force to bring boids towards the center of the box if they drift too far
            Vector3 originVector = (boxCenter - body[i].transform.position).normalized * homeForce;
            bp[i].acceleration += originVector;

            // Update velocity
            bp[i].velocity += bp[i].acceleration * Time.deltaTime;

            // Limit velocity
            if (bp[i].velocity.sqrMagnitude > limitVelocity)
            {
                bp[i].velocity = bp[i].velocity.normalized * limitVelocity;
            }

            // Check for NaN values in velocity and acceleration
            if (IsValidVector(bp[i].velocity))
            {
                body[i].transform.position += bp[i].velocity * Time.deltaTime;

                // Make the object face the direction of its velocity
                if (bp[i].velocity.sqrMagnitude > EPSILON) // Ensure velocity isn't close to zero
                {
                    body[i].transform.rotation = Quaternion.LookRotation(bp[i].velocity);
                }
            }

            // Keep boids within the box bounds
            ConstrainToBox(i);
        }
    }

    private Vector3 CalculatePull(Vector3 distanceVector, float m1, float m2)
    {
        // Avoid division by zero by adding EPSILON
        float distanceSqr = distanceVector.sqrMagnitude + EPSILON;
        Vector3 gravity = G * m1 * m2 / distanceSqr * distanceVector.normalized;
        return gravity;
    }

    private Vector3 Cohesion(int currentBoidIndex)
    {
        Vector3 centerOfMass = Vector3.zero;
        int neighborCount = 0;

        for (int i = 0; i < numberOfSphere; i++)
        {
            if (i != currentBoidIndex)
            {
                Vector3 distance = body[i].transform.position - body[currentBoidIndex].transform.position;
                if (distance.sqrMagnitude < separationDistance)
                {
                    centerOfMass += body[i].transform.position;
                    neighborCount++;
                }
            }
        }

        if (neighborCount == 0) return Vector3.zero;

        centerOfMass /= neighborCount;
        return (centerOfMass - body[currentBoidIndex].transform.position).normalized * cohesionForce;
    }

    private Vector3 Alignment(int currentBoidIndex)
    {
        Vector3 avgVelocity = Vector3.zero;
        int neighborCount = 0;

        for (int i = 0; i < numberOfSphere; i++)
        {
            if (i != currentBoidIndex)
            {
                Vector3 distance = body[i].transform.position - body[currentBoidIndex].transform.position;
                if (distance.sqrMagnitude < separationDistance)
                {
                    avgVelocity += bp[i].velocity;
                    neighborCount++;
                }
            }
        }

        if (neighborCount == 0) return Vector3.zero;

        avgVelocity /= neighborCount;
        return (avgVelocity - bp[currentBoidIndex].velocity).normalized * alignmentForce;
    }

    private void ConstrainToBox(int boidIndex)
    {
        float halfBoxWidth = boxWidth / 2f;
        float halfBoxHeight = boxHeight / 2f;
        float halfBoxDepth = boxDepth / 2f;

        Vector3 minBound = boxCenter - new Vector3(halfBoxWidth, halfBoxHeight, halfBoxDepth);
        Vector3 maxBound = boxCenter + new Vector3(halfBoxWidth, halfBoxHeight, halfBoxDepth);

        Vector3 newPosition = body[boidIndex].transform.position;

        if (newPosition.x < minBound.x) newPosition.x = minBound.x;
        else if (newPosition.x > maxBound.x) newPosition.x = maxBound.x;

        if (newPosition.y < minBound.y) newPosition.y = minBound.y;
        else if (newPosition.y > maxBound.y) newPosition.y = maxBound.y;

        if (newPosition.z < minBound.z) newPosition.z = minBound.z;
        else if (newPosition.z > maxBound.z) newPosition.z = maxBound.z;

        body[boidIndex].transform.position = newPosition;
    }

    private bool IsValidVector(Vector3 vector)
    {
        return !(float.IsNaN(vector.x) || float.IsNaN(vector.y) || float.IsNaN(vector.z));
    }
}
