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

        public PluginLoadFailedException(string message) : base(message)
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

}
