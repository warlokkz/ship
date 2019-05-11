using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace Ship.Project
{
	[UpdateAfter(typeof(StepPhysicsWorld))]
	public class ProjectileDespawnSystem : JobComponentSystem
	{
		EntityCommandBufferSystem barrier;

		
		[BurstCompile]
		public struct ProjectileDespawnJob : IJobForEachWithEntity<Projectile, Translation>
		{
			[WriteOnly]
			public EntityCommandBuffer.Concurrent CommandBuffer;
			
			public void Execute(Entity entity, int jobIndex, ref Projectile projectile, ref Translation trans)
			{
				if (trans.Value.y <= -2f)
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