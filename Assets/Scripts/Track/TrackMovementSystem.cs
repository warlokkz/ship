using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Ship.Project.Track
{
	[UpdateAfter(typeof(GenerateMoveTickSystem))]
	public class TrackMovementSystem : JobComponentSystem
	{
		private Entity _moveTickEntity;
		private EntityManager _entityManager;
		private EndSimulationEntityCommandBufferSystem _barrier;
		
		[BurstCompile]
		[RequireComponentTag(typeof(BelongsToTrack))]
		private struct TrackMovementJob : IJobForEachWithEntity<Translation>
		{
			[ReadOnly]
			public EntityCommandBuffer.Concurrent CommandBuffer;

			public float deltaDistance;
			
			public void Execute(Entity entity, int index, ref Translation translation)
			{
				CommandBuffer.SetComponent(index, entity, new Translation
					{
						Value = new float3
						{
							x = translation.Value.x - deltaDistance,
							y = translation.Value.y,
							z = translation.Value.z
						}
					}
				);
			}
		}
		
		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			var moveTick = _entityManager.GetComponentData<MoveTick>(_moveTickEntity);
			
			var job = new TrackMovementJob
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