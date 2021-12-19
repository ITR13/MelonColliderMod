using System;
using System.IO;
using MelonLoader;
using MelonLoader.TinyJSON;

namespace ColliderMod
{
    class ConfigWatcher
    {
        private const string FileName = "ColliderModConfig.json";

        private static readonly string FileDirectory = Path.Combine(
            Environment.CurrentDirectory,
            "UserData"
        );

        private static readonly string FullPath = Path.Combine(
            FileDirectory,
            FileName
        );

        public static ColliderModConfig ColliderModConfig = new ColliderModConfig();

        private static readonly FileSystemWatcher FileSystemWatcher;
        private static int _dirty = 0;

        static ConfigWatcher()
        {
            FileSystemWatcher = new FileSystemWatcher(FileDirectory, FileName)
            {
                NotifyFilter = (NotifyFilters)((1 << 9) - 1),
                EnableRaisingEvents = true
            };
            FileSystemWatcher.Changed += (_, __) => _dirty++;
            FileSystemWatcher.Created += (_, __) => _dirty++;
            FileSystemWatcher.Renamed += (_, __) => _dirty++;
            FileSystemWatcher.Deleted += (_, __) => _dirty++;
            _dirty++;
        }

        public static void Unload()
        {
            FileSystemWatcher.EnableRaisingEvents = false;
            _dirty = -1000;
        }

        public static bool UpdateIfDirty()
        {
            if (_dirty <= 0) return false;
            _dirty = 0;


            MainClass.Msg($"Updating ColliderMod configs at \"{FullPath}\"");

            var oldJson = "";

            if (!File.Exists(FullPath))
            {
                MainClass.Msg(
                    $"No config file found, using default"
                );
                ColliderModConfig = new ColliderModConfig();
            }
            else try
            {
                oldJson = File.ReadAllText(FullPath);
                JSON.MakeInto(JSON.Load(oldJson), out ColliderModConfig);
            }
            catch (Exception e)
            {
                ColliderModConfig = new ColliderModConfig();
                MainClass.Error(e.ToString());
                MainClass.Msg(
                    "Something went wrong when deserializing json. " +
                    "Delete the config to reset everything to default"
                );
                return true;
            }

            // Safety measure
            {
                // This could be a lot prettier, but I'm sleepy
                var createdColliderSize = new float[3];
                var createdColliderOffset = new float[3];
                var createdColliderColor = new float[4];
                var solidColliderColor = new float[4];
                var triggerColliderColor = new float[4];

                void CopyOver(float[] dst, float[] src)
                {
                    if(src ==null) return;
                    Array.Copy(src, dst, src.Length < dst.Length ? src.Length : dst.Length);
                }

                CopyOver(createdColliderSize, ColliderModConfig.createdColliderSize);
                CopyOver(createdColliderOffset, ColliderModConfig.createdColliderOffset);
                CopyOver(createdColliderColor, ColliderModConfig.createdColliderColor);
                CopyOver(solidColliderColor, ColliderModConfig.solidColliderColor);
                CopyOver(triggerColliderColor, ColliderModConfig.triggerColliderColor);

                ColliderModConfig.createdColliderSize = createdColliderSize;
                ColliderModConfig.createdColliderOffset = createdColliderOffset;
                ColliderModConfig.createdColliderColor = createdColliderColor;
                ColliderModConfig.solidColliderColor = solidColliderColor;
                ColliderModConfig.triggerColliderColor = triggerColliderColor;
            }

            var json = JSON.Dump(
                ColliderModConfig,
                EncodeOptions.PrettyPrint | EncodeOptions.NoTypeHints
            );

            if (oldJson == json) return true;

            File.WriteAllText(FullPath, json);
            _dirty--;

            return true;
        }
    }
}
