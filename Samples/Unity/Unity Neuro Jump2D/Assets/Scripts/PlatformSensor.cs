using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSensor : MonoBehaviour {

    public Collider FirstCollider;
    public Collider SecondCollider;

    private bool _isFirstObjectToggle = true;

    void OnTriggerEnter(Collider other) {

        if (other.name.Contains("Player")) {
            return;
        }

        if (_isFirstObjectToggle) {
            _isFirstObjectToggle = false;
            FirstCollider = other;
        } else {
            SecondCollider = FirstCollider;
            FirstCollider = other;
        }
    }
}
