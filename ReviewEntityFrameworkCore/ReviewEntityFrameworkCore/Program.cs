using Microsoft.EntityFrameworkCore;
using ReviewEntityFrameworkCore.Database;

namespace ReviewEntityFrameworkCore
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var context = new AppDbContext();
            //var isCreated = context.Database.EnsureCreated();

            var author = context.Authors.FirstOrDefault(x => x.Id == 1);
            context.Remove(author!);
            Console.ReadLine();
            context.SaveChanges();
            Console.ReadLine();
        }
    }

    /*
        DeleteBehavior.Cascade: sinh Cascade ở db
     */
}
