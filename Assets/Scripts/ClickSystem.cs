using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Ship.Project
{
	public class ClickSystem : ComponentSystem
	{
		protected override void OnUpdate()
		{
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Entities.ForEach((Entity entity, ref ClickInputData input) =>
			{
				if (Input.GetMouseButtonDown(1))
				{
					if (Physics.Raycast(ray, out var hit, 100))
					{
					}
				}
			});
		}
	}
}