using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace Store.Extension;

public static class VectorExtensions
{
    public static Vector GetEyePosition(CCSPlayerController player)
    {
        if (player.PlayerPawn?.Value is not { } pawn)
        {
            throw new ArgumentException("Player pawn not valid.");
        }

        Vector absOrigin = pawn.AbsOrigin!;

        return new Vector(absOrigin.X, absOrigin.Y, absOrigin.Z + pawn.ViewOffset.Z);
    }

    public static float CalculateDistance(Vector vector1, Vector vector2)
    {
        float dx = vector2.X - vector1.X;
        float dy = vector2.Y - vector1.Y;
        float dz = vector2.Z - vector1.Z;

        return MathF.Sqrt((dx * dx) + (dy * dy) + (dz * dz));
    }

    public static void Copy(Vector source, Vector destination)
    {
        destination.X = source.X;
        destination.Y = source.Y;
        destination.Z = source.Z;
    }

    public static bool IsZero(Vector vector)
    {
        return vector.LengthSqr() == 0;
    }
}