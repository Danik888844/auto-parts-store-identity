using AutoPartsIdentity.DataAccess.Models.DatabaseModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AutoPartsIdentity.DataAccess.Contexts;

public class IdentityDb : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    //public DbSet<User> Users {get;set;}
    
    public IdentityDb(){ }
    
    public IdentityDb(DbContextOptions<IdentityDb> options) : base(options) { }
}