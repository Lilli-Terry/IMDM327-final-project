using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using UnityEngine;
using Unity.MLAgents.Sensors;
using TMPro;

public class Sting : Agent
{
    [Header("Bee Settings")]
    public float stingProbability = 0.5f; // 50% chance to sting the hand
    public AudioClip stingSound; // Sound to play when the bee stings
    public AudioSource audioSource; // Audio source to play the sting sound

    private bool handDetected = false;

    //public GameObject stringScreen;
    public override void Initialize()
    {
        // Ensure the AudioSource is attached
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
           // stringScreen.SetActive(false);
        }
    }

    public override void OnEpisodeBegin()
    {
        // Reset state at the start of each episode
        handDetected = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Ray Perception Sensor will handle observations automatically
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // If the hand is detected, decide whether to sting
        if (handDetected)
        {
            Debug.Log("Hand detected by ray sensor!");

            // Random chance to sting
            if (Random.value < stingProbability)
            {
                Debug.Log("Bee stung the hand!");

                // Play the sting sound
                if (stingSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(stingSound);
                }

                // Reward the agent for stinging
                AddReward(1.0f);

               // StartCoroutine(Screen());
                EndEpisode();
            }
            else
            {
                Debug.Log("Bee chose not to sting.");
                AddReward(-0.1f);
            }
        }
    }
    public IEnumerator Screen()
    {
       // stringScreen.SetActive(true);
        yield return new WaitForSeconds(0.5f);
       // stringScreen.SetActive(false);
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
    }

    public void RaySensorDetected(string detectedTag)
    {
        if (detectedTag == "Hand")
        {
            handDetected = true;
        }
    }
}
