using Unity.Entities;
using UnityEngine;

namespace Ship.Project.Track
{
	[RequiresEntityConversion]
	public class TrackSegmentProxy : MonoBehaviour, IConvertGameObjectToEntity
	{
		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			var component = new RegisterTrackSegment();
			dstManager.AddComponentData(entity, component);
			
			var belongsComponent = new BelongsToTrack();
			dstManager.AddComponentData(entity, belongsComponent);
		}
	}
}
