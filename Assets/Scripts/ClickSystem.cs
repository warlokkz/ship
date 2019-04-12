using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using Ray = Unity.Physics.Ray;
using RaycastHit = Unity.Physics.RaycastHit;

namespace Ship.Project
{
	[UpdateAfter(typeof(BuildPhysicsWorld)), UpdateBefore(typeof(EndFramePhysicsSystem))]
	public class ClickSystem : JobComponentSystem
	{
		private const float k_MaxDistance = 100.0f;

		private EntityQuery m_ClickGroup;
		private BuildPhysicsWorld m_BuildPhysicsWorldSystem;

		public NativeArray<ClickData> ClickDatas;
		public struct ClickData
		{
			public float3 Move;
		}
		
		[BurstCompile]
		struct ClickJob : IJob
		{
			[ReadOnly] public CollisionWorld CollisionWorld;
			[ReadOnly] public int NumDynamicBodies;
			public NativeArray<ClickData> ClickDatas;
			public Ray Ray;
			public void Execute()
			{
				float fraction = 1.0f;
				RigidBody? hitBody = null;
				var rayCastInput = new RaycastInput {Ray = Ray, Filter = CollisionFilter.Default};
				if (CollisionWorld.CastRay(rayCastInput, out RaycastHit hit))
				{
					if (hit.RigidBodyIndex < NumDynamicBodies)
					{
						hitBody = CollisionWorld.Bodies[hit.RigidBodyIndex];
						fraction = hit.Fraction;
					}
				}

				if (hitBody != null)
				{
					float3 pointInWorld = Ray.Origin + Ray.Direction * fraction;
					ClickDatas[0] = new ClickData
					{
						Move = new float3 {
							x = pointInWorld.x,
							y = 0,
							z = pointInWorld.z
						}
					};
				}
				else
				{
					ClickDatas[0] = new ClickData();
				}
			}
		}

		public ClickSystem()
		{
			ClickDatas = new NativeArray<ClickData>(
				1,
				Allocator.Persistent,
				NativeArrayOptions.UninitializedMemory
			);
			ClickDatas[0] = new ClickData();
		}

		protected override void OnCreateManager()
		{
			m_BuildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
			m_ClickGroup = GetEntityQuery(new EntityQueryDesc
			{
				All = new ComponentType[] {typeof(Clickable)}
			});
		}

		protected override void OnDestroyManager()
		{
			ClickDatas.Dispose();
		}

		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			if (m_ClickGroup.CalculateLength() == 0)
			{
				return inputDeps;
			}
			
			var handle = JobHandle.CombineDependencies(inputDeps, m_BuildPhysicsWorldSystem.FinalJobHandle);

			if (Input.GetMouseButtonDown(1) && Camera.main != null)
			{
				Vector2 mousePosition = Input.mousePosition;
				UnityEngine.Ray unityRay = Camera.main.ScreenPointToRay(mousePosition);
				var ray = new Ray(unityRay.origin, unityRay.direction * k_MaxDistance);

				handle = new ClickJob
				{
					CollisionWorld = m_BuildPhysicsWorldSystem.PhysicsWorld.CollisionWorld,
					NumDynamicBodies = m_BuildPhysicsWorldSystem.PhysicsWorld.NumDynamicBodies,
					ClickDatas = ClickDatas,
					Ray = ray
				}.Schedule(
					JobHandle.CombineDependencies(
						handle,
						m_BuildPhysicsWorldSystem.FinalJobHandle
					)
				);

				handle.Complete();
			}

			return handle;
		}
	}
}