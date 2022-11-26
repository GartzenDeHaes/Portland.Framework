
namespace Portland.SceneGraph
{
	public interface IMonoBehaviour : IComponent
	{
		bool enabled { get; }
		GameObject gameObject { get; }
	}
}