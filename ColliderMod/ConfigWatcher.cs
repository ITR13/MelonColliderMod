using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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


            MelonModLogger.Log($"Updating ColliderMod configs at \"{FullPath}\"");

            var oldJson = "";

            if (!File.Exists(FullPath))
            {
                MelonModLogger.Log(
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
                MelonModLogger.LogError(e.ToString());
                MelonModLogger.Log(
                    "Something went wrong when deserializing json. " +
                    "Delete the config to reset everything to default"
                );
                return true;
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
