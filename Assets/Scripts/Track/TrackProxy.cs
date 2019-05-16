using Unity.Entities;
using UnityEngine;

namespace Ship.Project.Track
{
	[RequiresEntityConversion]
	public class TrackProxy : MonoBehaviour, IConvertGameObjectToEntity
	{
		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			var component = new BelongsToTrack();
			dstManager.AddComponentData(entity, component);
		}
	}
}