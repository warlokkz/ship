using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Ship.Project
{
	public enum AgentStatus
	{
		Idle = 0,
		Moving = 1
	}
	public struct NavAgent : IComponentData
	{
		public float Acceleration;
		public float MoveSpeed;
		public float3 Position;
		public Quaternion Rotation;
		public float RotationSpeed;

		public float CurrentMoveSpeed { get; set; }
		public float3 Destination { get; set; }
		public float3 NextPosition { get; set; }
		public float RemainingDistance { get; set; }
		public AgentStatus Status { get; set; }


		public NavAgent(
			float3 position,
			Quaternion rotation,
			float acceleration = 1f,
			float moveSpeed = 4f,
			float rotationSpeed = 10f
		) {
			Position = position;
			Rotation = rotation;

			Acceleration = acceleration;
			CurrentMoveSpeed = 0f;
			Destination = Vector3.zero;
			MoveSpeed = moveSpeed;
			NextPosition = Vector3.zero;
			RemainingDistance = 0f;
			RotationSpeed = rotationSpeed;
			Status = AgentStatus.Idle;
		}
	}
}