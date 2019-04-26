using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics.Systems;
using UnityEngine;

namespace Ship.Project {
    
	[UpdateAfter(typeof(BuildPhysicsWorld))]
    public class NavigationSystem : JobComponentSystem
    {
		private ClickSystem m_ClickSystem;

        [BurstCompile]
        private struct NavAgentMovementJob : IJobForEach<NavAgent>
        {
			public float3 ClickDestination;
            public float DeltaTime;
            public float3 Up;

            public void Execute(ref NavAgent agent)
            {
				// find the magnitude of the vector the agent is currently heading towards based on
				// the previous position.
				agent.Destination = ClickDestination;
				agent.Position = agent.NextPosition;
				Vector3 heading = agent.Destination - agent.Position;
				agent.RemainingDistance = heading.magnitude;

                if (agent.RemainingDistance > 0.001f)
				{
					agent.CurrentMoveSpeed = math.lerp(
						agent.CurrentMoveSpeed,
						agent.MoveSpeed,
						DeltaTime * agent.Acceleration
					);

					/**
					 * This block sets the rotation of the agent towards the destination if the agent
					 * has not reached it yet.
					 */
					Vector3 targetRotation = Quaternion.LookRotation(heading, Up).eulerAngles;
					targetRotation.x = 0;
					targetRotation.z = 0;

					if (agent.RemainingDistance < 1f)
					{
						// Rotates around the y-axis.
						agent.Rotation = Quaternion.Euler(targetRotation);
					} else
					{
						// Rotates the z-axis by a linearly interpolated z-value.
						agent.Rotation = Quaternion.Slerp(
							agent.Rotation,
							Quaternion.Euler(targetRotation),
							DeltaTime * agent.RotationSpeed
						);
					}

					// Now that the agent is facing in the direction of the destination, find the next forward
					// position based on current move speed and deltaTime.
					float3 forward = math.forward(agent.Rotation) * agent.CurrentMoveSpeed * DeltaTime;
					agent.NextPosition = agent.Position + forward;
				}
            }
        }
        
		protected override void OnCreate()
		{
			m_ClickSystem = World.GetOrCreateSystem<ClickSystem>();
		}
        
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
			ClickSystem.ClickData clickData = m_ClickSystem.ClickDatas[0];
			var job = new NavAgentMovementJob
			{
				DeltaTime = Time.deltaTime,
				ClickDestination = clickData.ClickDestination,
				Up = Vector3.up
			};
			
			return job.Schedule(this, inputDeps);
        }
    }
}
