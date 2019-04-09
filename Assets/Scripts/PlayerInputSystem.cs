using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Ship.Project
{
	public class PlayerInputSystem : ComponentSystem
	{
		private EntityQuery Query;

		protected override void OnCreate()
		{
			Query = GetEntityQuery(
				ComponentType.ReadOnly<PlayerInputData>()
			);
		}

		protected override void OnUpdate()
		{
			Entities.With(Query).ForEach(entity =>
			{
				var inputData = new PlayerInputData
				{
					Move = new float2(
						Input.GetAxisRaw("Horizontal"),
						Input.GetAxisRaw("Vertical")
					)
				};
				
				PostUpdateCommands.SetComponent(entity, inputData);
			});
		}
	}
}