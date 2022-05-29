using DefaultEcs;
using DefaultEcs.Resource;
using FNAInvaders.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FNAInvaders.Infrastructure;

internal sealed class TextureResourceManager : AResourceManager<string, Texture2D>
{
    private readonly GraphicsDevice _graphicsDevice;
    public TextureResourceManager(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
    }

    protected override Texture2D Load(string info) => Texture2D.FromStream(_graphicsDevice, File.OpenRead(Path.Combine("Content", info)));

    protected override void OnResourceLoaded(in Entity entity, string info, Texture2D resource)
    {
        entity.Get<DrawComponent>().Texture = resource;
        entity.Get<DrawComponent>().Size = new Point(resource.Width, resource.Height);
    }
}
