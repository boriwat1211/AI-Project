using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCheckpoints : MonoBehaviour
{
    private List<SingleCheckpoint> checkpointSingleList;
    private int nextCheckpointSingleIndex;
    private void Awake() 
    {
        Transform checkpointsTransform = transform.Find("Checkpoints");
        checkpointSingleList = new List<SingleCheckpoint>();
        foreach (Transform checkpointSingleTranform in checkpointsTransform) 
        {
            SingleCheckpoint checkSingleCheckpoint = checkpointSingleTranform.GetComponent<SingleCheckpoint>();
            checkSingleCheckpoint.SetTrackCheckpoints(this);
            checkpointSingleList.Add(checkSingleCheckpoint);
        }

        nextCheckpointSingleIndex = 0;
    }

    public void PlayerThroughCheckpoint(SingleCheckpoint singleCheckpoint) 
    {
        Debug.Log(checkpointSingleList.IndexOf(singleCheckpoint));
        if(checkpointSingleList.IndexOf(singleCheckpoint)==nextCheckpointSingleIndex) 
        {
            nextCheckpointSingleIndex = (nextCheckpointSingleIndex+1)% checkpointSingleList.Count;
            Debug.Log("Coorrect");
        }
        else
        {
            Debug.Log("Wrong");
        }
    }
}
