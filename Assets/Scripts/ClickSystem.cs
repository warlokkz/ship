using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using Ray = Unity.Physics.Ray;
using RaycastHit = Unity.Physics.RaycastHit;

namespace Ship.Project
{
	[UpdateAfter(typeof(BuildPhysicsWorld)), UpdateBefore(typeof(EndFramePhysicsSystem))]
	public class ClickSystem : ComponentSystem
	{
		private const float k_MaxDistance = 100.0f;

		private BuildPhysicsWorld m_BuildPhysicsWorldSystem;

		public struct ClickData : IComponentData
		{
			public float3 ClickDestination;
		}
		
		protected override void OnCreateManager()
		{
			m_BuildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();

			// initialize a new entity for click-data and set a blank singleton.
			EntityManager.CreateEntity(typeof(ClickData));
			SetSingleton(new ClickData());
		}

		protected override void OnUpdate()
		{
			var collisionWorld = m_BuildPhysicsWorldSystem.PhysicsWorld.CollisionWorld;
			
			if (Input.GetMouseButtonDown(1) && Camera.main != null)
			{
				// Create a new Ray from the camera screen
				Vector2 mousePosition = Input.mousePosition;
				UnityEngine.Ray unityRay = Camera.main.ScreenPointToRay(mousePosition);
				var ray = new Ray(unityRay.origin, unityRay.direction * k_MaxDistance);

				float fraction = 1.0f;
				RigidBody? hitBody = null;

				// Now cast the ray and see if the it hits any colliders.
				var rayCastInput = new RaycastInput {Ray = ray, Filter = CollisionFilter.Default};
				if (collisionWorld.CastRay(rayCastInput, out RaycastHit hit))
				{
					hitBody = collisionWorld.Bodies[hit.RigidBodyIndex];
					fraction = hit.Fraction;
				}

				// If we have a hit, then we'll update the singleton data with
				// the x and z values. (we're not doing anything with y at the moment)
				if (hitBody != null)
				{
					float3 pointInWorld = ray.Origin + ray.Direction * fraction;
					SetSingleton(new ClickData
					{
						ClickDestination = new float3
						{
							x = pointInWorld.x,
							y = 0,
							z = pointInWorld.z
						}
					});
				}
			}
		}
	}
}