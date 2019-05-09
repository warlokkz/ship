using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Ship.Project
{
	[UpdateAfter(typeof(ProjectileMovementSystem))]
	public class ProjectileDespawnSystem : JobComponentSystem
	{
		EntityCommandBufferSystem barrier;

		
		[BurstCompile]
		public struct ProjectileDespawnJob : IJobForEachWithEntity<Projectile>
		{
			[WriteOnly]
			public EntityCommandBuffer.Concurrent CommandBuffer;
			
			public void Execute(Entity entity, int jobIndex, ref Projectile projectile)
			{
				if (projectile.Velocity <= 0f)
				{
					CommandBuffer.DestroyEntity(jobIndex, entity);
				}
			}
		}
		
		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			EntityCommandBuffer.Concurrent commandBuffer = barrier.CreateCommandBuffer().ToConcurrent();

			var job = new ProjectileDespawnJob
			{
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