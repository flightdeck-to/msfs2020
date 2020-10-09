﻿using System;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.Logging;
using SharpDeck;
using SharpDeck.Events.Received;
using SharpDeck.Manifest;

namespace FlightDeck.Logics.Actions
{
    #region Action Registration
    
    [StreamDeckAction("tech.flighttracker.streamdeck.heading.increase")]
    public class HeadingIncreaseAction : ValueChangeAction
    {
        public HeadingIncreaseAction(ILogger<HeadingIncreaseAction> logger, IFlightConnector flightConnector)
            : base(logger, flightConnector) { }
    }
    [StreamDeckAction("tech.flighttracker.streamdeck.heading.decrease")]
    public class HeadingDecreaseAction : ValueChangeAction
    {
        public HeadingDecreaseAction(ILogger<HeadingDecreaseAction> logger, IFlightConnector flightConnector)
            : base(logger, flightConnector) { }
    }
    [StreamDeckAction("tech.flighttracker.streamdeck.altitude.increase")]
    public class AltitudeIncreaseAction : ValueChangeAction
    {
        public AltitudeIncreaseAction(ILogger<AltitudeIncreaseAction> logger, IFlightConnector flightConnector)
            : base(logger, flightConnector) { }
    }
    [StreamDeckAction("tech.flighttracker.streamdeck.altitude.decrease")]
    public class AltitudeDecreaseAction : ValueChangeAction
    {
        public AltitudeDecreaseAction(ILogger<AltitudeDecreaseAction> logger, IFlightConnector flightConnector)
            : base(logger, flightConnector) { }
    }

    #endregion

    public abstract class ValueChangeAction : StreamDeckAction
    {
        private readonly ILogger logger;
        private readonly IFlightConnector flightConnector;

        private Timer timer;
        private string action;
        private bool timerHaveTick = false;
        private uint? originalValue = null;
        private AircraftStatus status;

        public ValueChangeAction(ILogger logger, IFlightConnector flightConnector)
        {
            this.logger = logger;
            this.flightConnector = flightConnector;
            timer = new Timer { Interval = 400 };
            timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timerHaveTick = true;
            Process(false);
        }

        private void Process(bool isUp)
        {
            if (string.IsNullOrEmpty(action) || status == null) return;
            if (isUp && timerHaveTick) return;

            var actions = action.Split('.');

            if (actions.Length < 2)
            {
                return;
            }
            
            var valueToChange = actions[^2];
            var change = actions[^1];
            var increment = change == "increase" ? 1 : -1;
            if (!isUp) increment *= 10;

            if (originalValue == null) originalValue = valueToChange switch
            {
                "heading" => (uint)status.ApHeading,
                "altitude" => (uint)status.ApAltitude,
                _ => throw new NotImplementedException($"Value type: {valueToChange}")
            };

            switch (valueToChange)
            {
                case "heading":
                    originalValue = (uint)(originalValue + 360 + increment) % 360;
                    flightConnector.ApHdgSet(originalValue.Value);
                    break;

                case "altitude":
                    originalValue = (uint)(originalValue + 100 * increment);
                    flightConnector.ApAltSet(originalValue.Value);
                    break;

            }
        }

        private void FlightConnector_AircraftStatusUpdated(object sender, AircraftStatusUpdatedEventArgs e)
        {
            status = e.AircraftStatus;
        }

        protected override Task OnKeyDown(ActionEventArgs<KeyPayload> args)
        {
            action = args.Action;
            timerHaveTick = false;
            timer.Start();
            return Task.CompletedTask;

        }

        protected override Task OnKeyUp(ActionEventArgs<KeyPayload> args)
        {
            timer.Stop();
            Process(true);
            action = null;
            originalValue = null;
            timerHaveTick = false;
            return Task.CompletedTask;
        }

        protected override Task OnWillAppear(ActionEventArgs<AppearancePayload> args)
        {
            status = null;
            this.flightConnector.AircraftStatusUpdated += FlightConnector_AircraftStatusUpdated;
            return Task.CompletedTask;
        }

        protected override Task OnWillDisappear(ActionEventArgs<AppearancePayload> args)
        {
            status = null;
            this.flightConnector.AircraftStatusUpdated -= FlightConnector_AircraftStatusUpdated;
            return Task.CompletedTask;
        }
    }
}
