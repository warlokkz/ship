using Unity.Entities;
using UnityEngine;

namespace Ship.Project
{
	public struct Clickable : IComponentData { }

	[RequiresEntityConversion]
	public class InputProxy : MonoBehaviour, IConvertGameObjectToEntity
	{
		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			dstManager.AddComponentData(entity, new Clickable());
		}
	}
}