using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Ship.Project
{
	[RequiresEntityConversion]
	public class PlayerInputComponentProxy : MonoBehaviour, IConvertGameObjectToEntity
	{
		public float2 Move;
		
		// The MonoBehaviour data is converted to ComponentData on the entity.
		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			var data = new PlayerInputData
			{
				Move = Move
			};
			
			dstManager.AddComponentData(entity, data);
		}
	}
}