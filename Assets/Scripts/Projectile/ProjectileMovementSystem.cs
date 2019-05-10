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
	[UpdateAfter(typeof(ProjectileSpawnSystem))]
	public class ProjectileMovementSystem : JobComponentSystem
	{
		EntityCommandBufferSystem barrier;
		
		[BurstCompile]
		public struct ProjectileMovementJob : IJobForEachWithEntity<Projectile, PhysicsMass, PhysicsVelocity>
		{
			public float DeltaTime;
			[WriteOnly]
			public EntityCommandBuffer.Concurrent CommandBuffer;
			
			public void Execute(Entity entity, int index, ref Projectile projectile, ref PhysicsMass mass, ref PhysicsVelocity vel)
			{
				var moveDistance = DeltaTime * projectile.Velocity;
				float3 direction = math.normalize(projectile.EndPosition - projectile.StartPosition);
				float3 position = projectile.CurrentPosition + direction * moveDistance;
				quaternion rot = quaternion.LookRotation(direction, Vector3.up);

				projectile.CurrentPosition = position;
				projectile.Velocity = math.max(0, projectile.Velocity - projectile.Decay);
				
				mass.Transform.rot = rot;
				CommandBuffer.SetComponent(index, entity, mass);
			}
		}
		
		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			EntityCommandBuffer.Concurrent commandBuffer = barrier.CreateCommandBuffer().ToConcurrent();

			var job = new ProjectileMovementJob
			{
				DeltaTime = Time.deltaTime,
				CommandBuffer = commandBuffer
			}.Schedule(this, inputDeps);
			
			job.Complete();

			return job;
		}

		protected override void OnCreate()
		{
			barrier = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
		}
	}
}