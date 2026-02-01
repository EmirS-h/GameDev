using Raylib_cs;

namespace Alaz.Core
{
    public static class AssetManager
    {
        private static readonly Dictionary<string, Texture2D> Textures = [];
        private static readonly Dictionary<string, Shader> Shaders = [];
        private static readonly Dictionary<string, Font> Fonts = [];
        private static readonly Dictionary<string, Sound> Sounds = [];

        public static Texture2D LoadTexture(string key, string path)
        {
            if (!Textures.TryGetValue(key, out Texture2D value))
            {
                if (!File.Exists(path))
                    throw new FileNotFoundException($"Texture file not found at: {path}");

                Texture2D tex = Raylib.LoadTexture(path);

                if (tex.Id == 0)
                    throw new Exception($"Raylib failed to load texture: {path}");

                Textures.Add(key, tex);
                return tex;
            }
            return value;
        }

        public static Texture2D GetTexture(string key)
        {
            if (Textures.TryGetValue(key, out Texture2D value))
            {
                return value;
            }
            throw new Exception($"Texture with key '{key}' not found.");
        }

        public static Shader LoadShader(string key, string? vsPath, string? fsPath)
        {
            if (!Shaders.TryGetValue(key, out Shader value))
            {
                Shader shader = Raylib.LoadShader(vsPath, fsPath);

                if (shader.Id == 0)
                    throw new Exception($"Raylib failed to load shader: {key}");

                Shaders.Add(key, shader);
                return shader;
            }
            return value;
        }

        public static Shader GetShader(string key)
        {
            if (Shaders.TryGetValue(key, out Shader value))
            {
                return value;
            }
            throw new Exception($"Shader with key '{key}' not found.");
        }

        public static void UnloadAll()
        {
            foreach (var tex in Textures.Values)
            {
                Raylib.UnloadTexture(tex);
            }
            Textures.Clear();
            foreach (var shader in Shaders.Values)
            {
                Raylib.UnloadShader(shader);
            }
            Shaders.Clear();
        }

        public static bool UnloadTexture(string key)
        {
            if (Textures.TryGetValue(key, out Texture2D value))
            {
                Raylib.UnloadTexture(value);
                return Textures.Remove(key);
            }
            return false;
        }

        public static bool UnloadShader(string key)
        {
            if (Shaders.TryGetValue(key, out Shader value))
            {
                Raylib.UnloadShader(value);
                return Shaders.Remove(key);
            }
            return false;
        }
    }
}
