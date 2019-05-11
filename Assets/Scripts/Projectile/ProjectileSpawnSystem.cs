using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;
using UnityEngine;

namespace Ship.Project
{
	[UpdateAfter(typeof(PlayerInputSystem))]
	public class ProjectileSpawnSystem : JobComponentSystem
	{
		private EntityQuery _spawnerEntityQuery;
		private EntityQuery _navAgentEntityQuery;
		private EntityManager _entityManager;
		private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;

		[BurstCompile]
		private struct ProjectileSpawnJob : IJobForEach<PlayerInputSystem.FireInputData>
		{
			[ReadOnly]
			public EntityCommandBuffer CommandBuffer;
			public Entity Prefab;
			public float3 SpawnPosition;

			public void Execute(ref PlayerInputSystem.FireInputData fireInput)
			{
				if (float.IsPositiveInfinity(fireInput.FireDirection.x)) { return; }

				Entity instance =  CommandBuffer.Instantiate(Prefab);
				CommandBuffer.SetComponent(instance, new Translation { Value = SpawnPosition });

				CommandBuffer.SetComponent(instance, new PhysicsVelocity
				{
					Linear = new float3
					{
						x = (fireInput.FireDirection.x - SpawnPosition.x) / 5f,
						y = 9.8f / 4f,
						z = (fireInput.FireDirection.z - SpawnPosition.z) / 5f
					}
				});
				
				float3 direction = math.normalize(fireInput.FireDirection - SpawnPosition);
				quaternion rot = quaternion.LookRotation(direction, Vector3.up);
				
				CommandBuffer.SetComponent(instance, new PhysicsMass {
					Transform = { rot = rot }
				});

				CommandBuffer.AddComponent(instance, new Projectile());
			}
		}
		
		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			var projectileSpawner = _entityManager.GetComponentData<ProjectileSpawnerComponent>(
				_spawnerEntityQuery.GetSingletonEntity()
			);

			var navAgent = _entityManager.GetComponentData<NavAgent>(
				_navAgentEntityQuery.GetSingletonEntity()
			);
			
			JobHandle job = new ProjectileSpawnJob
			{
				CommandBuffer = _entityCommandBufferSystem.CreateCommandBuffer(),
				Prefab = projectileSpawner.Prefab,
				SpawnPosition = navAgent.Position
			}.Schedule(this, inputDeps);

			_entityCommandBufferSystem.AddJobHandleForProducer(job);

			return job;
		}

		protected override void OnCreateManager()
		{
			_entityManager = World.EntityManager;
			_entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
			_spawnerEntityQuery = GetEntityQuery(typeof(ProjectileSpawnerComponent));
			_navAgentEntityQuery = GetEntityQuery(typeof(NavAgent));
		}
	}
}