using Accord.Math;
using Accord.Statistics.Distributions.Multivariate;
using Accord.Statistics.Distributions.Sampling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3 = UnityEngine.Vector3; // Note: this might be necessary often

/// <summary>
///   Main controller for the sample application scene.
/// </summary>
/// 
/// <remarks>
///   This sample application simply uses a multivariate normal distribution (created
///   using the Accord.NET Framework) to move a sphere in a scene in random ways. This
///   is just an example to demonstrate how the Accord.NET libraries can be organized
///   and linked inside a Unity 3D project.
/// </remarks>
/// 
public class MainScript : MonoBehaviour
{
    GameObject sphere; // The sphere in the scene
    MultivariateNormalDistribution normal; // Accord.NET's multivariate normal distribution

    int numberOfFramesPassed = 0; // The number of frames that passed since the game started
    Vector3 destination; // The current destination point for the moving sphere.

    // Use this for initialization
    void Start()
    {
        // Get a reference to the game sphere
        this.sphere = GameObject.Find("Sphere");

        // Create a new Multivariate Normal Distribution
        this.normal = new MultivariateNormalDistribution(
            mean: new double[] { 0, 0, 0 },
            covariance: new double[,]
            {
                { 1, 0, 0 },
                { 0, 1, 0 },
                { 0, 0, 1 },
            });
    }

    // Update is called once per frame
    void Update()
    {
        // Move the sphere to its current destination position
        sphere.transform.position += Time.smoothDeltaTime * destination;

        // Increase the frame counter
        numberOfFramesPassed++;

        // Change destination every 10 frames
        if (numberOfFramesPassed % 10 == 0)
        {
            // 10 frames have passed, change destination:
            float[] random = normal.Generate().ToSingle();
            destination = new Vector3(random[0], random[1], random[2]);
        }
    }
}
