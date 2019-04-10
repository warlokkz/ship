using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Ship.Project
{
	public class MovementSystem : JobComponentSystem
	{
		[BurstCompile]
		struct MovementJob : IJobForEach<Translation, PlayerInputData>
		{
			public float DeltaTime;

			public void Execute(ref Translation translation, [ReadOnly] ref PlayerInputData input)
			{
				translation.Value = new float3
				{
					x = translation.Value.x + input.Move.x * DeltaTime,
					y = translation.Value.y,
					z = translation.Value.z + input.Move.z * DeltaTime
				};
			}
		}

		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			var job = new MovementJob
			{
				DeltaTime = Time.deltaTime
			};

			return job.Schedule(this, inputDeps);
		}
	}
}
