## What we are trying to do
- I want to train an agent that moves to hiding spots avoiding observation from a seeker agent.
- Later i want to take this agent and use it in my other unity project.

## This is what we have done so far
### Python environment
- I have made the python environment named "ml-agents" using conda.
- The python version is 3.9.13.
- I have installed the following packages:
  - mlagents
  - protobuf
  - onnx
  - torch

### Unity
- I have installed the ml-agents 2.0.1 version in the unity project.

### This is the hierarchy of the unity project:
- Camera
- Directional Light
- Env
    - Floor(this has child object for making the floor)
    - Wall(this has child object for making the wall, all its child objects have the tag "Wall")
    - Cubie(this is the RL agent that will be trained)
    - Hiding Spot(this is the hiding spot for the cubie, the cubie will try to reach this spot, it has the tag "Hiding Spot")


### These are the components of the cubie object:
- Agent Controller.cs

- Box Collider:
  - Is Trigger: true // This is to enable trigger events
  - Provides Contacts: false // This disables the generation of contact points and normal vectors during collisions, which are used for detailed physics calculations and collision response. When false, it optimizes performance by skipping these calculations since we don't need this detailed collision data for our hiding agent
  - Material: None // This is to use the default friction and bounce settings
  - Center: (0,0,0) // This is to align the collider with the visual mesh
  - Size: (1,1,1) // This is to match the size of the visual mesh
  - Layer Overrides: 
    - Layer Override Priority: 0 // Controls which layers this collider interacts with. Lower numbers mean lower priority in collision detection
    - Include Layers: Nothing // Specifies which physics layers this collider WILL interact with. "Nothing" means no layers are specifically included
    - Exclude Layers: Nothing // Specifies which physics layers this collider will NOT interact with. "Nothing" means no layers are specifically excluded
  
- Rigidbody:
  - Mass: 1 // Defines the object's mass in kilograms. Affects how forces impact the object and its momentum in physics calculations
  - Drag: 0 // Linear drag coefficient. Zero means no air resistance, allowing free movement. Higher values make the object slow down faster when moving through space
  - Angular Drag: 0 // Rotational drag coefficient. Zero means no rotational resistance, allowing free rotation. Higher values make the object slow down faster when rotating
  - Automatic Center of Mass: true // When enabled, Unity automatically calculates the center of mass based on the object's geometry. This provides realistic physics behavior for irregularly shaped objects
  - Automatic Tensor: true // When enabled, Unity automatically calculates the inertia tensor (resistance to rotational acceleration) based on the object's mass distribution
  - Use Gravity: False // This is to disable the gravity
  - Is Kinematic: False // When false, the object is fully physics-driven. When true, the object ignores forces and can only be moved through transform changes

  - Interpolate: None // No interpolation between physics updates. Other options (Interpolate/Extrapolate) can smooth movement but may introduce latency
  - Collision Detection: Discrete // Uses discrete time steps for collision detection. Suitable for normal speed objects. Continuous detection is needed only for very fast objects




- Behavior Parameters(ML-Agents Behavior Parameters):
  - Behavior Name: Agent Controller // Unique identifier used by ML-Agents to match this agent's behavior with its corresponding neural network policy during training and inference
  - Vector Observation Space Size: 6 // Defines the length of the observation vector that contains the agent's perception of its environment (e.g., positions, distances, states). Each element represents a different aspect of the environment the agent can observe
  - Stacked Vectors: 1 // Number of consecutive observation vectors to combine. Higher values (like 3 or 4) allow the agent to detect changes over time, similar to how multiple video frames show motion. Value of 1 means only current observation is used
  - Actions:
    - Continuous Actions: 1 // Number of continuous (floating point) actions the agent can take simultaneously. Each action can have any value within a range, useful for smooth movement or fine control
    - Discrete Branches: 1 // Number of independent categorical decisions the agent can make. Each branch represents a separate choice (like "move direction" or "jump/don't jump") with a fixed number of options
    - Model: None // The trained neural network model (.onnx file) that defines the agent's behavior. Set to None during training, but must be assigned the trained model for inference/deployment
    - Inference Device: Default // Controls whether model runs on CPU or GPU. Default auto-selects based on hardware. Set to CPU/GPU explicitly if needed for performance optimization
    - Behavior Type: Default // Controls agent's learning mode:
        - Default: Switches between training/inference automatically
        - Heuristic: Uses manually programmed behavior instead of neural network
        - Inference: Only runs trained model, no learning
    - Team ID: 0 // Numerical team identifier for multi-agent scenarios. Agents with same Team ID can share information and coordinate. Set to 0 for single agent
    - Use Child Sensors: Enabled // When enabled, automatically incorporates observations from sensor components on child GameObjects. Important for hierarchical agent structures
    - Observable Attribute: Ignore // Controls how [Observable] tagged variables are handled:
        - Ignore: Observable attributes are not included in observations
        - Include: Observable attributes are automatically added to observation space

- Decision Requester:
  - Script: DecisionRequester // Component that controls how often the agent makes decisions using the neural network
  - Decision Period: 5 // Number of Academy steps between decisions. Value of 5 means agent makes a decision every 5 fixed updates. Lower values mean more frequent decisions but higher computational cost
  - Take actions between: True // When enabled, agent will repeat its last action between decision steps. This provides smoother movement instead of agent remaining idle between decisions
