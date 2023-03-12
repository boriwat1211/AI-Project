using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCheckpoints : MonoBehaviour
{
    [SerializeField] float Timeleft = 5f;
    private List<CarAgent> carTransformList;
    private List<CheckpointSingle> checkpointSingleList;
    private List<int> nextCheckpointSingleIndexList;
    private List<float> checkpointTimeLeft;
    private List<float> timeLap;
    private List<float> timeLapCounter;
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
        checkpointTimeLeft = new List<float>();
        timeLap = new List<float>();
        timeLapCounter = new List<float>();
        foreach (CarAgent car in carTransformList) {
            nextCheckpointSingleIndexList.Add(0);
            checkpointTimeLeft.Add(Timeleft);
            timeLap.Add(0f);
            timeLapCounter.Add(0f);
        }
    }

    private void Update()
    {
        for(int i = 0;i<checkpointTimeLeft.Count;i++)
        {
            checkpointTimeLeft[i] = checkpointTimeLeft[i]-Time.deltaTime;
            timeLapCounter[i] = timeLapCounter[i]+Time.deltaTime;
            if(checkpointTimeLeft[i]<=0)
            {
                carTransformList[i].Timeout();
                carTransformList[i].CarEndEpisode();
            }
        }
    }
    public void DecreaseTimeHitWall(CarAgent car) {
        // checkpointTimeLeft[carTransformList.IndexOf(car)] = checkpointTimeLeft[carTransformList.IndexOf(car)]-(Time.deltaTime*10);
    }
    public void PlayerThroughCheckpoint(CheckpointSingle checkpointsingle, Transform carTransform) {
        if(checkpointSingleList.IndexOf(checkpointsingle) == nextCheckpointSingleIndexList[carTransformList.IndexOf(carTransform.GetComponent<CarAgent>())]) {
            // Debug.Log("Correct Checkpoint!!");
            carTransformList[carTransformList.IndexOf(carTransform.GetComponent<CarAgent>())].CarCorrectCheckpoint();
            nextCheckpointSingleIndexList[carTransformList.IndexOf(carTransform.GetComponent<CarAgent>())] = (nextCheckpointSingleIndexList[carTransformList.IndexOf(carTransform.GetComponent<CarAgent>())]+1) %  checkpointSingleList.Count;
            checkpointTimeLeft[carTransformList.IndexOf(carTransform.GetComponent<CarAgent>())] = Timeleft;
            Debug.Log(checkpointTimeLeft[carTransformList.IndexOf(carTransform.GetComponent<CarAgent>())]);
            if(nextCheckpointSingleIndexList[carTransformList.IndexOf(carTransform.GetComponent<CarAgent>())]==0) {
                // Debug.Log("Goal Checked!!");
                if(timeLap[carTransformList.IndexOf(carTransform.GetComponent<CarAgent>())]==0||timeLapCounter[carTransformList.IndexOf(carTransform.GetComponent<CarAgent>())]<timeLap[carTransformList.IndexOf(carTransform.GetComponent<CarAgent>())])
                {
                    timeLap[carTransformList.IndexOf(carTransform.GetComponent<CarAgent>())] = timeLapCounter[carTransformList.IndexOf(carTransform.GetComponent<CarAgent>())];
                    carTransformList[carTransformList.IndexOf(carTransform.GetComponent<CarAgent>())].CarTimeLapFaster();
                    carTransformList[carTransformList.IndexOf(carTransform.GetComponent<CarAgent>())].CarEndEpisode();
                }
                timeLapCounter[carTransformList.IndexOf(carTransform.GetComponent<CarAgent>())] = 0f;
            }
        } else {
            // Debug.Log("Wrong Checkpoint!!");
            carTransformList[carTransformList.IndexOf(carTransform.GetComponent<CarAgent>())].CarWrongCheckpoint();
        }
    }

    public void ResetCheckpoint(CarAgent car) {
        nextCheckpointSingleIndexList[carTransformList.IndexOf(car)] = 0;
        checkpointTimeLeft[carTransformList.IndexOf(car)] = Timeleft;
    }

    public Vector3 GetNextCheckpoint(CarAgent car) {
        return checkpointSingleList[nextCheckpointSingleIndexList[carTransformList.IndexOf(car)]].transform.localPosition;
    }

    public Vector3 GetNewSpawnPoint() {
        Vector3 pos = Vector3.zero + new Vector3(5,0,-10); 
        return pos;
    }
}
