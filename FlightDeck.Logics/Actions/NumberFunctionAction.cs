﻿using SharpDeck;
using SharpDeck.Events.Received;
using SharpDeck.Manifest;
using System;
using System.Threading.Tasks;

namespace FlightDeck.Logics.Actions
{
    #region Action Registration

    [StreamDeckAction("to.flightdeck.msfs2020.number.enter")]
    public class NumberEnterAction : NumberFunctionAction { }
    [StreamDeckAction("to.flightdeck.msfs2020.number.backspace")]
    public class NumberBackspaceAction : NumberFunctionAction { }
    [StreamDeckAction("to.flightdeck.msfs2020.number.cancel")]
    public class NumberCancelAction : NumberFunctionAction { }
    [StreamDeckAction("to.flightdeck.msfs2020.number.transfer")]
    public class NumberTransferAction : NumberFunctionAction
    {
        protected override async Task OnWillAppear(ActionEventArgs<AppearancePayload> args)
        {
            if (DeckLogic.NumpadParams?.Type == "XPDR")
            {
                await SetTitleAsync("VFR");
            }
            else
            {
                await SetTitleAsync("Xfer");
            }
        }
    }

    #endregion

    public class NumberFunctionAction : StreamDeckAction
    {
        protected override async Task OnKeyDown(ActionEventArgs<KeyPayload> args)
        {
            if (DeckLogic.NumpadParams != null)
            {
                var param = new RegistrationParameters(Environment.GetCommandLineArgs()[1..]);
                switch (args.Action)
                {
                    case "to.flightdeck.msfs2020.number.enter":
                        if (DeckLogic.NumpadTcs != null)
                        {
                            DeckLogic.NumpadTcs.SetResult((DeckLogic.NumpadParams.Value, false));
                        }
                        await StreamDeck.SwitchToProfileAsync(param.PluginUUID, args.Device, null);
                        break;
                    case "to.flightdeck.msfs2020.number.cancel":
                        if (DeckLogic.NumpadTcs != null)
                        {
                            DeckLogic.NumpadTcs.SetResult((null, false));
                        }
                        await StreamDeck.SwitchToProfileAsync(param.PluginUUID, args.Device, null);
                        break;
                    case "to.flightdeck.msfs2020.number.transfer":
                        if (DeckLogic.NumpadParams.Type == "XPDR")
                        {
                            DeckLogic.NumpadParams.Value = "1200";
                            if (DeckLogic.NumpadTcs != null)
                            {
                                DeckLogic.NumpadTcs.SetResult((DeckLogic.NumpadParams.Value, false));
                            }
                        }
                        else
                        {
                            if (DeckLogic.NumpadTcs != null)
                            {
                                DeckLogic.NumpadTcs.SetResult((DeckLogic.NumpadParams.Value, true));
                            }
                        }
                        await StreamDeck.SwitchToProfileAsync(param.PluginUUID, args.Device, null);
                        break;
                    case "to.flightdeck.msfs2020.number.backspace":
                        if (DeckLogic.NumpadParams.Value.Length > 0)
                        {
                            DeckLogic.NumpadParams.Value = DeckLogic.NumpadParams.Value[..^1];
                        }
                        break;
                }
            }
        }
    }
}
