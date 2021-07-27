﻿using SharpDeck;
using System;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace FlightDeck.Logics.Actions
{
    public abstract class BaseAction<TSettings> : StreamDeckAction<TSettings> where TSettings : class
    {
        public async Task SetImageSafeAsync(string base64Image)
        {
            try
            {
                await SetImageAsync(base64Image);
            }
            catch (SocketException)
            {
                // Ignore as we can't really do anything here
            }
            catch (WebSocketException)
            {
                // Ignore as we can't really do anything here
            }
            catch (ObjectDisposedException)
            {
                // Ignore
            }
        }
    }
    public abstract class BaseAction : StreamDeckAction
    {
        public async Task SetImageSafeAsync(string base64Image)
        {
            try
            {
                await SetImageAsync(base64Image);
            }
            catch (SocketException)
            {
                // Ignore as we can't really do anything here
            }
            catch (WebSocketException)
            {
                // Ignore as we can't really do anything here
            }
            catch (ObjectDisposedException)
            {
                // Ignore
            }
        }
    }
}
