using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Ship.Project
{
	public class PlayerInputSystem : ComponentSystem
	{
		protected override void OnUpdate()
		{
			Entities.ForEach((Entity entity, ref PlayerInputData input)  =>
			{
				
				var inputData = new PlayerInputData
				{
					Move = new float3(
						Input.GetAxisRaw("Horizontal"),
						0,
						Input.GetAxisRaw("Vertical")
					)
				};
				
				PostUpdateCommands.SetComponent(entity, inputData);
			});
		}
	}
}