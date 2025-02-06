using System.Threading.Tasks;

namespace Game.Runtime.Systems.Scenes
{
    public interface ISceneGroupManager
    {
        public Task LoadSceneGroup(int index);
    }
}