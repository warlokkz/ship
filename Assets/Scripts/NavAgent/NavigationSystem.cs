using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using ClickData = Ship.Project.ClickSystem.ClickData;

namespace Ship.Project {
    
	[UpdateAfter(typeof(ClickSystem))]
    public class NavigationSystem : JobComponentSystem
	{

		private Entity _clickDataEntity;
		private EntityManager _entityManager;

        [BurstCompile]
        private struct NavAgentMovementJob : IJobForEach<NavAgent>
        {
            public float DeltaTime;
			public ClickData m_ClickData;

            public void Execute(ref NavAgent agent)
            {
				// find the magnitude of the vector the agent is currently heading towards based on
				// the previous position.
				if (m_ClickData.Active) {
					agent.Destination = m_ClickData.ClickDestination;
				}
				
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
					Vector3 targetRotation = Quaternion.LookRotation(
						heading,
						Vector3.up
					).eulerAngles;
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
					float3 nextPosition = agent.Position + forward;
					Vector3 nextHeading = agent.Destination - nextPosition;

					agent.NextPosition = nextHeading.magnitude > 0.1f ? nextPosition : agent.Destination;
				}
            }
        }

		protected override void OnCreateManager()
		{
			_entityManager = World.EntityManager;
			_clickDataEntity = GetSingletonEntity<ClickData>();
		}

		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			ClickData clickData = _entityManager.GetComponentData<ClickData>(_clickDataEntity);
			
			var job = new NavAgentMovementJob
			{
				m_ClickData = clickData,
				DeltaTime = Time.deltaTime
			};
			return job.Schedule(this, inputDeps);
        }
    }
}
