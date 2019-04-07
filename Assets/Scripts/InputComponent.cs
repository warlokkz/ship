using System;
using Unity.Entities;

namespace Ship.Project
{
	[Serializable]
	public class InputComponent : IComponentData
	{
		public float Horizontal;
		public float Vertical;
	}
}
