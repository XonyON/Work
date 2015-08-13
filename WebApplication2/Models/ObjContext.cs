using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class ObjContext : DbContext
    {
        public ObjContext()
            : base("DBase")
        {

        }

        public DbSet<ToDoItem> Items { get; set; }
        public DbSet<AddTask> AddTasks { get; set; }
        public DbSet<User> Users { get; set; }
    }
}