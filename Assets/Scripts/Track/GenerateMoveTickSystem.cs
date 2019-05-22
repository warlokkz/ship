using Unity.Entities;
using UnityEngine;

namespace Ship.Project.Track
{
	public class GenerateMoveTickSystem : ComponentSystem
	{
		protected override void OnCreate()
		{
			EntityManager.CreateEntity(typeof(MoveTick));
			SetSingleton(new MoveTick
			{
				deltaDistance = 0f
			});
		}

		protected override void OnUpdate()
		{
			const float speed = 2f;
			SetSingleton(new MoveTick
			{
				deltaDistance = speed * Time.deltaTime
			});
		}
	}
}