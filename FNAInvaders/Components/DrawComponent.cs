using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FNAInvaders.Components;

public struct DrawComponent
{
    public Point Size;
    public Color Color;
    public float Rotation;
    public float Scale;
    public float? ZIndex;

    public Texture2D Texture;
}
