using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFModel
{
    public class StudentDbContext:DbContext
    {
        public StudentDbContext()
            : base("name=ConnectionString")
        {

        }
        public DbSet<Student> Students { get; set; }
    }

    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
