using System;

namespace rpsls.Domain
{
    public class MatchResult
    {
        public Guid Id { get; set; }

        public PlayerResult PlayerOne { get; set; }

        public PlayerResult PlayerTwo { get; set; }
    }
}