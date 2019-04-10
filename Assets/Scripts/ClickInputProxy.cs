using Unity.Entities;
using UnityEngine;

namespace Ship.Project
{
	[RequiresEntityConversion]
	public class ClickInputProxy : MonoBehaviour, IConvertGameObjectToEntity
	{
		// The MonoBehaviour data is converted to ComponentData on the entity.
		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			var data = new ClickInputData
			{
				Click = 0
			};
			dstManager.AddComponentData(entity, data);
		}
	}
}