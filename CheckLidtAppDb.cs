using Microsoft.EntityFrameworkCore;

class CheckListAppDb : DbContext
{
    public CheckListAppDb(DbContextOptions<CheckListAppDb> options)
        : base(options) { }

    public DbSet<CheckListApp> CheckLists => Set<CheckListApp>();
}