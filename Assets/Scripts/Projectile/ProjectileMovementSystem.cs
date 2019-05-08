using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Ship.Project
{
	[UpdateAfter(typeof(ProjectileSpawnSystem))]
	public class ProjectileMovementSystem : JobComponentSystem
	{
		[BurstCompile]
		public struct ProjectileMovementJob : IJobForEach<Projectile, Translation, Rotation>
		{
			public float DeltaTime;
			
			public void Execute(ref Projectile projectile, ref Translation translation, ref Rotation rotation)
			{
				var moveDistance = DeltaTime * projectile.Velocity;
				float3 direction = math.normalize(projectile.EndPosition - projectile.StartPosition);
				float3 position = projectile.CurrentPosition + direction * moveDistance;
				quaternion rot = quaternion.LookRotation(direction, Vector3.up);

				projectile.CurrentPosition = position;
				projectile.Velocity = math.max(0, projectile.Velocity - projectile.Decay);

				projectile.CurrentPosition.y = math.max(
					x: -2f,
					y: projectile.CurrentPosition.y - 0.0075f
				);

				if (projectile.Velocity <= 0f) {
				}
				
				translation.Value = projectile.CurrentPosition;
				rotation.Value = rot;
			}
		}
		
		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			var job = new ProjectileMovementJob { DeltaTime = Time.deltaTime };
			return job.Schedule(this, inputDeps);
		}
	}
}