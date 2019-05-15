using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace Ship.Project
{
	[UpdateAfter(typeof(BuildPhysicsWorld))]
	public class TrackMovementSystem : JobComponentSystem
	{

		private EndSimulationEntityCommandBufferSystem _barrier;
		
		[BurstCompile]
		[RequireComponentTag(typeof(TrackComponent))]
		private struct TrackMovementJob : IJobForEachWithEntity<Translation>
		{
			[ReadOnly]
			public EntityCommandBuffer.Concurrent CommandBuffer;
			public float DeltaTime;
			
			public void Execute(Entity entity, int index, ref Translation translation)
			{
				var speed = 1f;

				CommandBuffer.SetComponent(index, entity, new Translation
					{
						Value = new float3
						{
							x = translation.Value.x - speed * DeltaTime,
							y = translation.Value.y,
							z = translation.Value.z
						}
					}
				);
			}
		}
		
		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			var job = new TrackMovementJob
			{
				CommandBuffer = _barrier.CreateCommandBuffer().ToConcurrent(),
				DeltaTime = Time.deltaTime
			}.Schedule(this, inputDeps);
			
			_barrier.AddJobHandleForProducer(job);

			return job;
		}

		protected override void OnCreate()
		{
			_barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
		}
	}
}