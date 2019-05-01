using Unity.Entities;
using Unity.Jobs;

namespace Ship.Project
{
	[UpdateAfter(typeof(CursorSpawnSystem))]
	public class CursorDespawnSystem : JobComponentSystem
	{
		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			throw new System.NotImplementedException();
		}
	}
}