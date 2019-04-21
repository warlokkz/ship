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
		public float MoveSpeed;
		
		public float Acceleration { get; set; }
		public float CurrentMoveSpeed { get; set; }
		public float3 Destination { get; set; }
		public float3 Position { get; set; }
		public float RemainingDistance { get; set; }
		public AgentStatus Status { get; set; }
		public Quaternion Rotation { get; set; }


		public NavAgent(
			float moveSpeed,
			float3 position,
			Quaternion rotation,
			float acceleration = 1f
		) {
			MoveSpeed = moveSpeed;
			Position = position;
			Rotation = rotation;

			Acceleration = acceleration;
			CurrentMoveSpeed = 0f;
			Destination = Vector3.zero;
			RemainingDistance = 0f;
			Status = AgentStatus.Idle;
		}
	}
}