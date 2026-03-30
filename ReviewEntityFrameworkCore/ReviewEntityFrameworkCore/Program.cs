using Microsoft.EntityFrameworkCore;
using ReviewEntityFrameworkCore.Database;

namespace ReviewEntityFrameworkCore
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var context = new AppDbContext();
            //  var isCreated = context.Database.EnsureCreated();

            var author = context.Authors.FirstOrDefault(x => x.Id == 1);

            var articles = context.Articles.Where(x => x.AuthorId == 1).ToList();
            // foreach (var item in articles)
            // {
            //     item.AuthorId = null;
            // }

            System.Console.WriteLine("Chuẩn bị xóa Author");
            Console.ReadLine();
            // context.RemoveRange(articles);
            context.Remove(author!);
            Console.ReadLine();
            context.SaveChanges();
            Console.ReadLine();

        }
    }

    /*
        DeleteBehavior.Cascade: sinh Cascade ở db
        
        DeleteBehavior.Restrict: sinh ra ở dưới db ràng buộc foreign key No Action. Khi xóa cha thì EF sẽ kiểm tra xem các data con đang được tracked có còn
            tham chiếu đến primary key của data cha hay không ? Sẽ có 3 trường hợp xảy ra như sau:
                + TH1: Nếu còn ít nhất một data con còn tham chiếu đến primary key của data cha và khóa ngoại của data con không được phép null thì sẽ 
                        quăng exception ngay trên application (không thực hiện gọi lệnh dưới db, tức là ngay khi gọi dbContext.Remove()) => phải gọi lệnh xóa data con trước rồi sau đó gọi lệnh xóa cha
                + TH2: Nếu còn ít nhất một data con còn tham chiếu đến primary key của data cha và FOREIGN KEY của data con được phép đặt thành NULL
                        => khi gọi lệnh xóa data cha thì EF sẽ tự động đặt FOREIGN KEY của data con thành null rồi sau đó gọi lệnh xóa data cha
                        => Tức là sẽ có 2 lệnh được thực thi dưới db: đầu tiên là lệnh update FOREIGN KEY của data con thành null, thứ hai là lệnh xóa data cha
                +TH3: Nếu application chưa load bất kì data con nào hoặc không theo dõi bất kì data còn nào thì sao ? => EF sẽ coi như data cha không có data
                        con nào tham chiếu đến => sẽ gọi lệnh xóa như bth, còn việc có xóa được hay không thì sẽ phụ thuộc vào dữ liệu cũng như là ràng buộc 
                        thực tế dưới db
                + Lưu ý: Nếu DbContext đang không theo dõi bất kỳ data con nào thì sẽ khi gọi lệnh xóa data cha sẽ rơi vào TH3.
                
     */
}
