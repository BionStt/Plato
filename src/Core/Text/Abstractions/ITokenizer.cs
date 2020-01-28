﻿using System.Collections.Generic;

namespace PlatoCore.Text.Abstractions
{
    public interface ITokenizer<TToken> where TToken : class, IToken
    {

        IEnumerable<TToken> Tokenize(string input);

    }

    public class Token : IToken
    {

        public int Start { get; set; }

        public int End { get; set; }

        public string Value { get; set; }

    }

    public interface IToken
    {
        int Start { get; set; }

        int End { get; set; }

        string Value { get; set; }

    }

}
