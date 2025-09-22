namespace FileServerBase
{
    public delegate IDynamicCachedFile DelegateProvideDynamicCachedFile(string filePath, string relativePath, 
        bool isIndex, DynamicCachedFilesHost host);
}