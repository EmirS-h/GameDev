using Raylib_cs;
using System.Numerics;

namespace Alaz.Core;

public abstract class Game
{
    public int WindowWidth { get; private set; }
    public int WindowHeight { get; private set; }
    public int GameWidth { get; private set; }
    public int GameHeight { get; private set; }
    public string Title { get; protected set; }
    public int TargetFPS { get; protected set; } = 60;
    protected float Scale { get; private set; }
    protected Vector2 Offset { get; private set; }
    protected Rectangle RenderDestination { get; private set; }

    private RenderTexture2D _target;

    public Game(int windowWidth, int windowHeight, int gameWidth, int gameHeight, string title)
    {
        WindowWidth = windowWidth;
        WindowHeight = windowHeight;
        GameWidth = gameWidth;
        GameHeight = gameHeight;
        Title = title;
    }

    public void Run()
    {
        Raylib.SetConfigFlags(ConfigFlags.BorderlessWindowMode);
        Raylib.SetTargetFPS(TargetFPS);
        Raylib.InitWindow(WindowWidth, WindowHeight, Title);

        _target = Raylib.LoadRenderTexture(GameWidth, GameHeight);
        Raylib.SetTextureFilter(_target.Texture, TextureFilter.Point);

        LoadContent();
        Initialize();

        CalculateViewport();

        while (!Raylib.WindowShouldClose())
        {
            float dt = Raylib.GetFrameTime();

            Update(dt);

            // Render to Texture
            Raylib.BeginTextureMode(_target);
            Draw();
            DrawUI();

            Raylib.EndTextureMode();

            // Draw Texture to Screen
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            RenderToScreen();


            Raylib.EndDrawing();
        }

        UnloadContent();
        Raylib.UnloadRenderTexture(_target);
        Raylib.CloseWindow();
    }

    private void CalculateViewport()
    {
        int screenW = Raylib.GetScreenWidth();
        int screenH = Raylib.GetScreenHeight();

        Scale = Math.Min((float)screenW / GameWidth, (float)screenH / GameHeight);

        float renderW = GameWidth * Scale;
        float renderH = GameHeight * Scale;

        Offset = new Vector2((screenW - renderW) * 0.5f, (screenH - renderH) * 0.5f);
        RenderDestination = new Rectangle(Offset.X, Offset.Y, renderW, renderH);
    }

    protected virtual void RenderToScreen()
    {
        Rectangle sourceRec = new Rectangle(0, 0, _target.Texture.Width, -_target.Texture.Height);

        Raylib.DrawTexturePro(
            _target.Texture,
            sourceRec,
            RenderDestination,
            Vector2.Zero,
            0.0f,
            Color.White
        );
    }

    protected Vector2 GetMousePositionWorld()
    {
        Vector2 mouse = Raylib.GetMousePosition();
        return new Vector2(
            (mouse.X - Offset.X) / Scale,
            (mouse.Y - Offset.Y) / Scale
        );
    }
    protected abstract void LoadContent();
    protected abstract void Initialize();
    protected abstract void Update(float dt);
    protected abstract void Draw();
    protected abstract void DrawUI();
    protected abstract void UnloadContent();
}