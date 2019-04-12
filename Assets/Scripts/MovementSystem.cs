using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace Ship.Project
{
	[UpdateBefore(typeof(BuildPhysicsWorld))]
	public class MovementSystem : JobComponentSystem
	{
		private EntityQuery m_ClickGroup;
		private ClickSystem m_ClickSystem;
		
		[BurstCompile]
		struct MovementJob : IJobForEach<Translation, PlayerInputData>
		{
			public float DeltaTime;
			public float3 Move;

			public void Execute(ref Translation translation, [ReadOnly] ref PlayerInputData input)
			{
				translation.Value = new float3
				{
					x = translation.Value.x + Move.x * DeltaTime,
					y = translation.Value.y,
					z = translation.Value.z + Move.z * DeltaTime
				};
			}
		}

		protected override void OnCreate()
		{
			m_ClickSystem = World.GetOrCreateSystem<ClickSystem>();
			m_ClickGroup = GetEntityQuery(new EntityQueryDesc
			{
				All = new ComponentType[] { typeof(Clickable) }
			});
		}

		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			ComponentDataFromEntity<Translation> Positions = GetComponentDataFromEntity<Translation>(true);

			ClickSystem.ClickData clickData = m_ClickSystem.ClickDatas[0];
			
			var job = new MovementJob
			{
				DeltaTime = Time.deltaTime,
				Move = clickData.Move
			};

			return job.Schedule(this, inputDeps);
		}
	}
}
