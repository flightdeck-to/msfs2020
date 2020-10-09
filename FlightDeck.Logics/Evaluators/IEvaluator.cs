﻿using System.Collections.Generic;
using FlightDeck.Core;

namespace FlightDeck.Logics.Evaluators
{
    public interface IExpression
    {

    }

    public interface IEvaluator
    {
        (IEnumerable<TOGGLE_VALUE>, IExpression) Parse(string feedbackValue);
        bool Evaluate(Dictionary<TOGGLE_VALUE, string> values, IExpression expression);
    }
}
