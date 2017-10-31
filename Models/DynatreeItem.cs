using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace FTBAPISERVER.Models
{
    public class DynatreeItem
    {
    public string Name { get; set; }
        public DateTime CreationTimeUtc { get; set; }
        public long Length { get; set; }
        public string Ext { get; set; }
        public bool isFolder { get; set; }
        private string _actualshare  { get; set; }
    public string key { get; set; }
    public List<DynatreeItem> Folder;

    public DynatreeItem(FileSystemInfo fsi,string actualshare ,bool filesOK=true,string searchpattern="*.*")
    {
            _actualshare = actualshare;
        Name = fsi.Name;
        CreationTimeUtc = fsi.CreationTimeUtc;
        Folder = new List<DynatreeItem>();

        if ((fsi.Attributes & FileAttributes.Directory)  == FileAttributes.Directory)
        {
            isFolder = true;
            foreach (FileSystemInfo f in (fsi as DirectoryInfo).GetFileSystemInfos(searchpattern))
            {


                    if ((f.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        Folder.Add(new DynatreeItem(f,_actualshare));
                    }
                    else if (filesOK)
                    {
                        FileInfo bfile = new FileInfo(f.FullName);  
                        var tempinfo = new DynatreeItem(f,_actualshare);
                        tempinfo.Ext = f.Extension;
                        //tempinfo.key = f.FullName.Replace(actualshare, actualshare); ;
                        tempinfo.Length = bfile.Length;
                        Folder.Add(tempinfo);// new DynatreeItem(f));
                    }
                        
            }
        }
        else
        {
            isFolder = false;
        }

            key = fsi.FullName.Substring(fsi.FullName.IndexOf(actualshare)+actualshare.Length+1 ) ;//.Replace(@actualshare,friendlyshare); // ;;.Directory;// Name.Replace(" ", "").ToLower();
    }

    public string JsonToDynatree()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}
}