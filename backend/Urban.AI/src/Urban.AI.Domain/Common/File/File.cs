namespace Urban.AI.Domain.Common.File;

public sealed class File
{
    public string Filename { get; private set; }
    public string Content { get; private set; }
    public string Path { get; private set; }
    public string NameBucket { get; private set; }
    public string Mimetype { get; private set; }

    private File(string filename, string content, string path, string nameBucket, string mimetype)
    {
        Filename = filename;
        Content = content;
        Path = path;
        NameBucket = nameBucket;
        Mimetype = mimetype;
    }

    public File SaveFile(string filename, string content, string path, string nameBucket, string mimetype)
    {
        return new File(filename, content, path, nameBucket, mimetype);
    }

    public File GetFile(string filename, string path, string nameBucket)
    {
        return new File(filename, null!, path, nameBucket, null!);
    }

    public File DeleteFile(string path, string nameBucket)
    {
        return new File(null!, null!, path, nameBucket, null!);
    }

    public static File CreateForSave(string filename, string content, string path, string nameBucket, string mimetype)
    {
        return new File(filename, content, path, nameBucket, mimetype);
    }

    public static File CreateForGet(string filename, string path, string nameBucket)
    {
        return new File(filename, null!, path, nameBucket, null!);
    }

    public static File CreateForDelete(string path, string nameBucket)
    {
        return new File(null!, null!, path, nameBucket, null!);
    }
}
