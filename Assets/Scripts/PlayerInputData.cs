using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Ship.Project
{
	[Serializable]
	public struct PlayerInputData : IComponentData
	{
		public float2 Move;
	}
}