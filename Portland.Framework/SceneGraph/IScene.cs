namespace Portland.SceneGraph
{
	public interface IScene
	{
		IEntity SceneGraph { get; }
		string SceneName { get; }

		void AddComponentTo<T>(IEntity ent) where T : IComponent;
		void AddSystem(ISystem sys);
		IEntity CreateEnitity(IEntity parent = null);
		void DestroyEntity(IEntity entity);
		void ReleaseComponent(IComponent c);
		void RemoveComponentFrom<T>(IEntity ent) where T : IComponent;
		void Update(float deltaTime);
	}
}