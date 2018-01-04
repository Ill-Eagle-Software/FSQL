using System;
using System.Runtime.Serialization;

namespace FSQL.Exceptions {
    [Serializable]
    public class UndefinedAliasException : Exception
    {
        public UndefinedAliasException() { }
        public UndefinedAliasException(string message) : base(message) { }
        public UndefinedAliasException(string message, Exception inner) : base(message, inner) { }
        protected UndefinedAliasException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class TargetFolderDoesNotExistException : Exception {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public TargetFolderDoesNotExistException() {}
        public TargetFolderDoesNotExistException(string message) : base(message) {}
        public TargetFolderDoesNotExistException(string message, Exception inner) : base(message, inner) {}

        protected TargetFolderDoesNotExistException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) {}
    }


    [Serializable]
    public class MissingRequiredColumnException : Exception {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public MissingRequiredColumnException() {}
        public MissingRequiredColumnException(string message) : base(message) {}
        public MissingRequiredColumnException(string message, Exception inner) : base(message, inner) {}

        protected MissingRequiredColumnException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) {}
    }
}