using UnityEngine;
using System.Collections;

public class FlockManager : MonoBehaviour
{
    [SerializeField] private Transform flockFolder;
    public Boid boidPrefab;
    [SerializeField] private Transform area;
    public int numberOfBoids;
    public float alignmentWeight; // 0.5
    public float cohesionWeight; // 0.9
    public float separationWeight; // 0.1

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
            if (boid != null && boid.thisRigidbody != null)
            {
                int count = 0, separateCount = 0;
                Vector3 velocities = Vector3.zero;
                Vector3 centerOfMass = Vector3.zero;
                Vector3 velocity = Vector3.zero;

                for (int j = 0; j < numberOfBoids; j++)
                {
                    Vector3 toBoid = boids[j].transform.position - boid.transform.position;

                    float distance = toBoid.magnitude;
                    if (distance > 0 && distance < boid.neighborRadius)
                    {
                        velocities += boids[j].thisRigidbody.linearVelocity;
                        centerOfMass += boids[j].transform.position;

                        if (distance < boid.desiredSeparation)
                        {
                            Vector3 norm = toBoid / distance;
                            velocity -= norm / distance;
                            separateCount++;
                        }

                        count++;
                    }
                }

                Vector3 alignment = Vector3.zero;
                Vector3 cohesion = Vector3.zero;
                Vector3 separation = Vector3.zero;

                if (count > 0)
                {
                    alignment = (velocities / (numberOfBoids - 1)).normalized * alignmentWeight * Time.deltaTime;
                    cohesion = ((centerOfMass / (numberOfBoids - 1)) - boid.transform.position).normalized * cohesionWeight * Time.deltaTime;
                }

                if (separateCount > 0)
                {
                    separation = (velocity / (numberOfBoids - 1)).normalized * separationWeight * Time.deltaTime;
                }

                if (boid.debug)
                {
                    boid.showAlignmentDebug(alignment);
                    boid.showCohesionDebug(cohesion);
                    boid.showSeparationDebug(separation);
                }

                boid.thisRigidbody.linearVelocity += alignment + cohesion + separation;
            }
        }
	}
}
