using DefaultEcs.System;
using FNAInvaders.Components;
using Microsoft.Xna.Framework;

namespace FNAInvaders.Systems;

[With(typeof(PlayerTag))]
public partial class ApplyInputSystem : AEntitySetSystem<GameTime>
{
    [Update, UseBuffer]
    private void Update(GameTime gameTime, in GameWorldData gameWorldData, in InputData inputData, in SpeedData speedData, ref Position position)
    {
        var speed = speedData.Value * gameTime.ElapsedGameTime.TotalMilliseconds;

        if (inputData.Left)
        {
            position.Value.X -= (int)speed;
        }
        if (inputData.Right)
        {
            position.Value.X += (int)speed;
        }

        position.Value.X = Math.Clamp(position.Value.X, gameWorldData.Bounds.Left, gameWorldData.Bounds.Right);
    }
}
