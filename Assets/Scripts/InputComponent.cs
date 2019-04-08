using System;
using Unity.Entities;

namespace Ship.Project
{
	[Serializable]
	public struct InputComponent : IComponentData
	{
		public float Horizontal;
		public float Vertical;
	}
}
