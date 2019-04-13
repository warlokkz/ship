using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Ship.Project
{
	[RequiresEntityConversion]
	public class PlayerMovementProxy : MonoBehaviour, IConvertGameObjectToEntity
	{
		public float MoveSpeed;
		
		// The MonoBehaviour data is converted to ComponentData on the entity.
		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			var data = new PlayerMovementData
			{
				MoveSpeed = MoveSpeed
			};
			
			dstManager.AddComponentData(entity, data);
		}
	}
}