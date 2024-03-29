﻿using SharpDeck;
using SharpDeck.Events.Received;
using SharpDeck.Manifest;
using System.Threading.Tasks;

namespace FlightDeck.Logics.Actions
{
    #region

    [StreamDeckAction("to.flightdeck.msfs2020.number.0")]
    public class Number0Action : NumberAction { public Number0Action(IImageLogic imageLogic) : base(imageLogic) { } }
    [StreamDeckAction("to.flightdeck.msfs2020.number.1")]
    public class Number1Action : NumberAction { public Number1Action(IImageLogic imageLogic) : base(imageLogic) { } }
    [StreamDeckAction("to.flightdeck.msfs2020.number.2")]
    public class Number2Action : NumberAction { public Number2Action(IImageLogic imageLogic) : base(imageLogic) { } }
    [StreamDeckAction("to.flightdeck.msfs2020.number.3")]
    public class Number3Action : NumberAction { public Number3Action(IImageLogic imageLogic) : base(imageLogic) { } }
    [StreamDeckAction("to.flightdeck.msfs2020.number.4")]
    public class Number4Action : NumberAction { public Number4Action(IImageLogic imageLogic) : base(imageLogic) { } }
    [StreamDeckAction("to.flightdeck.msfs2020.number.5")]
    public class Number5Action : NumberAction { public Number5Action(IImageLogic imageLogic) : base(imageLogic) { } }
    [StreamDeckAction("to.flightdeck.msfs2020.number.6")]
    public class Number6Action : NumberAction { public Number6Action(IImageLogic imageLogic) : base(imageLogic) { } }
    [StreamDeckAction("to.flightdeck.msfs2020.number.7")]
    public class Number7Action : NumberAction { public Number7Action(IImageLogic imageLogic) : base(imageLogic) { } }
    [StreamDeckAction("to.flightdeck.msfs2020.number.8")]
    public class Number8Action : NumberAction
    {
        public Number8Action(IImageLogic imageLogic) : base(imageLogic) { }

        protected override async Task OnWillAppear(ActionEventArgs<AppearancePayload> args)
        {
            if (DeckLogic.NumpadParams?.Type == "XPDR")
            {
                await SetImageSafeAsync(null);
            }
            else
            {
                await base.OnWillAppear(args);
            }
        }
    }
    [StreamDeckAction("to.flightdeck.msfs2020.number.9")]
    public class Number9Action : NumberAction
    {
        public Number9Action(IImageLogic imageLogic) : base(imageLogic) { }

        protected override async Task OnWillAppear(ActionEventArgs<AppearancePayload> args)
        {
            if (DeckLogic.NumpadParams?.Type == "XPDR")
            {
                await SetImageSafeAsync(null);
            }
            else
            {
                await base.OnWillAppear(args);
            }
        }
    }

    #endregion

    public class NumberAction : BaseAction
    {
        private readonly IImageLogic imageLogic;

        public NumberAction(IImageLogic imageLogic)
        {
            this.imageLogic = imageLogic;
        }

        protected override async Task OnWillAppear(ActionEventArgs<AppearancePayload> args)
        {
            var tokens = args.Action.Split(".");
            var number = int.Parse(tokens[^1]);

            await SetImageSafeAsync(imageLogic.GetNumberImage(number));
        }

        protected override Task OnKeyDown(ActionEventArgs<KeyPayload> args)
        {
            if (DeckLogic.NumpadParams != null)
            {
                var tokens = args.Action.Split(".");
                var number = int.Parse(tokens[^1]);

                if (DeckLogic.NumpadParams.Type == "XPDR" && number > 7) return Task.CompletedTask;

                if (DeckLogic.NumpadParams.Value.Length < DeckLogic.NumpadParams.MinPattern.Length)
                {
                    DeckLogic.NumpadParams.Value += number.ToString();
                }
            }

            return Task.CompletedTask;
        }
    }
}
