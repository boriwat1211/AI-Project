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
    private float lastSpeed = 0;
    private void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.TryGetComponent<Wall>(out Wall wall)) {
            AddReward(-0.5f);
            Debug.Log("Reward Minused Wall Hit!!");
        }
        if(collision.gameObject.TryGetComponent<CarAgent>(out CarAgent car)) {
            AddReward(-0.05f);
            Debug.Log("Reward Minused Car Hit!!");
        }
    }
    private void OnCollisionStay(Collision collision) {
        if(collision.gameObject.TryGetComponent<Wall>(out Wall wall)) {
            AddReward(-0.1f);
            trackCheckpoints.DecreaseTimeHitWall(this);
            Debug.Log("Reward Minused Wall Stay!!");
        }
        if(collision.gameObject.TryGetComponent<CarAgent>(out CarAgent car)) {
            AddReward(-0.01f);
            trackCheckpoints.DecreaseTimeHitWall(this);
            Debug.Log("Reward Minused Car Stay!!");
        }
    }

    public override void Initialize()
    {
        controlledCar = GetComponent<CarController>();
    }
    public override void OnEpisodeBegin()
    {
        Vector3 pos = trackCheckpoints.GetNewSpawnPoint(this);
        transform.localPosition = pos;
        transform.forward = new Vector3(0,0,-10);
        trackCheckpoints.ResetCheckpoint(this);
        lastPos = transform.localPosition;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 diff = trackCheckpoints.GetNextCheckpoint(this)-transform.localPosition;
        // if(lastPos!=Vector3.zero && (Vector3.Distance(lastPos,trackCheckpoints.GetNextCheckpoint(this))>Vector3.Distance(transform.localPosition,trackCheckpoints.GetNextCheckpoint(this)))) {
        //     AddReward(0.01f);
        //     Debug.Log("Reward Added Move Forward!!");
        // }
        float directionDot = Vector3.Dot(transform.forward,trackCheckpoints.GetNextCheckpointForward(this));
        lastSpeed = controlledCar.SpeedInHour;
        lastPos = transform.localPosition;
        if(lastSpeed > 150) {
            Debug.Log("Reward Added Speed Over 150 km/hr. !!");
            // AddReward(2f);
        }
        else if (lastSpeed > 120) {
            Debug.Log("Reward Added Speed Over 120 km/hr. !!");
            // AddReward(1.75f);
        }
        else if (lastSpeed > 100) {
            Debug.Log("Reward Added Speed Over 100 km/hr. !!");
            // AddReward(1.50f);
        }
        else if(lastSpeed>80) {
            // Debug.Log("Reward Added Speed Over 80 km/hr. !!");
            // AddReward(1.25f);
        }
        else if(lastSpeed>50) {
            // Debug.Log("Reward Added Speed Over 50 km/hr. !!");
            // AddReward(1f);
        } else if(lastSpeed > 20) {
            // Debug.Log("Reward Added Speed Over 20 km/hr. !!");
            // AddReward(0.75f);
        } else if(lastSpeed > 5) {
            // Debug.Log("Reward Added Speed Over 5 km/hr. !!");
            // AddReward(0.5f);
        }
        else {
            // Debug.Log("Reward Minused Speed Under 5 km/hr. !!");
            // AddReward(-2f);
        }
        sensor.AddObservation(directionDot);
        sensor.AddObservation(controlledCar.CurrentMaxSlip);
        sensor.AddObservation(controlledCar.SpeedInHour/100);
        sensor.AddObservation(diff/20f);
        AddReward(-0.01f);
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
        float horizontal = actions.ContinuousActions[0];
        float vertical = actions.ContinuousActions[1];
        bool breke = false;
        if(actions.ContinuousActions[2]>0) {
            breke = true;
        }
        controlledCar.UpdateControls(horizontal,vertical,breke);
    }
    public void SetTrackCheckpoint(TrackCheckpoints trackCheckpoints) {
        this.trackCheckpoints = trackCheckpoints;
    }
    public void CarCorrectCheckpoint() {
        AddReward(1.0f);
        Debug.Log("Reward Added Correct Checkpoint!!");
    }

    public void CarWrongCheckpoint() {
        AddReward(-1.0f);
        Debug.Log("Reward Minused Wrong Checkpoint!!");
    }

    public void Timeout() {
        // AddReward(-20.0f);
        Debug.Log("Reward Minused Timeout!!");
    }
    public void CarEndEpisode() {
        EndEpisode();
    }   
    public void CarTimeLapFaster() {
        // AddReward(20.0f);
        Debug.Log("Reward Added TimeLap faster!!");
    }

    public void CarCompleteTrack(float Time) {

    }
}
