using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCheckpoints : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake(){
        Transform checkpointsTranform =  transform.Find("CheckPoints");
        Debug.Log(checkpointsTranform);
        foreach (Transform checkpointSingleTranform in checkpointsTranform){
            Debug.Log(checkpointSingleTranform);
        }
    }
}
