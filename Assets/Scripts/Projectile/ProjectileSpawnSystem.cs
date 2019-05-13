using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace Ship.Project
{
	[UpdateAfter(typeof(ProjectileRequestSystem))]
	public class ProjectileSpawnSystem : JobComponentSystem
	{
		private EntityQuery _spawnerEntityQuery;
		private EntityManager _entityManager;
		private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;

		[BurstCompile]
		private struct ProjectileSpawnJob : IJobForEachWithEntity<ProjectileRequestSystem.ProjectileSpawnRequest>
		{
			[ReadOnly]
			public EntityCommandBuffer CommandBuffer;
			public Entity Prefab;

			public void Execute(Entity entity, int index, ref ProjectileRequestSystem.ProjectileSpawnRequest spawnRequest)
			{
				Entity instance =  CommandBuffer.Instantiate(Prefab);
				
				CommandBuffer.SetComponent(instance, new Translation { Value = spawnRequest.StartPosition });
				CommandBuffer.SetComponent(instance, new PhysicsVelocity
				{
					// TechDebt: these values should be configurable from a ProjectileSettings component
					// or a singleton entity.
					Linear = new float3
					{
						x = (spawnRequest.EndPosition.x - spawnRequest.StartPosition.x) / 5f,
						y = 9.8f / 4f,
						z = (spawnRequest.EndPosition.z - spawnRequest.StartPosition.z) / 5f
					}
				});
				
				float3 direction = math.normalize(spawnRequest.EndPosition - spawnRequest.StartPosition);
				quaternion rot = quaternion.LookRotation(direction, Vector3.up);
				
				CommandBuffer.SetComponent(instance, new PhysicsMass {
					Transform = { rot = rot }
				});

				CommandBuffer.AddComponent(instance, new Projectile { Owner = spawnRequest.Owner });
				
				// Destroy the spawn request
				CommandBuffer.DestroyEntity(entity);
			}
		}
		
		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			var projectileSpawner = _entityManager.GetComponentData<ProjectileSpawnerComponent>(
				_spawnerEntityQuery.GetSingletonEntity()
			);

			JobHandle job = new ProjectileSpawnJob
			{
				CommandBuffer = _entityCommandBufferSystem.CreateCommandBuffer(),
				Prefab = projectileSpawner.Prefab
			}.Schedule(this, inputDeps);

			_entityCommandBufferSystem.AddJobHandleForProducer(job);

			return job;
		}

		protected override void OnCreateManager()
		{
			_entityManager = World.EntityManager;
			_entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
			_spawnerEntityQuery = GetEntityQuery(typeof(ProjectileSpawnerComponent));
		}
	}
}