using System.Numerics;
using Alaz.Core;
using Alaz.Utils;
using Raylib_cs;

namespace PlatformerGame.Entities
{
    public class Player
    {
        private const float GRAVITY = 1200.0f;
        private const float JUMP_FORCE = -250.0f;
        private const float MOVE_SPEED = 250.0f;
        private const float GROUND_Y = 290.0f;

        public Vector2 Position;
        public Vector2 Velocity;
        public float Scale = 2.0f;

        private Texture2D _tex;
        private bool _isGrounded;

        private float _vAngle, _tAngle, _vTilt;
        private float _vScale = 1.0f;
        private float _walkTime, _walkBob, _walkTilt;

        public Shader OutlineShader { get; set; }

        public Player(Vector2 pos)
        {
            Position = pos;
            _tex = AssetManager.LoadTexture("player", "Assets/Images/miner_idle_0.png");

            OutlineShader = AssetManager.LoadShader("outline", null, "Assets/Shaders/outline.fs");

            int texSizeLoc = Raylib.GetShaderLocation(OutlineShader, "u_texture_size");
            Raylib.SetShaderValue(OutlineShader, texSizeLoc, new Vector2(_tex.Width, _tex.Height), ShaderUniformDataType.Vec2);

            int colorLoc = Raylib.GetShaderLocation(OutlineShader, "u_outline_color");
            Raylib.SetShaderValue(OutlineShader, colorLoc, new Vector3(1 / 255, 255 / 255, 255 / 255), ShaderUniformDataType.Vec3);
        }

        public void Update(float dt)
        {
            float dir = 0;
            if (Raylib.IsKeyDown(KeyboardKey.D)) dir = 1;
            if (Raylib.IsKeyDown(KeyboardKey.A)) dir = -1;

            // Horizontal Movement
            if (dir != 0)
            {
                Velocity.X = dir * MOVE_SPEED;
                _tAngle = (dir > 0) ? 0 : 180;
                _walkTime += dt * 14.0f;
            }
            else
            {
                Velocity.X = 0;
                _walkTime = Lerp(_walkTime, 0, 5 * dt);
            }

            // Gravity & Position
            Velocity.Y += GRAVITY * dt;
            Position += Velocity * dt;

            // Floor Collision
            if (Position.Y >= GROUND_Y)
            {
                Position.Y = GROUND_Y;
                if (!_isGrounded && Velocity.Y > 100)
                {
                    _vScale = 0.7f;
                    CameraShake.Shake(0.25f, 2.0f);
                }
                _isGrounded = true;
                Velocity.Y = 0;

                if (Raylib.IsKeyPressed(KeyboardKey.Space))
                {
                    Velocity.Y = JUMP_FORCE;
                    _vScale = 1.4f;
                    _isGrounded = false;
                }
            }

            // Procedural Wobble Logic
            if (_isGrounded && Math.Abs(Velocity.X) > 1)
            {
                _walkBob = MathF.Sin(_walkTime) * 0.15f;
                _walkTilt = Lerp(_walkTilt, dir * 12.0f, 10 * dt);
            }
            else
            {
                _walkBob = Lerp(_walkBob, 0, 10 * dt);
                _walkTilt = Lerp(_walkTilt, 0, 10 * dt);
            }

            // Visual Smoothing (Juice)
            float diff = (_tAngle - _vAngle + 180) % 360 - 180;
            _vAngle += (diff < -180 ? diff + 360 : diff) * (10 * dt);

            _vTilt = !_isGrounded ? Lerp(_vTilt, Math.Clamp(Velocity.Y * 0.05f, -20, 20) * (_tAngle == 180 ? -1 : 1), 5 * dt)
                                   : Lerp(_vTilt, _walkTilt, 15 * dt);

            _vScale = Lerp(_vScale, 1.0f, 10 * dt);
        }

        public void Draw()
        {
            float rads = _vAngle * (MathF.PI / 180.0f);
            float flip = Math.Abs(MathF.Cos(rads));
            float sW = MathF.Cos(rads) < 0 ? -_tex.Width : _tex.Width;

            float dW = _tex.Width * Scale * (2.0f - _vScale + _walkBob) * flip;
            float dH = _tex.Height * Scale * (_vScale - _walkBob);

            Raylib.DrawTexturePro(_tex, new Rectangle(0, 0, sW, _tex.Height),
                new Rectangle(Position.X, Position.Y, dW, dH), new Vector2(dW / 2, dH), _vTilt, Color.White);

        }

        private float Lerp(float a, float b, float t) => a + (b - a) * t;
    }
}
