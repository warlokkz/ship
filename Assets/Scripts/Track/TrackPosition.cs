using Unity.Entities;
using Unity.Mathematics;

namespace Ship.Project.Track
{
	public struct TrackPosition : IComponentData
	{
		public float worldLength;
		public float3 startPosition;
		public float3 endPosition;
	}
}