﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Uno.Extensions.Logging;

namespace Uno.Extensions.Configuration
{
    public class Reloader
    {
        private ILogger Logger { get; }

        private IConfigurationRoot Config { get; }

        public Reloader(ILogger<ReloadService> logger, IConfigurationRoot configRoot)
        {
            Logger = logger;
            Config = configRoot;
        }

        public async Task ReloadAllFileConfigurationProviders(string? configFile = default)
        {
            Logger.LogDebugMessage($"Reloading config");


            var fileProviders = Config.Providers;
            foreach (var fp in fileProviders)
            {
                Logger.LogDebugMessage($@"Config provider of type '{fp.GetType().Name}'");
                if (fp is FileConfigurationProvider fcp && (configFile is null || configFile.ToLower().Contains(fcp.Source.Path.Split('/','\\').Last().ToLower())))
                {
                    Logger.LogDebugMessage($@"Loading from file '{fcp.Source.Path}'");
                    var provider = fcp.Source.FileProvider;
                    var info = provider.GetFileInfo(fcp.Source.Path);
					if (!File.Exists(info.PhysicalPath))
					{
						Logger.LogDebugMessage($@"File doesn't exist '{info.PhysicalPath}'");
					}
					else { 
						var contents = File.ReadAllText(info.PhysicalPath);
						Logger.LogDebugMessage($@"Contents '{contents}'");
						Logger.LogDebugMessage($@"Loading from full path '{info.PhysicalPath}'");
						fp.Load();
					}
                }
            }
            Logger.LogDebugMessage($"Reloading configuration complete");
        }
    }
}
