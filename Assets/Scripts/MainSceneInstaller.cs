using UnityEngine;
using Zenject;

public class MainSceneInstaller : MonoInstaller
{
    [SerializeField]
    private GameInput _gameInput;
    
    public override void InstallBindings()
    {
        Container.Bind<IGameInput>().FromInstance(_gameInput).AsSingle();
        
        Container.Bind<IWallHighlighter>().To<WallHighlighter>().AsSingle();
        Container.Bind<IWallSelector>().To<WallSelector>().AsSingle();
    }
}