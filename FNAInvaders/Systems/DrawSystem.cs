using DefaultEcs.System;
using FNAInvaders.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FNAInvaders.Systems;

[With(typeof(PositionComponent), typeof(DrawComponent))]
public partial class DrawSystem : AEntitySetSystem<GameTime>
{
    [ConstructorParameter]
    private readonly SpriteBatch _spriteBatch;

    protected override void PreUpdate(GameTime state) => _spriteBatch.Begin();

    [Update]
    private void Update(in PositionComponent position, in DrawComponent drawInfo)
    {
        _spriteBatch.Draw(
            drawInfo.Texture,
            new Vector2(position.Value.X - drawInfo.Size.X * drawInfo.Scale / 2, position.Value.Y - drawInfo.Size.Y * drawInfo.Scale / 2),
            null,
            drawInfo.Color,
            drawInfo.Rotation,
            Vector2.Zero,
            drawInfo.Scale,
            SpriteEffects.None,
            drawInfo.ZIndex ?? 0f);
    }

    protected override void PostUpdate(GameTime state) => _spriteBatch.End();
}
