using UnityEngine;
using System.Collections;

public class FlockManager : MonoBehaviour
{
    [SerializeField] private bool debug;
    [SerializeField] private Transform flockFolder;
    [SerializeField] private Boid boidPrefab;
    [SerializeField] private Transform area;
    public float neighborRadius;
    public float desiredSeparation;
    [SerializeField] private int numberOfBoids;
    [SerializeField] private bool physics;
    [SerializeField] private float alignmentWeight;
    [SerializeField] private float cohesionWeight;
    [SerializeField] private float separationWeight;

    private Boid[] boids;


	void Start ()
    {
        boids = new Boid[numberOfBoids];

        for (int i = 0; i < numberOfBoids; i++)
        {
            boids[i] = Instantiate(boidPrefab, transform.position, Quaternion.identity, flockFolder);
            boids[i].Init(area.position, area.localScale.x * 0.5f);
            boids[i].transform.parent = transform;
        }
	}

	void FixedUpdate ()
    {
        for (int i = 0; i < numberOfBoids; i++)
        {
            Boid boid = boids[i];
            boid.debug = debug;
            if (boid != null)
            {
                int count = 0, separateCount = 0;
                Vector3 velocity = Vector3.zero;
                Vector3 centerOfMass = Vector3.zero;
                Vector3 distance = Vector3.zero;

                for (int j = 0; j < numberOfBoids; j++)
                {
                    Vector3 toBoid = boid.transform.position - boids[j].transform.position;

                    float distToNeighbor = toBoid.magnitude;
                    if (distToNeighbor > 0 && distToNeighbor < neighborRadius)
                    {
                        velocity += boids[j].GetVelocity();
                        centerOfMass += boids[j].transform.position;

                        if (distToNeighbor < desiredSeparation)
                        {
                            Vector3 norm = toBoid / distToNeighbor;
                            distance += norm / distToNeighbor;
                            separateCount++;
                        }

                        count++;
                    }
                }

                Vector3 alignment = Vector3.zero;
                Vector3 cohesion = Vector3.zero;
                if (count > 0)
                {
                    alignment = (velocity / count).normalized * alignmentWeight;
                    cohesion = ((centerOfMass / count) - boid.transform.position).normalized * cohesionWeight;
                }

                Vector3 separation = Vector3.zero;
                if (separateCount > 0)
                {
                    separation = (distance / separateCount).normalized * separationWeight;
                }

                if (debug)
                {
                    boid.showAlignmentDebug(alignment);
                    boid.showCohesionDebug(cohesion);
                    boid.showSeparationDebug(separation);
                }

                boid.AddForce(alignment + cohesion + separation);
            }
        }
	}
}
