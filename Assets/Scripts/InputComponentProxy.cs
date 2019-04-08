using Unity.Entities;
using UnityEngine;

namespace Ship.Project
{
	[RequiresEntityConversion]
	public class InputComponentProxy : MonoBehaviour, IConvertGameObjectToEntity
	{
		public float Horizontal;
		public float Vertical;
		
		// The MonoBehaviour data is converted to ComponentData on the entity.
		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			var data = new InputComponent
			{
				Horizontal = Horizontal,
				Vertical = Vertical
			};
			
			dstManager.AddComponentData(entity, data);
		}
	}
}