using Unity.Entities;
using UnityEngine;

namespace Ship.Project
{
	public class ShipMovementSystem : ComponentSystem
	{
		protected override void OnUpdate()
		{
			var deltaTime = Time.deltaTime;
			Entities.ForEach((Rigidbody rigidBody, InputComponent inputComponent) =>
			{
				var moveVector = new Vector3(inputComponent.Horizontal, 0, inputComponent.Vertical);
				var movePosition = rigidBody.position + moveVector.normalized * 3 * deltaTime;
				rigidBody.MovePosition(movePosition);
			});
		}
	}
}
