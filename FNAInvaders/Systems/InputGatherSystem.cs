using DefaultEcs.System;
using FNAInvaders.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FNAInvaders.Systems;

[With(typeof(PlayerTag))]
public partial class InputGatherSystem : AEntitySetSystem<GameTime>
{
    [Update, UseBuffer]
    private void Update(ref InputData inputData)
    {
        var keyboardState = Keyboard.GetState();

        inputData.Left = keyboardState.IsKeyDown(Keys.Left);
        inputData.Right = keyboardState.IsKeyDown(Keys.Right);
        inputData.Fire = keyboardState.IsKeyDown(Keys.Space);
    }
}
