using System;
using System.Collections.Generic;

public class S3Object: IComparable  
{
    public string Key;
    public DateTime LastModified;
    public string ETag;
    public long Size;
    public string StorageClass;
    public readonly Dictionary<string, string> Tags;

    public S3Object()
    {
        Tags = new Dictionary<string, string>();
    }
    
    public int CompareTo(object o)
    {
        if (o != null && o is S3Object description)
        {
            return description.LastModified.CompareTo(LastModified);
        }
        return 1;
    }
}
