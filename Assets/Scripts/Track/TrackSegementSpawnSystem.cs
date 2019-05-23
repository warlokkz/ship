using Unity.Entities;
using UnityEngine;

namespace Ship.Project.Track
{
	public class TrackSegementSpawnSystem : ComponentSystem
	{
		private Entity _positionEntity;
		private EntityManager _entityManager;
		protected override void OnUpdate()
		{
			_positionEntity = GetSingletonEntity<TrackWorldPosition>();
			var trackPosition = _entityManager.GetComponentData<TrackWorldPosition>(_positionEntity);
		}
		protected override void OnCreate()
		{
			_entityManager = World.EntityManager;
		}
	}
}