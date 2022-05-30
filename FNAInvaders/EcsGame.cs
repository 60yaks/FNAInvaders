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
    private ISystem<GameTime> _updateSystem;
    private ISystem<GameTime> _drawSystem;
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

        _world = new World(100);

        _runner = new DefaultParallelRunner(Environment.ProcessorCount);

        _textureResourceManager = new TextureResourceManager(GraphicsDevice);
        _textureResourceManager.Manage(_world);

        _updateSystem = new EnumerableSequentialSystem<GameTime>(
            new InputGatherSystem(_world),
            new ApplyInputSystem(_world)
        );
        _drawSystem = new EnumerableSequentialSystem<GameTime>(
            new DrawSystem(_world, _runner, _spriteBatch)
        );

        _world.SetMaxCapacity<GameWorldData>(1);
        _world.Set(new GameWorldData { Bounds = new Rectangle(50, 50, 1180, 620) });

        var player = _world.CreateEntity();
        player.SetSameAsWorld<GameWorldData>();
        player.Set(new PlayerTag());
        player.Set(new InputData());
        player.Set(new SpeedData { Value = 0.75 });
        player.Set(new Position { Value = new Point(640, 670) });
        player.Set(new DrawInfo { Color = Color.White, Scale = 1f });
        player.Set(new ManagedResource<string, Texture2D>("playerShip1_red.png"));

#if DEBUG
        _editor = Editor.EditorWindow.StartEditor(
            new Editor.EditorViewModel<GameTime>(_updateSystem, _drawSystem),
            Window.ClientBounds.Right,
            Window.ClientBounds.Top);
#endif
    }

    protected override void Update(GameTime gameTime)
    {
        _updateSystem.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        _drawSystem.Update(gameTime);
    }

    protected override void Dispose(bool disposing)
    {
#if DEBUG
        _editor.Cancel();
#endif

        _drawSystem.Dispose();
        _runner.Dispose();
        _world.Dispose();
        _textureResourceManager.Dispose();
        _spriteBatch.Dispose();
        ((IDisposable)_deviceManager).Dispose();
        base.Dispose(disposing);
    }
}
