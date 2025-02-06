using System.Threading.Tasks;

namespace Game.Runtime.Systems.Scenes
{
    public interface ISceneLoaderService
    {
        Task LoadSceneGroup(int index);
    }
}