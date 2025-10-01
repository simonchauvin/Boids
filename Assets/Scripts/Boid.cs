using UnityEngine;
using System.Collections;
using System.Xml.Serialization;

public class Boid : MonoBehaviour
{
    #region Parameters
    public bool debug;
    [SerializeField] private float maxVelocity = 100;
    #endregion

    #region Components
    private Rigidbody thisRigidbody;
    private Transform neighborArea;
    private Transform separationArea;
    #endregion

    private FlockManager flockManager;

    private Vector3 gameAreaCenter;
    private float gameAreaRadius;


    private void Awake()
    {
        // Components
        thisRigidbody = GetComponent<Rigidbody>();
        neighborArea = transform.Find("NeighborArea");
        separationArea = transform.Find("SeparationArea");

        // Flock
        flockManager = GetComponentInParent<FlockManager>();
    }

    void Start ()
    {
        // Init
        transform.position = new Vector3(Random.value * 10f, Random.value * 10f, Random.value * 10f);
        //transform.position = new Vector3(Random.value * 10f, 0f, Random.value * 10f);
        thisRigidbody.linearVelocity = new Vector3(Random.value * 2 - 1, Random.value * 2 - 1, Random.value * 2 - 1);
        //thisRigidbody.velocity = new Vector3(Random.value * 2 - 1, 0f, Random.value * 2 - 1);

        neighborArea.gameObject.SetActive(false);
        separationArea.gameObject.SetActive(false);
	}

    public void Init(Vector3 areaCenter, float areaRadius)
    {
        gameAreaCenter = areaCenter;
        gameAreaRadius = areaRadius;
    }

	void Update ()
    {
        transform.rotation = Quaternion.LookRotation(thisRigidbody.linearVelocity);

        if (debug)
        {
            ShowDebug();
        }
        else
        {
            HideDebug();
        }
	}

    public void ShowDebug()
    {
        neighborArea.localScale = new Vector3(flockManager.neighborRadius * 2, flockManager.neighborRadius * 2, flockManager.neighborRadius * 2);
        separationArea.localScale = new Vector3(flockManager.desiredSeparation * 2, flockManager.desiredSeparation * 2, flockManager.desiredSeparation * 2);

        neighborArea.gameObject.SetActive(true);
        separationArea.gameObject.SetActive(true);
    }

    public void HideDebug()
    {
        neighborArea.gameObject.SetActive(false);
        separationArea.gameObject.SetActive(false);
    }

    void FixedUpdate()
    {
        // Cap velocity
        if (thisRigidbody.linearVelocity.sqrMagnitude > maxVelocity * maxVelocity)
            thisRigidbody.linearVelocity = thisRigidbody.linearVelocity.normalized * maxVelocity;

        boundPosition();
        //warpPosition();
    }

    private void boundPosition ()
    {
        if (Vector3.Distance(transform.position, gameAreaCenter) > gameAreaRadius)
        {
            //thisRigidbody.AddForce((Vector3.zero - transform.localPosition) * 0.01f);
            thisRigidbody.linearVelocity += (Vector3.zero - transform.localPosition) * 0.1f * Time.deltaTime;
        }
    }

    private void warpPosition ()
    {
        if (Vector3.Distance(transform.localPosition, Vector3.zero) > 40f)
        {
            transform.localPosition = -transform.localPosition + (transform.localPosition - Vector3.zero).normalized;
        }
    }

    public void AddForce(Vector3 force)
    {
        thisRigidbody.linearVelocity += force * Time.deltaTime;
    }

    public Vector3 GetVelocity()
    {
        return thisRigidbody.linearVelocity;
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
