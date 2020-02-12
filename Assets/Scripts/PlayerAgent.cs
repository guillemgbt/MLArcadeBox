using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class PlayerAgent : Agent {


    public Obstacle obstacle;

    Rigidbody rBody;

    private bool didCollieWithObstacle = false;
    private float lateralSpeed = 50f;
    private float previousXDist = float.MinValue;
    private float previousXDistToIdeal = float.MaxValue;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public override void AgentReset()
    {
        if (IsPlayerFalling() || didCollieWithObstacle) {
            ResetPlayerPosition();
        }

        didCollieWithObstacle = false;
        previousXDist = float.MinValue;
        previousXDistToIdeal = float.MaxValue;
        ResetObstacle();
    }


    public override void CollectObservations()
    {

        Vector3 dist = obstacle.transform.position - this.transform.position;

        //TODO: Closest point?
        float xDist = dist.x/10; 
        float zDist = dist.z/32;
        
        float distToIdeal = Mathf.Abs(GetIdealXPosition()-this.transform.position.x)/8f;

        float leftMarginDist = (-5f-this.transform.position.x)/10f;
        float rightMarginDist = (5f - this.transform.position.x)/10f;

        float obstacleWidth = obstacle.transform.localScale.x/4f;

        /*
        Debug.Log(string.Format("xDist: {0}", xDist));
        Debug.Log(string.Format("zDist: {0}", zDist));
        Debug.Log(string.Format("velocity: {0}", velocity));
        Debug.Log(string.Format("leftMarginDist: {0}", leftMarginDist));
        Debug.Log(string.Format("rightMarginDist: {0}", rightMarginDist));
        Debug.Log(string.Format("obstacleWidth: {0}", obstacleWidth));
        */

        AddVectorObs(xDist);
        AddVectorObs(zDist);
        AddVectorObs(distToIdeal);
        AddVectorObs(leftMarginDist);
        AddVectorObs(rightMarginDist);
        AddVectorObs(obstacleWidth);

    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {

        //REWARDS

        if (IsPlayerFalling() || didCollieWithObstacle ) {
            //did fall or collide
            AddReward(-1.0f); //Give big negative reward
            Done();
        }

        if ((obstacle.transform.position.z - this.transform.position.z) < -1.5f) {
            //Has avoided obstcle
            AddReward(1.0f); //Give big reward
            Done();
        }


        //Distance to obstacle
        float xDist = Mathf.Abs(obstacle.transform.position.x - this.transform.position.x);

        if (xDist > previousXDist) {
            AddReward(0.05f);
        } else {
            AddReward(-0.05f);
        }

        previousXDist = xDist;



        //Distance to ideal X position
        float xDistIdeal = Mathf.Abs(GetIdealXPosition() - this.transform.position.x);

        if ((xDistIdeal < previousXDistToIdeal) || (xDistIdeal < 0.5f)) {
            AddReward(0.1f);
        } else {
            AddReward(-0.1f);
        }

        previousXDistToIdeal = xDistIdeal;


        //Actions
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0] * lateralSpeed;
        rBody.AddForce(controlSignal);

    }

    void OnCollisionEnter(Collision collision)
    {

        if (collision.collider.name == "Obstacle"){
            didCollieWithObstacle = true;
        }
    }

    private float GetIdealXPosition() {

        Bounds obstacleBounds = obstacle.GetBounds();

        if (obstacleBounds.center.x < 0) {
            //Get right pass
            float rightXPosition = obstacleBounds.center.x + (obstacleBounds.size.x / 2f);
            return ((rightXPosition + 5f)/2f);

        } else {
            //Get left pass
            float leftXPosition = obstacleBounds.center.x - (obstacleBounds.size.x / 2f);
            return ((leftXPosition - 5f) / 2f);
        }

    }

    private void ResetPlayerPosition()
    {
        this.transform.position = new Vector3(0f, 0.5f, 0f);
        this.transform.rotation = Quaternion.identity;
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;
    }

    private bool IsPlayerFalling() {
        return this.transform.position.y < -1.0;
    }

    private void ResetObstacle() {
        obstacle.ResetObstacle();
    }
}
