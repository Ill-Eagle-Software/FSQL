namespace FSQL.Interfaces {
    public interface IFolderTable : ITable<IFileInformation>, IAlias
    {
        bool Flattened { get; }
    }
}