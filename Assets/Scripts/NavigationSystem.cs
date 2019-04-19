using System.Collections.Concurrent;
using System.Numerics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine.AI;

namespace Ship.Project {
    
    [DisableAutoCreation]
    public class NavigationSystem : JobComponentSystem
    {
        private struct AgentData
        {
            public int Index;
            public Entity Entity;
        }

        private NativeQueue<AgentData> _needsWayPoint;
        private ConcurrentDictionary<int, Vector3[]> _waypoints = new ConcurrentDictionary<int, Vector3[]>();
        private NativeHashMap<int, AgentData> _pathFindingData;
        
        [BurstCompile]
        private struct DetectNextWaypointJob : IJobParallelFor
        {
            public int NavMeshQuerySystemVersion;
            public NativeQueue<AgentData>.Concurrent NeedsWayPoint;

            public void Execute(int index)
            {
                
            }
        }

        [BurstCompile]
        private struct MovementJob : IJobParallelFor
        {
            
            
            public void Execute(int index)
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
