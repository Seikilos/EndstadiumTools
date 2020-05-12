using System;
using System.IO;
using MSBuildRunnerGUI.Annotations;
using MSBuildRunnerGUI.Contracts;
using Newtonsoft.Json;

namespace MSBuildRunnerGUI.Persistence
{
    public class UserSettingsManager : IUserSettingsManager
    {
        private readonly IFileIO _fileIO;
        
        public UserSettingsManager([NotNull] IFileIO fileIO, [NotNull] string fileLocation)
        {
            _fileIO = fileIO ?? throw new ArgumentNullException(nameof(fileIO));

            StorageFile = fileLocation ?? throw new ArgumentNullException(nameof(fileLocation));
        }


        public string StorageFile { get; }

        public UserSettings LoadFromLocation()
        {
            _fileIO.CreateDirectory(Path.GetDirectoryName(StorageFile));

            if (_fileIO.Exists(StorageFile) == false)
            {
                SaveSettings(new UserSettings());
            }

            return JsonConvert.DeserializeObject<UserSettings>(_fileIO.ReadFile(StorageFile));
        }


        public void SaveSettings(UserSettings settings)
        {
            _fileIO.WriteAllText(StorageFile, JsonConvert.SerializeObject(settings, Formatting.Indented));
        }
    }
}