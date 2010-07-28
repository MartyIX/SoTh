using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sokoban.Lib.Exceptions
{
    public class PluginLoadFailedException : System.Exception
    {
        // The default constructor needs to be defined
        // explicitly now since it would be gone otherwise.

        public PluginLoadFailedException()
        {
        }

        public PluginLoadFailedException(string message)
            : base(message)
        {
        }
    }

    public class SolverException : System.Exception
    {
        // The default constructor needs to be defined
        // explicitly now since it would be gone otherwise.

        public SolverException()
        {
        }

        public SolverException(string message)
            : base(message)
        {
        }
    }

    public class NotStandardSokobanVariantException : System.Exception
    {
        // The default constructor needs to be defined
        // explicitly now since it would be gone otherwise.

        public NotStandardSokobanVariantException()
        {
        }

        public NotStandardSokobanVariantException(string message)
            : base(message)
        {
        }
    }

    public class NoRoundIsOpenException : System.Exception
    {
        // The default constructor needs to be defined
        // explicitly now since it would be gone otherwise.

        public NoRoundIsOpenException()
        {
        }

        public NoRoundIsOpenException(string message)
            : base(message)
        {
        }
    }


    
    public class NotValidQuestException : System.Exception
    {
        // The default constructor needs to be defined
        // explicitly now since it would be gone otherwise.

        public NotValidQuestException()
        {
        }

        public NotValidQuestException(string message)
            : base(message)
        {
        }
    }


    public class InvalidStateException : System.Exception
    {
        public InvalidStateException()
        {
        }

        public InvalidStateException(string message)
            : base(message)
        {
        }
    }

    public class UninitializedException : System.Exception
    {
        public UninitializedException()
        {
        }

        public UninitializedException(string message)
            : base(message)
        {
        }
    }

    public class NetworkTransferException : System.Exception
    {
        public NetworkTransferException()
        {
        }

        public NetworkTransferException(string message)
            : base(message)
        {
        }
    }

    public class InvalidDataFromNetworkException : System.Exception
    {
        public InvalidDataFromNetworkException()
        {
        }

        public InvalidDataFromNetworkException(string message)
            : base(message)
        {
        }
    }
    
}
