using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSpawner : MonoBehaviour {

    private bool _wasSpawned = false;

    public void OnCollisionEnter(Collision collision) {
        if (!_wasSpawned) {
            GameObject newGround = Instantiate(GameObject.Find("PlatformCube"));
            newGround.transform.localScale = transform.localScale - Vector3.right * transform.localScale.x + Random.Range(8, 12F) * Vector3.right;
            newGround.transform.position = transform.position + (Vector3.right * transform.localScale.x) + Random.Range(1, 10) / 5 * Vector3.right + newGround.transform.localScale.x / 4 * Vector3.right;
            _wasSpawned = true;
        }
    }
}
