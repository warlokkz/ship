using Unity.Entities;
using Unity.Rendering;

namespace Ship.Project.Track
{
	[UpdateAfter(typeof(TrackMovementSystem))]
	public class TrackWorldSyncSystem : ComponentSystem
	{
		private Entity _moveTickEntity;
		private Entity _positionEntity;
		private EntityManager _entityManager;
		
		private const float WORLD_LENGTH_MAX = 60f;

		protected override void OnUpdate()
		{
			var moveTick = _entityManager.GetComponentData<MoveTick>(_moveTickEntity);
			var trackPosition = _entityManager.GetComponentData<TrackWorldPosition>(_positionEntity);

			var newWorldLength = trackPosition.worldLength;

			Entities.WithAll<RegisterTrackSegment>().ForEach(
				action: (Entity entity, ref RenderBounds bounds) =>
				{
					newWorldLength += bounds.Value.Size.x;
					PostUpdateCommands.RemoveComponent<RegisterTrackSegment>(entity);
				});
			
			SetSingleton(new TrackWorldPosition
			{
				worldLength = newWorldLength - moveTick.deltaDistance
			});
		}

		protected override void OnCreate()
		{
			_entityManager = World.EntityManager;
			_moveTickEntity = GetSingletonEntity<MoveTick>();
			
			_entityManager.CreateEntity(typeof(TrackWorldPosition));
			SetSingleton(new TrackWorldPosition());

			_positionEntity = GetSingletonEntity<TrackWorldPosition>();
		}
	}
}