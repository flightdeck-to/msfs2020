﻿using FlightDeck.Core;
using Microsoft.Extensions.Logging;
using SharpDeck;
using SharpDeck.Events.Received;
using SharpDeck.Manifest;
using System.Threading.Tasks;

namespace FlightDeck.Logics.Actions
{
    [StreamDeckAction("to.flightdeck.msfs2020.artificial.horizon")]
    public class HorizonAction : BaseAction
    {
        private readonly ILogger<HorizonAction> logger;
        private readonly IFlightConnector flightConnector;
        private readonly IImageLogic imageLogic;

        private TOGGLE_VALUE bankValue = TOGGLE_VALUE.PLANE_BANK_DEGREES;
        private TOGGLE_VALUE pitchValue = TOGGLE_VALUE.PLANE_PITCH_DEGREES;
        private TOGGLE_VALUE headingValue = TOGGLE_VALUE.PLANE_HEADING_DEGREES_MAGNETIC;

        private double currentHeadingValue = 0;
        private double currentBankValue = 0;
        private double currentPitchValue = 0;

        public HorizonAction(ILogger<HorizonAction> logger, IFlightConnector flightConnector, IImageLogic imageLogic)
        {
            this.logger = logger;
            this.flightConnector = flightConnector;
            this.imageLogic = imageLogic;
        }

        protected override async Task OnWillAppear(ActionEventArgs<AppearancePayload> args)
        {
            flightConnector.GenericValuesUpdated += FlightConnector_GenericValuesUpdated;

            RegisterValues();

            await UpdateImage();
        }

        private async void FlightConnector_GenericValuesUpdated(object sender, ToggleValueUpdatedEventArgs e)
        {
            bool isUpdated = false;

            if (e.GenericValueStatus.ContainsKey((bankValue, null)) && currentBankValue != e.GenericValueStatus[(bankValue, null)])
            {
                currentBankValue = e.GenericValueStatus[(bankValue, null)];
                isUpdated = true;
            }
            if (e.GenericValueStatus.ContainsKey((headingValue, null)) && currentHeadingValue != e.GenericValueStatus[(headingValue, null)])
            {
                currentHeadingValue = e.GenericValueStatus[(headingValue, null)];
                isUpdated = true;
            }
            if (e.GenericValueStatus.ContainsKey((pitchValue, null)) && currentPitchValue != e.GenericValueStatus[(pitchValue, null)])
            {
                currentPitchValue = e.GenericValueStatus[(pitchValue, null)];
                isUpdated = true;
            }

            if (isUpdated)
            {
                await UpdateImage();
            }
        }

        protected override Task OnWillDisappear(ActionEventArgs<AppearancePayload> args)
        {
            flightConnector.GenericValuesUpdated -= FlightConnector_GenericValuesUpdated;
            DeRegisterValues();
            return Task.CompletedTask;
        }

        private void RegisterValues()
        {
            flightConnector.RegisterSimValues((bankValue, null), (pitchValue, null));
        }

        private void DeRegisterValues()
        {
            flightConnector.DeRegisterSimValues((bankValue, null), (pitchValue, null));
        }

        protected override Task OnKeyDown(ActionEventArgs<KeyPayload> args)
        {
            bankValue = 0;
            _ = UpdateImage();
            return Task.CompletedTask;
        }

        private async Task UpdateImage()
        {
            await SetImageSafeAsync(imageLogic.GetHorizonImage(currentPitchValue, currentBankValue, currentHeadingValue));
        }
    }
}
