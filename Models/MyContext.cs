using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Exam.Models {

    public class MyContext : DbContext {
        public MyContext (DbContextOptions options) : base (options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<Center> Centers { get; set; }

        public DbSet<OtherUser> OtherUsers { get; set; }
    }
}