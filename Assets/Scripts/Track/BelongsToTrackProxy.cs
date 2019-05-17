using Unity.Entities;
using UnityEngine;

namespace Ship.Project.Track
{
	[RequiresEntityConversion]
	public class BelongsToTrackProxy : MonoBehaviour, IConvertGameObjectToEntity
	{
		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			var belongsComponent = new BelongsToTrack();
			dstManager.AddComponentData(entity, belongsComponent);
		}
	}
}