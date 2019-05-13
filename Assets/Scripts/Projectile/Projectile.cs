using Unity.Entities;

namespace Ship.Project
{
	public struct Projectile : IComponentData
	{
		public Entity Owner;
	}
}
