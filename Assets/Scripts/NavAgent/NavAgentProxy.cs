using Unity.Entities;
using UnityEngine;

namespace Ship.Project
{
	[RequiresEntityConversion]
	public class NavAgentProxy : MonoBehaviour, IConvertGameObjectToEntity
	{
		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			Transform t = transform;
			var data = new NavAgent(t.position, t.rotation);
			
			dstManager.AddComponentData(entity, data);
		}
	}
}