﻿using FlightDeck.Core;
using System.Collections.Generic;

namespace FlightDeck.Logics
{
    public interface IExpression
    {

    }

    public interface IEvaluator
    {
        (IEnumerable<TOGGLE_VALUE>, IExpression) Parse(string feedbackValue);
        bool Evaluate(Dictionary<TOGGLE_VALUE, double> values, IExpression expression);
    }
}
