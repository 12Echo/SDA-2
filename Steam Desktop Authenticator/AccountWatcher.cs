using System;
using System.Threading;
using System.Threading.Tasks;
using SteamAuth;
using SteamKit2;
using SteamKit2.Internal;

namespace Steam_Desktop_Authenticator
{
    class AccountWatcher : IDisposable
    {
        public SteamGuardAccount Account { get; }
        public string Status { get; private set; } = "Connecting";
        public bool Connected { get; private set; }
        public bool Failed { get; private set; }

        public event Action<AccountWatcher> ConfirmationsChanged;
        public event Action<AccountWatcher> StatusChanged;

        private readonly SteamClient client;
        private readonly CallbackManager manager;
        private readonly SteamUser user;
        private readonly SynchronizationContext ui;
        private readonly CancellationTokenSource stop = new CancellationTokenSource();
        private DateTime reconnectAt = DateTime.MaxValue;
        private int reconnectDelay = 5;

        public AccountWatcher(SteamGuardAccount account)
        {
            Account = account;
            ui = SynchronizationContext.Current;

            client = new SteamClient();
            manager = new CallbackManager(client);
            user = client.GetHandler<SteamUser>();

            manager.Subscribe<SteamClient.ConnectedCallback>(OnConnected);
            manager.Subscribe<SteamClient.DisconnectedCallback>(OnDisconnected);
            manager.Subscribe<SteamUser.LoggedOnCallback>(OnLoggedOn);
            manager.Subscribe<SteamUser.LoggedOffCallback>(OnLoggedOff);
            manager.Subscribe<SteamUnifiedMessages.ServiceMethodNotification>(OnNotification);
        }

        public void Start()
        {
            Task.Run(() =>
            {
                client.Connect();
                while (!stop.IsCancellationRequested)
                {
                    manager.RunWaitCallbacks(TimeSpan.FromSeconds(1));
                    if (DateTime.UtcNow >= reconnectAt)
                    {
                        reconnectAt = DateTime.MaxValue;
                        client.Connect();
                    }
                }
            });
        }

        public void Dispose()
        {
            stop.Cancel();
            client.Disconnect();
        }

        private void OnConnected(SteamClient.ConnectedCallback callback)
        {
            SetStatus("Logging in");
            user.LogOn(new SteamUser.LogOnDetails
            {
                Username = Account.AccountName,
                AccessToken = Account.Session.RefreshToken,
                ClientOSType = EOSType.Android9,
                UIMode = EUIMode.Mobile,
            });
        }

        private void OnDisconnected(SteamClient.DisconnectedCallback callback)
        {
            Connected = false;
            if (stop.IsCancellationRequested || Failed) return;

            SetStatus("Reconnecting");
            reconnectAt = DateTime.UtcNow.AddSeconds(reconnectDelay);
            reconnectDelay = Math.Min(reconnectDelay * 2, 300);
        }

        private void OnLoggedOn(SteamUser.LoggedOnCallback callback)
        {
            if (callback.Result != EResult.OK)
            {
                // Anything that rejects the token itself will keep rejecting it
                Failed = callback.Result == EResult.InvalidPassword
                    || callback.Result == EResult.AccessDenied
                    || callback.Result == EResult.Expired
                    || callback.Result == EResult.Revoked
                    || callback.Result == EResult.AccountLoginDeniedNeedTwoFactor
                    || callback.Result == EResult.AccountLogonDenied;
                SetStatus("login failed (" + callback.Result + ")");
                return;
            }

            Connected = true;
            reconnectDelay = 5;
            SetStatus("connected");
            Raise(ConfirmationsChanged);
        }

        private void OnLoggedOff(SteamUser.LoggedOffCallback callback)
        {
            Connected = false;
            SetStatus("logged off (" + callback.Result + ")");
        }

        private void OnNotification(SteamUnifiedMessages.ServiceMethodNotification callback)
        {
            if (!(callback.Body is CSteamNotification_NotificationsReceived_Notification received))
                return;

            foreach (var notification in received.notifications)
            {
                if (notification.notification_type == ESteamNotificationType.k_ESteamNotificationType_MobileConfirmation ||
                    notification.notification_type == ESteamNotificationType.k_ESteamNotificationType_TradeOffer)
                {
                    Raise(ConfirmationsChanged);
                    return;
                }
            }
        }

        private void SetStatus(string status)
        {
            Status = status;
            Raise(StatusChanged);
        }

        private void Raise(Action<AccountWatcher> handler)
        {
            if (handler == null || stop.IsCancellationRequested) return;
            if (ui != null)
                ui.Post(state => handler(this), null);
            else
                handler(this);
        }
    }
}
