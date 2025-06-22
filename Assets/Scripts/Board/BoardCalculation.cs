using System;
using System.Collections.Generic;
using UnityEngine;

public static class BoardCalculation
{
    public static bool CalculateBoardValue(Queue<char> cards, out int result)
    {
        EquationParser(cards, out List<string> terms, out List<char> operators);
        List<int> values = new List<int>();

        Debug.Log(terms.Count);
        foreach (var term in terms)
        {
            int value = Convert.ToInt32(term, 16);
            values.Add(value);
        }

        result = values[0];
        if(operators.Count > 0)
        {
            for (int i = 0; i < operators.Count; i++)
            {
                switch (operators[i])
                {
                    case '+':
                        result += values[i + 1];
                        break;
                    case '*':
                        result *= values[i + 1];
                        break;
                }
            }
        }

        Debug.Log(result);
        return true;
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
