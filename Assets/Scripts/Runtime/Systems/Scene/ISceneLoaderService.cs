using System.Threading.Tasks;

namespace Game.Runtime.Systems.SceneManagement
{
    public interface ISceneLoaderService
    {
        Task LoadSceneGroup(int index);
    }
}