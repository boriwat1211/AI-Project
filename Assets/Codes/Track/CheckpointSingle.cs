using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSingle : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        Debug.Log(other);
        if(other.TryGetComponent<Car>(out Car car)) {
            Debug.Log("TEST");
        }
    }
}
