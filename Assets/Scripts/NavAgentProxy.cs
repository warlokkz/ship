using Unity.Entities;
using UnityEngine;

namespace Ship.Project
{
	[RequiresEntityConversion]
	public class NavAgentProxy : MonoBehaviour, IConvertGameObjectToEntity
	{
		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			var data = new NavAgent();
			
			dstManager.AddComponentData(entity, data);
		}
	}
}