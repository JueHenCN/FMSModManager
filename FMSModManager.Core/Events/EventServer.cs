using Prism.Events;

namespace FMSModManager.Core.Events
{
    public class ModsChangedEvent : PubSubEvent { }

    public class SteamworkServiceRefuseEvent : PubSubEvent { }

    public class LanguageChangedEvent : PubSubEvent<string> { }

    public class GameFolderChangedEvent : PubSubEvent<string> { }

} 