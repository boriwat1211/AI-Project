using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSingle : MonoBehaviour
{
    private TrackCheckpoints trackCheckpoints;
    private void OnTriggerEnter(Collider other) {
        if(other.TryGetComponent<CarAgent>(out CarAgent carAgent)) {
            trackCheckpoints.PlayerThroughCheckpoint(this,carAgent.transform);
        }
    }

    public void SetTrackCheckpoint(TrackCheckpoints trackCheckpoints) {
        this.trackCheckpoints = trackCheckpoints;
    }
}
