using System;
using System.Runtime.Serialization;

namespace FSQL.Exceptions {
    [Serializable]
    public class RedefinedAliasException : Exception {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public RedefinedAliasException() {}
        public RedefinedAliasException(string message) : base(message) {}
        public RedefinedAliasException(string message, Exception inner) : base(message, inner) {}

        protected RedefinedAliasException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) {}
    }
}