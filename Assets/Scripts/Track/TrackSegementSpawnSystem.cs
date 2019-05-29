using Unity.Entities;
using Unity.Jobs;

namespace Ship.Project.Track
{

	public struct SpawnRequestComponent : IComponentData { }
	public class TrackSegementSpawnSystem : JobComponentSystem
	{
		private Entity _positionEntity;
		private EntityQuery _spawnerEntityQuery;
		private EntityManager _entityManager;
		private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;
		protected override void OnCreate()
		{
			_entityManager = World.EntityManager;
			_entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
			_spawnerEntityQuery = GetEntityQuery(typeof(TrackSpawnerComponent));
		}

		// todo: WIP, need to figure out the position where to spawn the next track
		private struct SpawnTrackSegmentJob : IJobForEach<SpawnRequestComponent>
		{
			public void Execute(ref SpawnRequestComponent request)
			{
			}
		}

		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			_positionEntity = GetSingletonEntity<TrackWorldPosition>();
			var trackPosition = _entityManager.GetComponentData<TrackWorldPosition>(_positionEntity);
			var trackSpawner = _entityManager.GetComponentData<TrackSpawnerComponent>(
				_spawnerEntityQuery.GetSingletonEntity()
			);
			
			var job = new SpawnTrackSegmentJob();

			return job.Schedule(this, inputDeps);
		}
	}
}