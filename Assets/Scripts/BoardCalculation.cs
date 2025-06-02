using System;
using System.Collections.Generic;
using UnityEngine;

public static class BoardCalculation
{
    public static int CalculateBoardValue(Queue<char> cards)
    {
        EquationParser(cards, out List<string> terms, out List<char> operators);
        int result = 0;
        return result;
    }

    private static void EquationParser(Queue<char> cards, out List<string> terms, out List<char> operators)
    {
        terms = new List<string>();
        string term = "";

        operators = new List<char>();

        foreach (var card in cards)
        {
            if (IsOperator(card))
            {
                terms.Add(term);
                term = "";
                operators.Add(card);
            }
            else
            {
                term += card;
            }
        }

        terms.Add(term);
    }

    public static bool IsOperator(char c)
    {
        return c == '+' || c == '-' || c == '*' || c == '/';
    }
}
