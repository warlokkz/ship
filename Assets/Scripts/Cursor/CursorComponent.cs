using Unity.Entities;

namespace Ship.Project
{
	public struct CursorComponent : IComponentData
	{
		public Entity Prefab;
	}
}