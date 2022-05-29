using DefaultEcs;
using DefaultEcs.Resource;
using DefaultEcs.System;
using DefaultEcs.Threading;
using FNAInvaders.Components;
using FNAInvaders.Infrastructure;
using FNAInvaders.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FNAInvaders;

internal class EcsGame : Game
{
    private readonly GraphicsDeviceManager _deviceManager;
    private SpriteBatch _spriteBatch;
    private TextureResourceManager _textureResourceManager;
    private World _world;
    private ISystem<GameTime> _mainSystem;
    private DefaultParallelRunner _runner;

#if DEBUG
    private CancellationTokenSource _editor;
#endif

    public EcsGame()
    {
        _deviceManager = new GraphicsDeviceManager(this);

        _deviceManager.GraphicsProfile = GraphicsProfile.HiDef;
        _deviceManager.PreferredBackBufferWidth = 1280;
        _deviceManager.PreferredBackBufferHeight = 720;
        _deviceManager.IsFullScreen = false;
        _deviceManager.SynchronizeWithVerticalRetrace = true;
        _deviceManager.ApplyChanges();
    }

    protected override void Initialize()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _world = new World(1000);

        _runner = new DefaultParallelRunner(Environment.ProcessorCount);

        _textureResourceManager = new TextureResourceManager(GraphicsDevice);
        _textureResourceManager.Manage(_world);

        _mainSystem = new CustomSequentialSystem<GameTime>(
            new DrawSystem(_spriteBatch, _world));

        var player = _world.CreateEntity();
        player.Set(new PositionComponent { Value = new Point(640, 360) });
        player.Set(new DrawComponent { Color = Color.White, Scale = 1f });
        player.Set(new ManagedResource<string, Texture2D>("playerShip1_red.png"));

        player = _world.CreateEntity();
        player.Set(new PositionComponent { Value = new Point(320, 360) });
        player.Set(new DrawComponent { Color = Color.White, Scale = 1f });
        player.Set(new ManagedResource<string, Texture2D>("enemyBlue1.png"));

#if DEBUG
        _editor = Editor.EditorWindow.StartEditor(_world, _mainSystem, Window.ClientBounds.Right, Window.ClientBounds.Top);
#endif
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        _mainSystem.Update(gameTime);
    }

    protected override void Dispose(bool disposing)
    {
#if DEBUG
        _editor.Cancel();
#endif

        _mainSystem.Dispose();
        _runner.Dispose();
        _world.Dispose();
        _textureResourceManager.Dispose();
        _spriteBatch.Dispose();
        ((IDisposable)_deviceManager).Dispose();
        base.Dispose(disposing);
    }
}
