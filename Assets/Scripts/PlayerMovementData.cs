using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Ship.Project
{
	[Serializable]
	public struct PlayerMovementData : IComponentData
	{
		public float MoveSpeed;
	}
}