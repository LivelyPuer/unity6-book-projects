public interface ISaveable
{
    string CaptureJson();
    void RestoreJson(string json);
}
