using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

namespace Ship.Project
{
	[UpdateAfter(typeof(NavigationSystem))]
	public class NavAgentSyncSystem : JobComponentSystem
	{
		[BurstCompile]
		private struct NavAgentSyncJob : IJobForEach<NavAgent, Translation, Rotation>
		{
			public NavAgentSyncJob(JobHandle inputDeps)
			{
				throw new System.NotImplementedException();
			}

			public void Execute(ref NavAgent navAgent, ref Translation translation, ref Rotation rotation)
			{
				translation.Value = navAgent.Position;
				rotation.Value = navAgent.Rotation;
			}
		}
		
		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			var job = new NavAgentSyncJob();
			return job.Schedule(this, inputDeps);
		}
	}
}