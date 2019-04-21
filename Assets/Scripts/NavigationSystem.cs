using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics.Systems;
using UnityEngine;

namespace Ship.Project {
    
	[UpdateBefore(typeof(BuildPhysicsWorld))]
    public class NavigationSystem : JobComponentSystem
    {
		private ClickSystem m_ClickSystem;
        private struct AgentData
        {
            public int Index;
            public Entity Entity;
        }

        [BurstCompile]
        private struct NavAgentMovementJob : IJobForEach<NavAgent>
        {
			public float3 ClickDestination;
            public float DeltaTime;
            public float3 Up;
            public float3 One;

            public void Execute(ref NavAgent agent)
            {
                if (agent.Status != AgentStatus.Moving)
                {
                    return;
                }

				agent.Destination = ClickDestination;

                if (agent.RemainingDistance > 0)
				{
					agent.CurrentMoveSpeed = Mathf.Lerp(
						agent.CurrentMoveSpeed,
						agent.MoveSpeed,
						DeltaTime * agent.Acceleration
					);

					var heading = (Vector3) (agent.Destination - agent.Position);
					agent.RemainingDistance = heading.magnitude;

					if (agent.RemainingDistance > 0.001f)
					{
						// tbd...
					}
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
				Up = Vector3.up,
				One = Vector3.one
			};
			
			return job.Schedule(this, inputDeps);
        }
    }
}
