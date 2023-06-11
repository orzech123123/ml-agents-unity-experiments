using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToGoalAgent : Agent
{
    public Transform TargetTransform;
    public float MoveSpeed = 1f;
    public Material WinMaterial;
    public Material LoseMaterial;
    public MeshRenderer FloorMeshRenderer;

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * MoveSpeed;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(TargetTransform);
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = Vector3.zero;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<Goal>(out var goal))
        {
            FloorMeshRenderer.material = WinMaterial;
            SetReward(1f);
            EndEpisode();
        }
        if (other.TryGetComponent<Wall>(out var wall))
        {
            FloorMeshRenderer.material = LoseMaterial;
            SetReward(-1f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }
}
