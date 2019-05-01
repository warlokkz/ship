using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Ship.Project
{
	[RequiresEntityConversion]
	public class CursorProxy : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
	{
		public GameObject Prefab;
		
		public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
		{
			referencedPrefabs.Add(Prefab);
		}

		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			var cursorData = new CursorComponent
			{
				Prefab = conversionSystem.GetPrimaryEntity(Prefab)
			};
			dstManager.AddComponentData(entity, cursorData);
		}
	}
}