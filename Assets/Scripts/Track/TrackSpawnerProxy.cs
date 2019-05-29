using Unity.Entities;
using UnityEngine;
using System.Collections.Generic;

namespace Ship.Project.Track
{
	[RequiresEntityConversion]
	public class TrackSpawnerProxy : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
	{
		public GameObject Prefab;
		public float ForegroundZOffset;
		public float BackgroundZOffset;
		
		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			var component = new TrackSpawnerComponent
			{
				Prefab = conversionSystem.GetPrimaryEntity(Prefab),
				foregroundZOffset = ForegroundZOffset,
				backgroundZOffset = BackgroundZOffset
			};
			dstManager.AddComponentData(entity, component);
		}

		public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
		{
			referencedPrefabs.Add(Prefab);
		}
	}
	
	public struct TrackSpawnerComponent : IComponentData
	{
		public Entity Prefab;
		public float foregroundZOffset;
		public float backgroundZOffset;
	}
}
