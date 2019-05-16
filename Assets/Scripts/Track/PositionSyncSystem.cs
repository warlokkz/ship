using Unity.Entities;

namespace Ship.Project.Track
{
	[UpdateAfter(typeof(MovementSystem))]
	public class PositionSyncSystem : ComponentSystem
	{
		private Entity _moveTickEntity;
		private Entity _positionEntity;
		private EntityManager _entityManager;

		protected override void OnUpdate()
		{
			var moveTick = _entityManager.GetComponentData<MoveTick>(_moveTickEntity);
			var trackPosition = _entityManager.GetComponentData<TrackPosition>(_positionEntity);
			
			SetSingleton(new TrackPosition
			{
				worldLength = trackPosition.worldLength - moveTick.deltaDistance
			});
		}

		protected override void OnCreate()
		{
			_entityManager = World.EntityManager;
			_moveTickEntity = GetSingletonEntity<MoveTick>();
			
			_entityManager.CreateEntity(typeof(TrackPosition));
			SetSingleton(new TrackPosition());

			_positionEntity = GetSingletonEntity<TrackPosition>();
		}
	}
}