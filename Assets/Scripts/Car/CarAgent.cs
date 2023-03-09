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
            AddReward(-1f);
            Debug.Log("Reward Minused Wall Hit!!");
        }
    }
    private void OnCollisionStay(Collision collision) {
        if(collision.gameObject.TryGetComponent<Wall>(out Wall wall)) {
            AddReward(-0.5f);
            trackCheckpoints.DecreaseTimeHitWall(this);
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
            AddReward(1f);
            Debug.Log("Reward Added Move Forward!!");
        } else {
            AddReward(-2f);
            Debug.Log("Reward Minused Move Backward!!");
        }
        // if(Vector3.Distance(transform.position,lastPos)>5) {
        //     AddReward(0.1f);
        //     // Debug.Log("Reward Added Move Forward!!");
        // } else {
        //     AddReward(-0.1f);
        // }
        if(controlledCar.SpeedInHour>10||controlledCar.CurrentMaxSlip>2)
        {
            AddReward(0.1f);
        }
        lastPos = transform.position;
        sensor.AddObservation(controlledCar.CurrentMaxSlip);
        sensor.AddObservation(controlledCar.CarDirection);
        sensor.AddObservation(controlledCar.SpeedInHour);
        sensor.AddObservation(diff/20f);
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
        // int horizontal = actions.DiscreteActions[0];
        // int vertical = actions.DiscreteActions[1];
        // int breke = actions.DiscreteActions[2];
        // float Horizontal = 0f;
        // float Vertical = 0f;
        // bool Brake = false;
        // switch(horizontal){
        //     case 0: Horizontal = 0f; break;
        //     case 1: Horizontal = 1f; break;
        //     case 2: Horizontal = -1f; break;
        // }
        // switch(vertical){
        //     case 0: Vertical = 0f; break;
        //     case 1: Vertical = 1f; break;
        //     case 2: Vertical = -1f; break;
        // }
        // switch(breke){
        //     case 0: Brake = false; break;
        //     case 1: Brake = true; break;
        // }
        // controlledCar.UpdateControls(Horizontal,Vertical,Brake);

        float horizontal = actions.ContinuousActions[0]-actions.ContinuousActions[1];
        float vertical = actions.ContinuousActions[2]-actions.ContinuousActions[3];
        bool breke = false;
        if(actions.ContinuousActions[4]>0.2) {
            breke = true;
        }
        controlledCar.UpdateControls(horizontal,vertical,breke);

        // float horizontal = 0f;
        // float horizontalDecision = actions.ContinuousActions[0];
        // if(horizontalDecision>0.5) {
        //     horizontal = ((horizontalDecision-0.5f)/0.5f);
        // } else if(horizontalDecision==0.5) {
        //     horizontal = 0f;
        // } else {
        //     horizontal = ((horizontalDecision)/0.5f)*(-1);
        // }
        // float vertical = 0f;
        // float verticalDecision = actions.ContinuousActions[1];
        // if(verticalDecision>0.5) {
        //     vertical = ((verticalDecision-0.5f)/0.5f);
        // } else if(verticalDecision == 0.5) {
        //     vertical = 0f;
        // } else {
        //     vertical = ((verticalDecision)/0.5f)*(-1);
        // }
        // bool breke = false;
        // float brekeDecision = actions.ContinuousActions[2];
        // if(brekeDecision>0.5) {
        //     breke = true;
        // } else {
        //     breke = false;
        // }
        // controlledCar.UpdateControls(horizontal,vertical,breke);
    }
    public void SetTrackCheckpoint(TrackCheckpoints trackCheckpoints) {
        this.trackCheckpoints = trackCheckpoints;
    }
    public void CarCorrectCheckpoint() {
        AddReward(10.0f);
        Debug.Log("Reward Added Correct Checkpoint!!");
    }

    public void CarWrongCheckpoint() {
        AddReward(-10.0f);
        Debug.Log("Reward Minused Wrong Checkpoint!!");
    }

    public void Timeout() {
        AddReward(-5.0f);
        Debug.Log("Reward Minused Timeout!!");
    }
    public void CarEndEpisode() {
        EndEpisode();
    }
}
