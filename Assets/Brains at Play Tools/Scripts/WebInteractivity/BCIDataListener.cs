public class BCIDataListener : Singleton<BCIDataListener>
{
    public static EEGData CurrentData;

    private InputProxy proxy;


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
    }

    public void Player1Update(int n)
    {
        print("Player 1 update called. Value: " + n);
        proxy.SetP1(n > 0 ? true : false);
    }
    public void Player2Update(int n)
    {
        print("Player 2 update called. Value: " + n);
        proxy.SetP2(n > 0 ? true : false);
    }
    public void Player3Update(int n)
    {
        print("Player 3 update called. Value: " + n);
        proxy.SetP3(n > 0 ? true : false);
    }
    public void Player4Update(int n)
    {
        print("Player 4 update called. Value: " + n);
        proxy.SetP4(n > 0 ? true : false);
    }
}