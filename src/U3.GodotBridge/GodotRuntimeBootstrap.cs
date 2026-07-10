using UnityEngine;

namespace U3.GodotBridge;

public sealed class GodotRuntimeBootstrap
{
    private readonly IUnityLogger _logger;
    private bool _initialized;
    private BridgeTestBehaviour? _behaviour;

    public GameObject? HostObject { get; private set; }

    public GodotRuntimeBootstrap()
        : this(new GodotUnityLogger())
    {
    }

    public GodotRuntimeBootstrap(IUnityLogger logger)
    {
        _logger = logger;
    }

    public void Initialize()
    {
        if (_initialized)
        {
            return;
        }

        Debug.SetLogger(_logger);
        RuntimeLoop.Reset();

        HostObject = new GameObject("GodotHost");
        _behaviour = HostObject.AddComponent<BridgeTestBehaviour>();
        _initialized = true;

        Debug.Log("GodotRuntimeBootstrap initialized");
    }

    public void Tick(float deltaTime)
    {
        Initialize();
        RuntimeLoop.Tick(deltaTime);
    }

    public void TickFixed(float deltaTime)
    {
        Initialize();
        RuntimeLoop.TickFixed(deltaTime);
    }

    public void Shutdown()
    {
        if (!_initialized)
        {
            return;
        }

        RuntimeLoop.Reset();
        Debug.ResetLogger();
        Physics.ResetBackend();
        Input.ResetBackend();
        HostObject = null;
        _behaviour = null;
        _initialized = false;
    }

    private sealed class BridgeTestBehaviour : MonoBehaviour
    {
        private int _updateLogCount;

        public override void Awake()
        {
            Debug.Log("BridgeTestBehaviour.Awake");
        }

        public override void Start()
        {
            Debug.Log("BridgeTestBehaviour.Start");
        }

        public override void Update()
        {
            if (_updateLogCount < 5)
            {
                Debug.Log($"BridgeTestBehaviour.Update frame={Time.frameCount} delta={Time.deltaTime:0.000}");
                _updateLogCount++;
            }
        }

        public override void FixedUpdate()
        {
            Debug.Log($"BridgeTestBehaviour.FixedUpdate fixedDelta={Time.fixedDeltaTime:0.000}");
        }
    }
}
