using System;
using Ludo.Core.Events;

namespace Ludo.Localization
{
    public class LanguageChangedEvent : IEvent
    {
        public readonly string Code;
        public LanguageChangedEvent(string c) => Code = c;
    }
}