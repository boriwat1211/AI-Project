using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
public class CarAgent : Agent
{
    private TrackCheckpoints trackCheckpoints;
    private CarController controlledCar;

    private Vector3 lastPos = Vector3.zero;

    private void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.TryGetComponent<Wall>(out Wall wall)) {
            AddReward(-0.5f);
            Debug.Log("Reward Minused Wall Hit!!");
        }
    }
    private void OnCollisionStay(Collision collision) {
        if(collision.gameObject.TryGetComponent<Wall>(out Wall wall)) {
            AddReward(-0.1f);
            Debug.Log("Reward Minused Wall Stay!!");
        }
    }
    public override void Initialize()
    {
        controlledCar = GetComponent<CarController>();
    }
    public override void OnEpisodeBegin()
    {
        Vector3 pos = trackCheckpoints.GetNewSpawnPoint();
        transform.position = pos;
        transform.forward = new Vector3(0,0,-10);
        trackCheckpoints.ResetCheckpoint(this);

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 diff = trackCheckpoints.GetNextCheckpoint(this)-transform.position;
        if(lastPos!=Vector3.zero && (Vector3.Distance(lastPos,trackCheckpoints.GetNextCheckpoint(this))>Vector3.Distance(transform.position,trackCheckpoints.GetNextCheckpoint(this)))) {
            AddReward(0.1f);
            // Debug.Log("Reward Added Move Forward!!");
        }
        lastPos = transform.position;
        sensor.AddObservation(diff/20f);
        AddReward(-0.001f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        float Horizontal = Input.GetAxis("Horizontal");
        float Vertical = Input.GetAxis("Vertical");
        bool Brake = Input.GetButton("Jump");
        controlledCar.UpdateControls(Horizontal,Vertical,Brake);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        int horizontal = actions.DiscreteActions[0];
        int vertical = actions.DiscreteActions[1];
        int breke = actions.DiscreteActions[2];
        float Horizontal = 0f;
        float Vertical = 0f;
        bool Brake = false;
        switch(horizontal){
            case 0: Horizontal = 0f; break;
            case 1: Horizontal = 1f; break;
            case 2: Horizontal = -1f; break;
        }
        switch(vertical){
            case 0: Vertical = 0f; break;
            case 1: Vertical = 1f; break;
            case 2: Vertical = -1f; break;
        }
        switch(breke){
            case 0: Brake = false; break;
            case 1: Brake = true; break;
        }
        controlledCar.UpdateControls(Horizontal,Vertical,Brake);
    }
    public void SetTrackCheckpoint(TrackCheckpoints trackCheckpoints) {
        this.trackCheckpoints = trackCheckpoints;
    }
    public void CarCorrectCheckpoint() {
        AddReward(1.0f);
        // Debug.Log("Reward Added Correct Checkpoint!!");
    }

    public void CarWrongCheckpoint() {
        AddReward(-1.0f);
        // Debug.Log("Reward Minused Wrong Checkpoint!!");
    }

    public void Timeout() {
        AddReward(-1.0f);
        // Debug.Log("Reward Minused Timeout!!");
    }
    public void CarEndEpisode() {
        EndEpisode();
    }
}
