using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class FoodAgent : Agent
{
    public event EventHandler OnAteFood;
    public event EventHandler OnEpisodeBeginEvent;

    [SerializeField] private FoodSpawner foodSpawner;
    [SerializeField] private FoodButton foodButton;

    private Rigidbody agentRigidbody;

    private void Awake() {
        agentRigidbody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin() {
        transform.localPosition = new Vector3(UnityEngine.Random.Range(-2.5f, +2.5f), UnityEngine.Random.Range(-2.0, +2.0f));

        OnEpisodeBeginEvent?.Invoke(this, EventArgs.Empty);
    }
    
    
    public override void CollectObsrvations(VectorSensor sensor) {
        sensor.AddObservation(foodButton.CanUseButton() ? 1 : 0);

        Vector3 dirToFoodButton = (foodButton.transform.localPosition - transform.localPosition).normalized;
        // move to the sides, move posterior + anterior
        sensor.AddObservation(dirToFoodButton.x);
        sensor.AddObservation(dirToFoodButton.z); 

        sensor.AddObservation(foodSpawner.HasFoodSpawned() ? 1 : 0);

        if (foodSpawner.HasFoodSpawned()) {
            // new action value variable
            Vector3 dirToFood = (foodSpawner.GetLastFoodTransform().localPosition - transform.localPosition).normalized
            sensor.AddObservation(dirToFood.x);
            sensor.AddObservation(dirToFood.z);
        } else
        {
            //reward not spawned
            sensor.AddObservation(0f); // x
            sensor.AddObservation(0f); // z
        }
    }

    public override void OnActionReceived(ActionBuffers actions){
        int moveX = actions.DiscreteActions[0]; // 0 = Don't move; 1 = Left ; 2 = Right
        int moveZ = actions.DiscreteActions[1]; // 0 = Don't move; 1 = Back; 2 = Forward

        Vector3 addForce = new Vector3(0,0,0);

        switch (moveX) {
            case 0: addForce.x = 0f; break;
            case 1: addForce.x = -1f; break;
            case 2: addForce.x = +1f; break;
        }
        switch (moveZ) {
            case 0: addForce.z = 0f; break;
            case 1: addForce.z = -1f; break;
            case 2: addForce.z = +1f; break;
        }

        float moveSpeed = 5f;
        agentRigidbody.velocity = addForce * moveSpeed + new Vector3(0,agentRigidbody.velocity.y, 0);

        bool isUseButtonDown = actions.DiscreteActions[2] == 1;
        if (isUseButtonDown) {
            // Use Action
            Collider[] colliderArray = Physics.OverlapBox(transform.position, Vector3.one * .5f);
            foreach (Collider collider in colliderArray) {
                if (collider.TryGetComponent<FoodButton>(out FoodButton foodButton)) {
                    if (foodButton.CanUseButton()) {
                        foodButton.UseButton();
                        AddReward(1f); // When the Button is used successfully
                    }
                }
            }
        }
        AddReward(-1f / MaxStep);
    }
    public override void Heuristic(in ActionBuffers actionsOut) {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        switch (Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"))) {
            case -1: discreteActions[0] = 1; break;
            case 0: discreteActions[0] = 0; break;
            case +1: discreteActions[0] = 2; break;

        }
        switch (Mathf.RoundToInt(Input.GetAxisRaw("Vertical"))) {
            case -1: discreteActions[1] = 1; break;
            case 0: discreteActions[1] = 0; break;
            case +1: discreteActions[1] = 2; break;
        }
        discreteActions[2] = Input.GetKey(KeyCode.E) ? 1 : 0; // Use Action
    }

    private void OnCollisionEnter(Collision collision){
        if (collision.gameObject.TryGetComponent<Food>(out Food food)) {
            AddReward(1f);
            Destroy(food.gameObject);
            OnAteFood?.Invoke(this, EventArgs.Empty);

            EndEpisode();
        }

        // if (collision.gameObject.TryGetComponent<Wall>(out Wall wall)) {
            // EndEpisode();
        //  }
    }

}
