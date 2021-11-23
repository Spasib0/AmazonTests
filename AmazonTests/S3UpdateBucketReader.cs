using System;
using System.Collections.Generic;
using System.Xml;

public class S3UpdateBucketReader
{
    public const string BUCKET_URL = "https://s3.eu-central-1.amazonaws.com/naura-builds/";
    public const string LIST_COMMAND = "https://s3.eu-central-1.amazonaws.com/naura-builds/?list-type=2";
    public const string TAGGING_COMAND = "?tagging";
    

    private static void GetObjectTags(S3Object s3Object)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(BUCKET_URL + s3Object.Key + TAGGING_COMAND);
        if (xmlDoc.HasChildNodes && xmlDoc.ChildNodes.Count > 1)
        {
            XmlNode keys = xmlDoc.ChildNodes[1].ChildNodes[0];
            for (int j = 0; j < keys.ChildNodes.Count; j++)
            {
                string key = "";
                string value = "";
                XmlNode root = keys.ChildNodes[j];
                for (int i = 0; i < root.ChildNodes.Count; i++)
                {
                    switch (root.ChildNodes[i].Name)
                    {
                        case "Key":
                            key = root.ChildNodes[i].InnerText;
                            break;
                        case "Value":
                            value = root.ChildNodes[i].InnerText;
                            break;
                    }
                }
                s3Object.Tags.Add(key, value);
            }
        }
    }

    public static List<S3Object> ListBucket()
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(LIST_COMMAND);
        List<S3Object> result = new List<S3Object>();
        if (xmlDoc.HasChildNodes && xmlDoc.ChildNodes.Count >1)
        {
            XmlNode root = xmlDoc.ChildNodes[1];
            for (int i=0; i < root.ChildNodes.Count; i++)
            {
                if(root.ChildNodes[i].Name != "Contents")
                    continue;
                XmlNode contents = root.ChildNodes[i];
                S3Object s3Object = new S3Object();
                result.Add(s3Object);
                for (int j = 0; j < contents.ChildNodes.Count; j++)
                {
                    switch (contents.ChildNodes[j].Name)
                    {
                        case "Key":
                            s3Object.Key = contents.ChildNodes[j].InnerText;
                            break;
                        case "LastModified":
                            s3Object.LastModified = DateTime.Parse(contents.ChildNodes[j].InnerText);
                            break;
                        case "ETag":
                            s3Object.ETag = contents.ChildNodes[j].InnerText;
                            break;
                        case "StorageClass":
                            s3Object.StorageClass = contents.ChildNodes[j].InnerText;
                            break;
                        case "Size":
                            s3Object.Size = Int64.Parse(contents.ChildNodes[j].InnerText);
                            break;
                    }
                }

                GetObjectTags(s3Object);
            }
        }
        return result;
    }
}
