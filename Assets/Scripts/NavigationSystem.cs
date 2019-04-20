using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Ship.Project {
    
    [DisableAutoCreation]
    public class NavigationSystem : JobComponentSystem
    {
        private struct AgentData
        {
            public int Index;
            public Entity Entity;
        }

        [BurstCompile]
        private struct MovementJob : IJobForEach<NavAgent>
        {
            private readonly float DeltaTime;
            private readonly float3 Up;
            private readonly float3 One;

            public MovementJob(float deltaTime)
            {
                DeltaTime = deltaTime;
                Up = Vector3.up;
                One = Vector3.one;
            }

            public void Execute(ref NavAgent agent)
            {
                throw new System.NotImplementedException();
            }
        }
        
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            throw new System.NotImplementedException();
        }
    }
}
