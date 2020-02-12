using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {


    Rigidbody rBody;

    private float forwardForce = 1000;
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        ComputeForwardForce();
    }

    private void ComputeForwardForce()
    {
        rBody.AddForce(0, 0, -forwardForce * Time.deltaTime);
    }

    public Rigidbody GetRigidbody() {
        return rBody;
    }

    public void ResetObstacle() {
        this.transform.position = new Vector3((Random.value - 0.5f) * 6, 0.5f, 30f); //Max x position will be 4
        this.transform.localScale = new Vector3(Random.value*2 + 2f, 1, 1); //Max x scale is 2 ( max width will be 2)
        this.transform.rotation = Quaternion.identity;
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;
    }

    public Bounds GetBounds() {
        return this.GetComponent<Renderer>().bounds;
    }
}