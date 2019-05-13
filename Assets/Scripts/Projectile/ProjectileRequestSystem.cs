using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Ship.Project
{
	public class ProjectileRequestSystem : JobComponentSystem
	{
		private EntityQuery _navAgentEntityQuery;
		private EntityManager _entityManager;
		private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;

		public struct ProjectileSpawnRequest : IComponentData
		{
			public float3 StartPosition;
			public float3 EndPosition;
			public Entity Owner;
		}

		[BurstCompile]
		private struct ProjectileSpawnRequestJob : IJobForEach<PlayerInputSystem.FireInputData>
		{
			[ReadOnly]
			public EntityCommandBuffer CommandBuffer;
			public float3 StartPosition;
			public Entity Owner;

			public void Execute(ref PlayerInputSystem.FireInputData fireInput)
			{
				// make sure the fireInput is valid
				if (float.IsPositiveInfinity(fireInput.destination.x)) { return; }

				Entity entity = CommandBuffer.CreateEntity();
				CommandBuffer.AddComponent(entity, new ProjectileSpawnRequest
				{
					Owner = Owner,
					StartPosition = StartPosition,
					EndPosition = fireInput.destination
				});
			}
		}
		
		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			var playerEntity = _navAgentEntityQuery.GetSingletonEntity();
			var navAgent = _entityManager.GetComponentData<NavAgent>(playerEntity);
			
			JobHandle job = new ProjectileSpawnRequestJob {
				CommandBuffer = _entityCommandBufferSystem.CreateCommandBuffer(),
				Owner = playerEntity,
				StartPosition = navAgent.Position
			}.Schedule(this, inputDeps);

			_entityCommandBufferSystem.AddJobHandleForProducer(job);

			return job;
		}

		protected override void OnCreateManager()
		{
			_entityManager = World.EntityManager;
			_entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
			_navAgentEntityQuery = GetEntityQuery(typeof(NavAgent));
		}
	}
}