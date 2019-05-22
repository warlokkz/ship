using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

namespace Ship.Project.Track
{
	[UpdateAfter(typeof(GenerateMoveTickSystem))]
	public class ProjectileTrackSyncSystem : JobComponentSystem
	{
		private Entity _moveTickEntity;
		private EntityManager _entityManager;
		private EndSimulationEntityCommandBufferSystem _barrier;
		
		[BurstCompile]
		[RequireComponentTag(typeof(Projectile))]
		private struct ProjectileSyncJob : IJobForEachWithEntity<Translation>
		{
			[ReadOnly]
			public EntityCommandBuffer.Concurrent CommandBuffer;

			public float deltaDistance;
			
			public void Execute(Entity entity, int index, ref Translation translation)
			{
				translation.Value.x -= deltaDistance;
			}
		}
	
		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			var moveTick = _entityManager.GetComponentData<MoveTick>(_moveTickEntity);
			
			var job = new ProjectileSyncJob
			{
				CommandBuffer = _barrier.CreateCommandBuffer().ToConcurrent(),
				deltaDistance = moveTick.deltaDistance
			}.Schedule(this, inputDeps);
			
			_barrier.AddJobHandleForProducer(job);

			return job;
		}

		protected override void OnCreate()
		{
			_entityManager = World.EntityManager;
			_moveTickEntity = GetSingletonEntity<MoveTick>();
			_barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
		}
	}
}