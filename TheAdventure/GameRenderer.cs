using Silk.NET.Maths;
using Silk.NET.SDL;

namespace TheAdventure;

public readonly record struct Color(byte R, byte G, byte B);

public sealed unsafe class GameRenderer : IDisposable
{
    private readonly Sdl _sdl;
    private readonly Renderer* _renderer;
    private bool _disposed;

    public GameRenderer(Sdl sdl, Window* window)
    {
        _sdl = sdl;
        _renderer = sdl.CreateRenderer(window, -1, (uint)RendererFlags.Accelerated);
        if (_renderer == null)
        {
            throw sdl.GetErrorAsException() ?? new Exception("Failed to create renderer.");
        }

        _sdl.RenderSetVSync(_renderer, 1);
    }

    public void Clear(Color color)
    {
        _sdl.SetRenderDrawColor(_renderer, color.R, color.G, color.B, 255);
        _sdl.RenderClear(_renderer);
    }

    public void FillRect(float x, float y, float w, float h, Color color)
    {
        _sdl.SetRenderDrawColor(_renderer, color.R, color.G, color.B, 255);
        var rect = new Rectangle<int>(new Vector2D<int>((int)x, (int)y), new Vector2D<int>((int)w, (int)h));
        _sdl.RenderFillRect(_renderer, &rect);
    }

    public void Present()
    {
        _sdl.RenderPresent(_renderer);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _sdl.DestroyRenderer(_renderer);
        _disposed = true;
    }
}
