using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCheckpoints : MonoBehaviour
{
    public event EventHandler OnPlayerCorrectCheckpoint;
    public event EventHandler OnPlayerWrongCheckpoint;
    
    [SerializeField] float Timeleft = 30f;
    private List<CarAgent> carTransformList;
    private List<CheckpointSingle> checkpointSingleList;
    private List<int> nextCheckpointSingleIndexList;
    private List<float> checkpointTimeLeft;
    private void Awake() {
        Transform checkpointsTransform = transform.Find("Checkpoints");
        checkpointSingleList = new List<CheckpointSingle>();
        foreach (Transform checkpointSingleTransform in checkpointsTransform) {
            CheckpointSingle checkpointSingle = checkpointSingleTransform.GetComponent<CheckpointSingle>();
            checkpointSingle.SetTrackCheckpoint(this);
            checkpointSingleList.Add(checkpointSingle);
        }
        Transform carTransform = transform.Find("Car");
        carTransformList = new List<CarAgent>();
        foreach (Transform car in carTransform) {
            CarAgent carAgent = car.GetComponent<CarAgent>();
            carAgent.SetTrackCheckpoint(this);
            carTransformList.Add(carAgent);
        }
        nextCheckpointSingleIndexList = new List<int>();
        foreach (CarAgent car in carTransformList) {
            nextCheckpointSingleIndexList.Add(0);
        }
        checkpointTimeLeft = new List<float>();
        foreach (CarAgent car in carTransformList) {
            checkpointTimeLeft.Add(Timeleft);
        }
    }

    private void Update()
    {
        for(int i = 0;i<checkpointTimeLeft.Count;i++)
        {
            checkpointTimeLeft[i] = checkpointTimeLeft[i]-Time.deltaTime;
            if(checkpointTimeLeft[i]<=0)
            {
                carTransformList[i].Timeout();
                carTransformList[i].CarEndEpisode();
            }
        }
    }
    public void PlayerThroughCheckpoint(CheckpointSingle checkpointsingle, Transform carTransform) {
        if(checkpointSingleList.IndexOf(checkpointsingle) == nextCheckpointSingleIndexList[carTransformList.IndexOf(carTransform.GetComponent<CarAgent>())]) {
            // Debug.Log("Correct Checkpoint!!");
            carTransformList[carTransformList.IndexOf(carTransform.GetComponent<CarAgent>())].CarCorrectCheckpoint();
            nextCheckpointSingleIndexList[carTransformList.IndexOf(carTransform.GetComponent<CarAgent>())] = (nextCheckpointSingleIndexList[carTransformList.IndexOf(carTransform.GetComponent<CarAgent>())]+1) %  checkpointSingleList.Count;
            checkpointTimeLeft[carTransformList.IndexOf(carTransform.GetComponent<CarAgent>())] = Timeleft;
            OnPlayerCorrectCheckpoint?.Invoke(this,EventArgs.Empty);
        } else {
            // Debug.Log("Wrong Checkpoint!!");
            carTransformList[carTransformList.IndexOf(carTransform.GetComponent<CarAgent>())].CarWrongCheckpoint();
            OnPlayerWrongCheckpoint?.Invoke(this,EventArgs.Empty);
        }
    }

    public void ResetCheckpoint(CarAgent car) {
        nextCheckpointSingleIndexList[carTransformList.IndexOf(car)] = 0;
        checkpointTimeLeft[carTransformList.IndexOf(car)] = Timeleft;
    }

    public Vector3 GetNextCheckpoint(CarAgent car) {
        return checkpointSingleList[nextCheckpointSingleIndexList[carTransformList.IndexOf(car)]].transform.position;
    }

    public Vector3 GetNewSpawnPoint() {
        Vector3 pos = Vector3.zero + new Vector3(UnityEngine.Random.Range(-5f,+5f),0, UnityEngine.Random.Range(-15f,-6f)); 
        return pos;
    }
}
