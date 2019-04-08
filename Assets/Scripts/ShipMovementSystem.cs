using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Ship.Project
{
	public class ShipMovementSystem : ComponentSystem
	{
		protected override void OnUpdate()
		{
			Entities.ForEach<Translation, InputComponent>((ref Translation translation, ref InputComponent inputComponent) =>
			{
				var deltaTime = Time.deltaTime;

				var y = translation.Value.y + inputComponent.Vertical * deltaTime;
				
				if (translation.Value.y >= 3)
				{
					y = translation.Value.y - inputComponent.Vertical * deltaTime;
				}

				if (translation.Value.y <= -3)
				{
					y = translation.Value.y + inputComponent.Vertical * deltaTime;
				}
				
				translation.Value = new float3
				{
					x = translation.Value.x + inputComponent.Horizontal * deltaTime,
					y = y,
					z = 0
				};
			});
		}
	}
}
