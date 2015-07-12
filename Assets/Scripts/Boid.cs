using UnityEngine;
using System.Collections;

public class Boid : MonoBehaviour
{
    #region Parameters
    public float neighborRadius;
    public float desiredSeparation;
    public bool debug;
    #endregion

    #region Components
    public Rigidbody thisRigidbody { get; private set; }
    private Transform neighborArea;
    private Transform separationArea;
    #endregion

    private FlockManager flockManager;

	
	void Start ()
    {
        // Components
        thisRigidbody = GetComponent<Rigidbody>();
        neighborArea = transform.FindChild("NeighborArea");
        separationArea = transform.FindChild("SeparationArea");

        // Flock
        flockManager = GetComponentInParent<FlockManager>();

        // Init
        transform.position = new Vector3(Random.value * 10f, Random.value * 10f, Random.value * 10f);
        //transform.position = new Vector3(Random.value * 10f, 0f, Random.value * 10f);
        thisRigidbody.velocity = new Vector3(Random.value * 2 - 1, Random.value * 2 - 1, Random.value * 2 - 1);
        //thisRigidbody.velocity = new Vector3(Random.value * 2 - 1, 0f, Random.value * 2 - 1);

        neighborArea.localScale = new Vector3(neighborRadius * 2, neighborRadius * 2, neighborRadius * 2);
        separationArea.localScale = new Vector3(desiredSeparation * 2, desiredSeparation * 2, desiredSeparation * 2);
        neighborArea.gameObject.SetActive(false);
        separationArea.gameObject.SetActive(false);
	}
	
	
	void Update ()
    {
        transform.rotation = Quaternion.LookRotation(thisRigidbody.velocity);

        if (debug)
        {
            neighborArea.gameObject.SetActive(true);
            separationArea.gameObject.SetActive(true);
        }
        else
        {
            neighborArea.gameObject.SetActive(false);
            separationArea.gameObject.SetActive(false);
        }
	}

    void FixedUpdate()
    {
        /*foreach (Collider collider in Physics.OverlapSphere(transform.localPosition, distance, LayerMask.GetMask("Boid")))
        {

        }*/

        boundPosition();
        //warpPosition();
    }

    private void boundPosition ()
    {
        if (Vector3.Distance(transform.localPosition, Vector3.zero) > 40f)
        {
            //thisRigidbody.AddForce((Vector3.zero - transform.localPosition) * 0.01f);
            thisRigidbody.velocity += (Vector3.zero - transform.localPosition) * 0.1f * Time.deltaTime;
        }
    }

    private void warpPosition ()
    {
        if (Vector3.Distance(transform.localPosition, Vector3.zero) > 40f)
        {
            transform.localPosition = -transform.localPosition + (transform.localPosition - Vector3.zero).normalized;
        }
    }

    public void showAlignmentDebug(Vector3 velocity)
    {
        Debug.DrawRay(transform.localPosition, velocity, Color.blue);
    }

    public void showCohesionDebug(Vector3 velocity)
    {
        Debug.DrawRay(transform.localPosition, velocity, Color.green);
    }

    public void showSeparationDebug(Vector3 velocity)
    {
        Debug.DrawRay(transform.localPosition, velocity, Color.red);
    }
}
