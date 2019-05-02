using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Ship.Project
{
	
	[RequiresEntityConversion]
	public class Cursor : MonoBehaviour, IConvertGameObjectToEntity
	{
		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			dstManager.AddComponentData(entity, new CursorComponent());
			dstManager.AddComponentData(entity, new CursorLifetimeComponent { Value = 0.5f });
		}
	}
	
	public struct CursorComponent : IComponentData
	{
		public float3 Translation;
	}
}