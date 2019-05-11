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
	public class PlayerInputSystem : ComponentSystem
	{
		private const float k_MaxDistance = 100.0f;

		private BuildPhysicsWorld m_BuildPhysicsWorldSystem;

		public struct ClickData : IComponentData
		{
			public bool Active;
			public float3 ClickDestination;
		}

		public struct FireInputData : IComponentData
		{
			public float3 FireDirection;
		}
		
		protected override void OnCreateManager()
		{
			m_BuildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();

			// initialize a new entity for click-data and set a blank singleton.
			EntityManager.CreateEntity(typeof(ClickData));
			SetSingleton(new ClickData());
			
			// do the same for FireInputData
			EntityManager.CreateEntity(typeof(FireInputData));
			SetSingleton(new FireInputData
			{
				FireDirection = new float3
				{
					x = float.PositiveInfinity,
					y = float.PositiveInfinity,
					z = float.PositiveInfinity
				}
			});
		}

		protected override void OnUpdate()
		{
			// Movement Updates
			if (Input.GetMouseButtonDown(button: 1) && Camera.main != null)
			{
				if (GetPointInWorldFromMousePosition(out float3 pointInWorld)) {
					SetSingleton(new ClickData
					{
						Active = true,
						ClickDestination = new float3
						{
							x = pointInWorld.x,
							y = 0,
							z = pointInWorld.z
						}
					});
				}
			} else {
				SetSingleton(new ClickData());
			}
			
			// Fire Updates?
			if (Input.GetMouseButtonDown(button: 0) && Camera.main != null)
			{
				if (GetPointInWorldFromMousePosition(out float3 pointInWorld))
				{
					SetSingleton(new FireInputData
					{
						FireDirection = new float3
						{
							x = pointInWorld.x,
							y = 0,
							z = pointInWorld.z
						}
					});
				}
			} else
			{
				SetSingleton(new FireInputData
				{
					FireDirection = new float3
					{
						x = float.PositiveInfinity,
						y = float.PositiveInfinity,
						z = float.PositiveInfinity
					}
				});
			}
		}

		private bool GetPointInWorldFromMousePosition(out float3 pointInWorld)
		{
			CollisionWorld collisionWorld = m_BuildPhysicsWorldSystem.PhysicsWorld.CollisionWorld;
			// Create a new Ray from the camera screen
			Vector2 mousePosition = Input.mousePosition;
			UnityEngine.Ray unityRay = Camera.main.ScreenPointToRay(mousePosition);
			var ray = new Ray(unityRay.origin, unityRay.direction * k_MaxDistance);

			var fraction = 1.0f;
			RigidBody? hitBody = null;

			// Now cast the ray and see if the it hits any colliders.
			var rayCastInput = new RaycastInput
			{
				Ray = ray, Filter = new CollisionFilter
				{
					CategoryBits = ~0u << 2,
					MaskBits = ~0u << 2
				}
			};

			if (collisionWorld.CastRay(rayCastInput, out RaycastHit hit))
			{
				hitBody = collisionWorld.Bodies[hit.RigidBodyIndex];
				fraction = hit.Fraction;
			}

			// If we have a hit, then we'll update the singleton data with
			// the x and z values. (we're not doing anything with y at the moment)
			if (hitBody != null)
			{
				pointInWorld = ray.Origin + ray.Direction * fraction;
				return true;
			}

			// default value
			pointInWorld = float3.zero;
			return false;
		}
	}
}