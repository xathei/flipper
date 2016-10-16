using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FFACETools;

namespace Flipper
{
    public static class Chat
    {
        #region Fields
        private static FFACE _fface;
        private static List<string> _matches;
        private static Thread ChatThread;
        private static bool _activated;
        #endregion

        #region Events and Delegates
        public delegate void FoundMatch(string match);
        public static event FoundMatch FoundMatchHandler;
        #endregion

        #region Set Fields
        public static void SetInstance(FFACE fface)
        {
            _fface = fface;
        }

        public static void SetMatches(List<string> matches)
        {
            _matches = matches;
        }

        public static void Watch(bool activate = true)
        {
            if (activate)
            {
                _activated = true;
                ChatThread = new Thread(DoChat);
                ChatThread.Start();
            }
            else
            {
                _activated = false;
                ChatThread = null;
            }
        }
        #endregion


        private static void DoChat()
        {
            string chat = string.Empty;
            string prev = string.Empty;

            while (_activated)
            {
                try { chat = _fface.Chat.GetNextLine().Text; }
                catch { }

                if (!string.IsNullOrEmpty(chat) && chat != prev)
                {
                    // "chat"
                    prev = chat;

                    if (_matches.Any(x => chat.Contains(x)))
                    {
                        FoundMatchHandler?.Invoke(chat);
                    }
                }
                Thread.Sleep(1);
            }
        }
    }
}
