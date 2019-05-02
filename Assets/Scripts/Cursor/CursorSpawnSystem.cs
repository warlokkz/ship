using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
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
		private struct CursorSpawnJob : IJobForEach<CursorSpawnerComponent>
		{
			public ClickSystem.ClickData m_ClickData;
			public EntityCommandBuffer CommandBuffer;

			public void Execute(ref CursorSpawnerComponent cursorSpawnerComponent)
			{
				if (!m_ClickData.Active) return;
				Entity instance = CommandBuffer.Instantiate(cursorSpawnerComponent.Prefab);
				CommandBuffer.SetComponent(instance, new Translation {Value = m_ClickData.ClickDestination});
			}
		}

		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			ClickSystem.ClickData clickData = _entityManager.GetComponentData<ClickSystem.ClickData>(_clickDataEntity);
			
			var job = new CursorSpawnJob
			{
				m_ClickData = clickData,
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