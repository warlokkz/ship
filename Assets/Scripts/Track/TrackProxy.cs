using Unity.Entities;
using UnityEngine;

namespace Ship.Project
{
	[RequiresEntityConversion]
	public class TrackProxy : MonoBehaviour, IConvertGameObjectToEntity
	{
		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			var component = new TrackComponent();
			dstManager.AddComponentData(entity, component);
		}
	}
}