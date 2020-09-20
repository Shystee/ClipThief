using System;

namespace ClipThief.Ui.Exceptions
{
    public class WrongPercentageDecimalException : Exception
    {
        public WrongPercentageDecimalException(string message)
            : base(message)
        {

        }
    }
}