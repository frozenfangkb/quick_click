using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class Target : MonoBehaviour
{

    public float 
        minForce = 12, 
        maxForce = 18, 
        throttleForce = 10,
        spawnPositionXRange = 4,
        spawnPositionY = -6;

    [Range(-100,100)]
    public int pointsValue;

    public ParticleSystem explosionParticle;
    private Rigidbody _rigidbody;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        transform.position = RandomSpawnPos();
        _rigidbody.AddForce(RandomForce(), ForceMode.Impulse);
        _rigidbody.AddTorque(
            RandomTorque(),
            RandomTorque(),
            RandomTorque(),
            ForceMode.Impulse);
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnMouseDown()
    {
        if (gameManager.gameState == GameManager.GameState.inGame)
        {
            gameManager.UpdateScore(pointsValue);
            /*if (gameObject.CompareTag("BadStuff"))
            {
                gameManager.GameOver();
            }*/
            Destroy(gameObject);
            Instantiate(explosionParticle, transform.position, explosionParticle.transform.rotation);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("KillZone"))
        {
            if (gameObject.CompareTag("GoodStuff"))
            {
                // gameManager.UpdateScore(-pointsValue);
                gameManager.GameOver();
            }
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Returns a random force for the AddForce method of a Rigidbody
    /// </summary>
    /// <returns>Random force multiplied by Vector3.up</returns>
    private Vector3 RandomForce()
    {
        return Random.Range(minForce, maxForce) * Vector3.up;
    }

    /// <summary>
    /// Returns a random torque amount
    /// </summary>
    /// <returns>random float between -throttleForce and throttleForce</returns>
    private float RandomTorque()
    {
        return Random.Range(-throttleForce, throttleForce);
    }

    /// <summary>
    /// Returns a random Vector3 for spawning a new GameObject
    /// </summary>
    /// <returns>Random Vector3</returns>
    private Vector3 RandomSpawnPos()
    {
        return new Vector3(Random.Range(-spawnPositionXRange, spawnPositionXRange), spawnPositionY);
    }
}
