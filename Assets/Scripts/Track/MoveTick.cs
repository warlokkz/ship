using Unity.Entities;

namespace Ship.Project.Track
{
	public struct MoveTick : IComponentData
	{
		public float deltaDistance;
	}
}