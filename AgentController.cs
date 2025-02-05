using System.Collections;                // Required for IEnumerator and other collection types
using System.Collections.Generic;        // Required for List<T> and other generic collections
using UnityEngine;                      // Core Unity engine functionality
using Unity.MLAgents;                   // ML-Agents framework core functionality
using Unity.MLAgents.Actuators;         // ML-Agents actuators for agent actions
using Unity.MLAgents.Sensors;           // ML-Agents sensors for environmental observations
using System.Linq;                      // Required for LINQ operations
using UnityEngine.AI;                   // Required for NavMesh functionality


    public class AgentController : Agent
    {
        [SerializeField] private Transform target;                // Reference to the target Transform that the agent will interact with
        public override void OnEpisodeBegin() // This method is called at the start of each episode to reset the environment and agent state
        {
            transform.localPosition = new Vector3( 0f, 0.0196f, 0f); // Reset agent position to starting coordinates (center X, slightly elevated Y, center Z)
            
            int rand = Random.Range(0, 2); // Generate random number (0 or 1) to randomly place target on left or right side
            if(rand == 0) // If random number is 0, then the target will be on the left side
            {
                target.localPosition = new Vector3(-0.48f, 0f, 0f); // Place target on left side (-4 on X axis, slightly elevated Y, center Z)
            }
            if (rand == 1) // If random number is 1, then the target will be on the right side


            {
                target.localPosition = new Vector3(0.48f, 0f, 0f); // Place target on right side (4 on X axis, slightly elevated Y, center Z)
            }

        }
        public override void CollectObservations(VectorSensor sensor)    // This method is called by ML-Agents on every step to collect environmental observations

        {
            sensor.AddObservation(transform.localPosition);      // Add agent's current position (Vector3) to observations - this helps agent know where it is in the environment
            sensor.AddObservation(target.localPosition);         // Add target's current position (Vector3) to observations - this helps agent know where the target is relative to itself
                                                               // Total observation space size = 6 (3 floats for agent position + 3 floats for target position)
        }
        public override void OnActionReceived(ActionBuffers actions)    // Called by ML-Agents framework every time the neural network makes a decision. Receives action buffers containing the network's output
        {
            float move = actions.ContinuousActions[0];          // Extract the first continuous action value from neural network output (range typically -1 to 1). This represents the desired movement along X axis
            float moveSpeed = 2f;                              // Constant multiplier to scale the movement speed. Higher values make the agent move faster, lower values make it move slower
            
            transform.localPosition += new Vector3(move, 0f) * Time.deltaTime * moveSpeed;  // Update agent's position: Creates movement vector (move on X, 0 on Y), multiplies by deltaTime for frame-rate independence, and by moveSpeed for proper scaling
        }
        public override void Heuristic(in ActionBuffers actionsOut) // This method allows manual control during training by overriding the neural network's decisions with keyboard input
        {
            var continuousActionsOut = actionsOut.ContinuousActions;                // Get reference to continuous actions buffer where we'll write our manual control values
            float horizontalInput = Input.GetAxis("Horizontal");                    // Get horizontal input from keyboard left/right arrow keys or A/D keys (returns value between -1 and 1)
            continuousActionsOut[0] = horizontalInput;                             // Write the horizontal input value to the first continuous action slot for X-axis movement
        }
        private void OnTriggerEnter(Collider other) // This is for the trigger events
        {
            if (other.gameObject.CompareTag("Hiding Spot")) // If the agent collides with the hiding spot

            {
                SetReward(10f); // Give a reward
                EndEpisode(); // End the episode

            }
            else if (other.gameObject.CompareTag("Wall")) // If the agent collides with the wall   
            {
                SetReward(-5f); // Give a penalty
                EndEpisode(); // End the episode
            }

        }
    }