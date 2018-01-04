namespace FSQL.Interfaces {
    public interface IAlias {
        string Name { get; }
        string Location { get; }
        string FileSpec { get; }
        bool IsFolder { get; }
        bool IsFile { get; }
        bool Exists { get; }

        IFolderTable Open();
    }
}