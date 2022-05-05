using Text = UnityEngine.UI.Text;

public class BCIDataListener : Singleton<BCIDataListener>
{
    public static EEGData CurrentData;

    private InputProxy proxy;
    private MessageHandler messageHandler;

    public float focusThreshold = 0.6f;
    public Text messageText, hostText;

    // Start is called before the first frame update
    void Start()
    {
        CurrentData = new EEGData {
            alpha = 0.0f,
            alphaBeta = 0.0f,
            alphaTheta = 0.0f,
            coherence = 0.0f,
            focus = 0.0f,
            thetaBeta = 0.0f,
            blink = 0.0f,
            o1 = 0.0f
        };

        proxy = FindObjectOfType<InputProxy>();
        messageHandler = GetComponent<MessageHandler>();

        var debug = UnityEngine.GameObject.Find("Debug Canvas").GetComponentsInChildren<Text>();
        messageText = debug[0];
        hostText = debug[1];
        SetHost(false);
    }

    public void Player1Update(int n)
    {
        //print("Player 1 update called. Value: " + n);
        proxy.SetP1(n > focusThreshold);
    }
    public void Player2Update(int n)
    {
        //print("Player 2 update called. Value: " + n);
        proxy.SetP2(n > focusThreshold);
    }
    public void Player3Update(int n)
    {
        //print("Player 3 update called. Value: " + n);
        proxy.SetP3(n > focusThreshold);
    }
    public void Player4Update(int n)
    {
        //print("Player 4 update called. Value: " + n);
        proxy.SetP4(n > focusThreshold);
    }

    public void Player1Message(string s)
    {
        ReceiveMessage(s);
    }
    public void Player2Message(string s)
    {
        ReceiveMessage(s);
    }
    public void Player3Message(string s)
    {
        ReceiveMessage(s);
    }
    public void Player4Message(string s)
    {
        ReceiveMessage(s);
    }

    public void ReceiveMessage(string s)
    {
        if (messageText) messageText.text = s;
        print("Message received in engine:\n" + s);

        messageHandler.HandleMessage(s);
    }

    public void SetHost(bool isHost)
    {
        hostText.text = isHost ? "is host" : "not host";
        MessageHandler.isHost = isHost;
        TurnManager.isHost = isHost;
    }
}