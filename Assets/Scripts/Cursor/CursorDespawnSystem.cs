using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace Ship.Project
{
	public struct CursorLifetimeComponent : IComponentData
	{
		public float Value;
	};

	[UpdateAfter(typeof(CursorSpawnSystem))]
	public class CursorDespawnSystem : JobComponentSystem
	{
		EntityCommandBufferSystem barrier;

		[BurstCompile]
		private struct CursorDespawnJob : IJobForEachWithEntity<CursorLifetimeComponent>
		{
			public float DeltaTime;

			[WriteOnly]
			public EntityCommandBuffer.Concurrent CommandBuffer;
			public void Execute(Entity entity, int jobIndex, ref CursorLifetimeComponent lifetime)
			{
				lifetime.Value -= DeltaTime;
				if (lifetime.Value <= 0.01f)
				{
					CommandBuffer.DestroyEntity(jobIndex, entity);
				}
			}
		}
		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			EntityCommandBuffer.Concurrent commandBuffer = barrier.CreateCommandBuffer().ToConcurrent();

			var job = new CursorDespawnJob
			{
				DeltaTime = Time.deltaTime,
				CommandBuffer = commandBuffer
			}.Schedule(this, inputDeps);
			
			barrier.AddJobHandleForProducer(job);
			
			job.Complete();

			return job;
		}

		protected override void OnCreate()
		{
			barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
		}
	}
}