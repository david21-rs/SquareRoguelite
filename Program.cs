using System.Diagnostics;
using Silk.NET.SDL;

namespace TheAdventure;

public static class Program
{
    public static void Main()
    {
        var sdl = new Sdl(new SdlContext());

        if (sdl.Init(Sdl.InitVideo | Sdl.InitEvents | Sdl.InitTimer) < 0)
        {
            throw new InvalidOperationException("Failed to initialize SDL.");
        }

        IntPtr window;
        unsafe
        {
            window = (IntPtr)sdl.CreateWindow(
                "The Adventure", Sdl.WindowposUndefined, Sdl.WindowposUndefined,
                Game.WindowWidth, Game.WindowHeight,
                (uint)WindowFlags.Shown);

            if (window == IntPtr.Zero)
            {
                throw sdl.GetErrorAsException() ?? new Exception("Failed to create window.");
            }
        }

        var input = new Input();
        var game = new Game();

        using (var renderer = CreateRenderer(sdl, window))
        {
            var timer = Stopwatch.StartNew();
            var ev = new Event();
            var quit = false;

            while (!quit)
            {
                input.NewFrame();

                while (sdl.PollEvent(ref ev) != 0)
                {
                    switch ((EventType)ev.Type)
                    {
                        case EventType.Quit:
                            quit = true;
                            break;

                        case EventType.Keydown when ev.Key.Repeat == 0:
                            input.OnKeyDown((KeyCode)ev.Key.Keysym.Scancode);
                            break;

                        case EventType.Keyup:
                            input.OnKeyUp((KeyCode)ev.Key.Keysym.Scancode);
                            break;
                    }
                }

                var dt = (float)timer.Elapsed.TotalSeconds;
                timer.Restart();
                // avoid huge jumps when the window is dragged or the process stalls
                if (dt > 0.05f)
                {
                    dt = 0.05f;
                }

                game.Update(input, dt);
                game.Render(renderer);
            }
        }

        game.SaveHighScore();

        unsafe
        {
            sdl.DestroyWindow((Window*)window);
        }

        sdl.Quit();
    }

    private static GameRenderer CreateRenderer(Sdl sdl, IntPtr window)
    {
        unsafe
        {
            return new GameRenderer(sdl, (Window*)window);
        }
    }
}
