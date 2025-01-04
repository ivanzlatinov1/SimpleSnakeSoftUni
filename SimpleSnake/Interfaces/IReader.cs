using System.Diagnostics.CodeAnalysis;
using SimpleSnake.Enums;

namespace SimpleSnake.Interfaces
{
    public interface IReader
    {
        bool TryReadDirection([NotNullWhen(true)] out Direction? direction);
        bool ReadConfirmation(char expectedKeyChar);
    }
}
