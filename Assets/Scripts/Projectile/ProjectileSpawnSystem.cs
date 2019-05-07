using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Ship.Project
{
	[UpdateAfter(typeof(PlayerInputSystem))]
	public class ProjectileSpawnSystem : JobComponentSystem
	{
		private EntityQuery _eq;
		private EntityManager _entityManager;
		private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;

		[BurstCompile]
		private struct ProjectileSpawnJob : IJobForEach<PlayerInputSystem.FireInputData>
		{
			[ReadOnly]
			public EntityCommandBuffer CommandBuffer;
			public Entity Prefab;

			public void Execute(ref PlayerInputSystem.FireInputData fireInput)
			{
				if (float.IsPositiveInfinity(fireInput.FireDirection.x)) { return; }

				Entity instance =  CommandBuffer.Instantiate(Prefab);
				CommandBuffer.AddComponent(instance, new Projectile());
			}
		}
		
		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			var projectileSpawner = _entityManager.GetComponentData<ProjectileSpawnerComponent>(_eq.GetSingletonEntity());
			
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
			_eq = GetEntityQuery(typeof(ProjectileSpawnerComponent));
		}
	}
}