using Unity.MLAgents.Actuators;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class movetogoal : Agent
{
    //moving the agents
    [SerializeField] private Transform targetTransform;

    // // //for better visualisation
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;

    //resetting everything back to original state to train again. resets the character position back into starting state of 0,0,0 
    //called as soon as the episode begins

    public override void OnEpisodeBegin() {
        transform.localPosition = Vector3.zero;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
        // passes 6 values altogether, 3 positions each (x, y, z)
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        //GRAB FLOAT OBSERVATION --> move x on index 0 
        //GRAB FLOAT OBSERVATION --> move z on index 1
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        float moveSpeed = 3f;

        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
    }

    //HEURISTIC TESTING --> drive the actions ourselves 
    //Helps modify the actions that is received by te OnActionReceived function 
    // since we are usuing continuousactions in onactionreceived, use actionsOut
    public override void Heuristic(in ActionBuffers actionsOut){
        //base.Heuristic(actionsOut); 
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }


    //activate trigger to reach goal as the reward also has a collider
    //call the function setreward which meets a specific amount --> have a single goal 
    // function add reward increments the current reward --> AI car driver, increment on every checkpoint you hit 
     //episode ends when the character either achieves the full reward or loses 
    
    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent<Goal>(out Goal goal)){

        //specific value chosen doesnt really matter
        //only matters if it is relative to other rewards --> hit a wall, give large penalty
            SetReward(+1f); //get a positive reward
            floorMeshRenderer.material = winMaterial;
            EndEpisode();
        }

        if (other.TryGetComponent<Wall>(out Wall wall)){
            SetReward(-1f); //get a negative reward
            floorMeshRenderer.material = loseMaterial;
            EndEpisode();
        }
    }
}


