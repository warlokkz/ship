using Unity.Entities;
using Unity.Mathematics;

namespace Ship.Project
{
	public struct Projectile : IComponentData
	{
		public float3 StartPosition;
		public float3 EndPosition;
		public float3 CurrentPosition;
		public float Decay;
		public float Velocity;
	}
}
