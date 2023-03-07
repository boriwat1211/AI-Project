using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class CarAgent : Agent
{
    CarController ControlledCar;
    private float Horizontal;
    private float Vertical;
    private bool Brake;
    private void Awake()
    {
        ControlledCar = GetComponent<CarController> ();
    }
    public override void CollectObservations(VectorSensor sensor)
    {  
        // Debug.Log(ControlledCar.SpeedInHour.ToInt());
        sensor.AddObservation(ControlledCar.SpeedInHour.ToInt());
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        Debug.Log(actions.DiscreteActions[0]);
        int horizontal = actions.DiscreteActions[0];
        int vertical = actions.DiscreteActions[1];
        int breke = actions.DiscreteActions[2];
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
        ControlledCar.UpdateControls(Horizontal,Vertical,Brake);
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        Horizontal = Input.GetAxis ("Horizontal");
		Vertical = Input.GetAxis ("Vertical");
		Brake = Input.GetButton ("Jump");
        Debug.Log(Horizontal);
        Debug.Log(Vertical);
        Debug.Log(Brake);
        ControlledCar.UpdateControls (Horizontal, Vertical, Brake);
    }
    private void Update()
    {
        if(ControlledCar.SpeedInHour.ToInt()>50){
            AddReward(0.5f);
        }
        if(ControlledCar.SpeedInHour.ToInt()==0){
            AddReward(-1f);
        }
    }
}
