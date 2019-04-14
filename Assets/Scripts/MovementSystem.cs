using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
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
		struct MovementJob : IJobForEach<Translation, Rotation, PlayerMovementData>
		{
			public float DeltaTime;
			public float3 Move;

			public void Execute(
				ref Translation translation,
				ref Rotation rotation,
				[ReadOnly] ref PlayerMovementData movementData
			) {
				
				translation.Value += DeltaTime * movementData.MoveSpeed * math.forward(rotation.Value);

				var angle = math.atan2(
					Move.z - translation.Value.z,
					Move.x - translation.Value.x
				) * 180 / math.PI;

				rotation.Value = math.mul(
					math.normalize(rotation.Value),
					quaternion.AxisAngle(math.up(),
					(float)angle)
				);
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
			ComponentDataFromEntity<PhysicsVelocity> Velocities = GetComponentDataFromEntity<PhysicsVelocity>();

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
