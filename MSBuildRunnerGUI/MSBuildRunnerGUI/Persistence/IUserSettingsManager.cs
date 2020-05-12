namespace MSBuildRunnerGUI.Persistence
{
    public interface IUserSettingsManager
    {
        string StorageFile { get; }

        UserSettings LoadFromLocation();
        void SaveSettings(UserSettings settings);


    }
}