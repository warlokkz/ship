using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Ship.Project
{
	[UpdateAfter(typeof(ClickSystem))]
	public class CursorSpawnSystem : JobComponentSystem
	{
		private Entity _clickDataEntity;
		private EntityManager _entityManager;
		private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;

		[BurstCompile]
		private struct WaypointSpawnJob : IJobForEach<CursorComponent>
		{
			public float3 ClickDestination;
			public EntityCommandBuffer CommandBuffer;

			public void Execute(ref CursorComponent cursorComponent)
			{
				var instance = CommandBuffer.Instantiate(cursorComponent.Prefab);
				CommandBuffer.SetComponent(instance, new Translation { Value = ClickDestination });
			}
		}

		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			ClickSystem.ClickData clickData = _entityManager.GetComponentData<ClickSystem.ClickData>(_clickDataEntity);
			
			var job = new WaypointSpawnJob
			{
				ClickDestination = clickData.ClickDestination,
				CommandBuffer = _entityCommandBufferSystem.CreateCommandBuffer()
			}.ScheduleSingle(this, inputDeps);

			_entityCommandBufferSystem.AddJobHandleForProducer(job);

			return job;
		}

		protected override void OnCreateManager()
		{
			_entityManager = World.EntityManager;
			_clickDataEntity = GetSingletonEntity<ClickSystem.ClickData>();
			_entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
		}
	}
}