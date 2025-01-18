using System.Threading.Tasks;

namespace Game.Runtime.Systems.SceneManagement
{
    public interface ISceneGroupManager
    {
        public Task LoadSceneGroup(int index);
    }
}