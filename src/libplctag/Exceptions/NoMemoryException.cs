﻿using System;

namespace libplctag
{

    public class NoMemoryException : LibPlcTagException
    {
        public NoMemoryException()
        {
        }

        public NoMemoryException(string message)
            : base(message)
        {
        }

        public NoMemoryException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
