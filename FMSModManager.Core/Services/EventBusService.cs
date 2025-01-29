using System;

namespace FMSModManager.Core.Services
{
    public class EventBusService
    {
        public event Action ModsChanged;
        
        public void NotifyModsChanged()
        {
            ModsChanged?.Invoke();
        }
    }
} 