using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Ship.Project
{
	[RequiresEntityConversion]
	public class ProjectileSpawner : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
	{
		public GameObject Prefab;
		
		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			var component = new ProjectileSpawnerComponent { Prefab = conversionSystem.GetPrimaryEntity(Prefab) };
			dstManager.AddComponentData(entity, component);
		}

		public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
		{
			referencedPrefabs.Add(Prefab);
		}
	}

	public struct ProjectileSpawnerComponent : IComponentData
	{
		public Entity Prefab;
	}
}