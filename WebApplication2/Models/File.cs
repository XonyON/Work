//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication2.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class File
    {
        public File() { }
        public File(int id, byte[] data, string nam, string typ, DateTime time)
        {
            itemid = id;
            file1 = data;
            this.name = nam;
            this.type = typ;
            this.time = time;
        }

        public int Id { get; set; }
        public byte[] file1 { get; set; }
        public System.DateTime time { get; set; }
        public int itemid { get; set; }
        public string type { get; set; }
        public string name { get; set; }
    }
}
