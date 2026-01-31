using System.Collections.Generic;
using System.Numerics;
using Alaz.Core;
using Alaz.Utils;
using PlatformerGame.Entities;
using Raylib_cs;

namespace PlatformerGame
{
    internal class Platformer() : Game(1280, 720, 640, 360, "Platformer Game")
    {
        private Player _player;
        private Texture2D _playerTex;

        private Camera2D _camera;
        Rectangle[] worldProps = new Rectangle[60];
        Color[] propColors = new Color[60];

        private Shader _crtShader;

        protected override void LoadContent()
        {
            _playerTex = Raylib.LoadTexture("Assets/Images/mage.png");
            _crtShader = Raylib.LoadShader(null, "Assets/Shaders/crt.fs");

            // Set One - Time Uniforms
            int texSizeLoc = Raylib.GetShaderLocation(_crtShader, "u_texture_size");
            Raylib.SetShaderValue(_crtShader, texSizeLoc, new Vector2(GameWidth, GameHeight), ShaderUniformDataType.Vec2);
        }

        protected override void Initialize()
        {
            _player = new Player(new Vector2(50, 50), _playerTex);

            _camera = new Camera2D { Offset = new Vector2(GameWidth / 2, GameHeight / 2), Zoom = 1.0f };

            int propCount = 60;
            worldProps = new Rectangle[propCount];
            propColors = new Color[propCount];
            Random rnd = new Random();

            for (int i = 0; i < propCount; i++)
            {
                worldProps[i] = new Rectangle(rnd.Next(-1000, 2000), rnd.Next(150, 270), rnd.Next(20, 60), rnd.Next(20, 60));
                propColors[i] = new Color(rnd.Next(100, 255), rnd.Next(100, 255), rnd.Next(100, 255), 255);
            }
        }

        protected override void Update(float dt)
        {
            Vector2 shakeOffset = CameraShake.GetOffset(dt);
            _player.Update(dt);
            _camera.Target = Vector2.Lerp(_camera.Target, _player.Position, 0.1f);
            _camera.Offset = new Vector2(GameWidth / 2.0f, GameHeight / 2.0f) + shakeOffset;
        }
        protected override void Draw()
        {
            Raylib.ClearBackground(new Color(20, 20, 20, 255));

            Raylib.BeginMode2D(_camera);

            _player.Draw();

            for (int i = 0; i < 60; i++)
                Raylib.DrawRectangleRec(worldProps[i], propColors[i]);

            Raylib.EndMode2D();
        }

        protected override void RenderToScreen()
        {
            // 1. Update "Time" uniform so scanlines move
            float time = (float)Raylib.GetTime();
            int timeLoc = Raylib.GetShaderLocation(_crtShader, "u_time");
            Raylib.SetShaderValue(_crtShader, timeLoc, time, ShaderUniformDataType.Float);

            // 2. Start Shader
            Raylib.BeginShaderMode(_crtShader);

            // 3. Call the base engine to draw the actual game texture
            base.RenderToScreen();

            // 4. End Shader
            Raylib.EndShaderMode();
        }

        protected override void DrawUI()
        {
        }

        protected override void UnloadContent()
        {
            Raylib.UnloadTexture(_playerTex);
            Raylib.UnloadShader(_crtShader);
        }

    }
}
