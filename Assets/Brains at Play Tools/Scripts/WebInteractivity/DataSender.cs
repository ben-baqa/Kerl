// Sends data to JS backend
public class DataSender : Singleton<DataSender>
{
    // This has to be formatted for the data to be sent.
    public void SendToJS()
    {
        JSplugin.SendFloatToServer(4.56f);
        JSplugin.SendStringToServer("this came from unity");
    }
}
