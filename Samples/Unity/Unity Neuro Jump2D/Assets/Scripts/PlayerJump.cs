using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerJump : MonoBehaviour {

    [SerializeField]
    private NeuralNetwork neuralNetwork;
    [SerializeField]
    private PlatformSensor platformSensor;

    [SerializeField]
    private Text controlText;
    [SerializeField]
    private Text samplesText;

    private const float TIME_UNTIL_NEXT_DATASET = 0.3f;

    private Rigidbody _playerRigidBody;

    private float _speed = 3f;
    private float _countedTime;

    private double _canJump;

    private double distanceInPercent;
    private bool _getSamples;
    private float _distanceToEndOfPlatform;

    void Start() {
        _playerRigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update() {
        // move the parent (Camera + player)
        this.transform.parent.position += Vector3.right * Time.deltaTime * _speed;
        _countedTime += Time.deltaTime;

        if (!neuralNetwork.IsTrainingFinished) {

            // check for jump
            if (Input.GetKeyDown(KeyCode.Space)) {
                Jump();
            } else {
                // adding a new dataset
                if (_getSamples) {
                    if ((_countedTime > TIME_UNTIL_NEXT_DATASET) && !neuralNetwork.IsTrainingFinished && (_distanceToEndOfPlatform > 0) && (_distanceToEndOfPlatform < 1)) {
                        _countedTime = 0;
                        neuralNetwork.Train(_distanceToEndOfPlatform, _canJump, 0);
                    }
                }
            }

            controlText.text = "Player is controlled by: PLAYER";
            samplesText.text = string.Format("Samples collected: {0}/{1}", neuralNetwork.GatheredDataSets, NeuralNetwork.NUMBER_OF_TRAINING_SETS);
        } else {
            if ((_distanceToEndOfPlatform > 0) && (_distanceToEndOfPlatform < 1)) {
                double[] result = neuralNetwork.Compute(new double[] { _distanceToEndOfPlatform, _canJump });
                if (result[0] > 0.5) {
                    Jump();
                }
            }
            controlText.text = "Player is controlled by: NEURAL NETWORK";
        }

        // check if 2 gameobjects were found
        if ((platformSensor.FirstCollider != null) && (platformSensor.SecondCollider != null)) {
            _getSamples = true;

            // the surface point of the other collider that is closer to the position of this collider
            Vector3 jumpPoint = platformSensor.SecondCollider.ClosestPointOnBounds(platformSensor.FirstCollider.transform.position);
            _distanceToEndOfPlatform = jumpPoint.x - transform.position.x;

            // draw debug ray for better visualisation
            Debug.DrawRay(transform.position, jumpPoint - transform.position, Color.red);
        }
    }

    // reset jump after landing
    void OnCollisionEnter(Collision collision) {
        _canJump = 1;
    }

    private void Jump() {
        if (_canJump == 1) {
            if (!neuralNetwork.IsTrainingFinished) {
                // train neural network to jump 
                neuralNetwork.Train(_distanceToEndOfPlatform, _canJump, 1);
                Debug.Log("jump at dist: " + _distanceToEndOfPlatform);
            }

            // jump
            _playerRigidBody.AddForce(Vector3.up * 450F);
            _canJump = 0;
        }
    }
}
