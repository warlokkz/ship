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
		private BuildPhysicsWorld m_BuildPhysicsWorldSystem;
		
		[BurstCompile]
		struct ClickJob : IJob
		{
			[ReadOnly] public CollisionWorld CollisionWorld;
			[ReadOnly] public int NumDynamicBodies;
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
				}
			}
		}

		protected override void OnCreateManager()
		{
			m_BuildPhysicsWorldSystem = World.GetOrCreateManager<BuildPhysicsWorld>();
		}

		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			var handle = JobHandle.CombineDependencies(inputDeps, m_BuildPhysicsWorldSystem.FinalJobHandle);

			if (Input.GetMouseButtonDown(1) && Camera.main != null)
			{
				Vector2 mousePosition = Input.mousePosition;
				UnityEngine.Ray unityRay = Camera.main.ScreenPointToRay(mousePosition);
				var ray = new Ray(unityRay.origin, unityRay.direction * 100);

				handle = new ClickJob
				{
					CollisionWorld = m_BuildPhysicsWorldSystem.PhysicsWorld.CollisionWorld,
					NumDynamicBodies = m_BuildPhysicsWorldSystem.PhysicsWorld.NumDynamicBodies,
					Ray = ray
				}.Schedule(JobHandle.CombineDependencies(handle, m_BuildPhysicsWorldSystem.FinalJobHandle));

				handle.Complete();
			}

			return handle;
		}
	}
}