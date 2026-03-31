using Microsoft.EntityFrameworkCore;
using ReviewEntityFrameworkCore.Database;

namespace ReviewEntityFrameworkCore
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var context = new AppDbContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var author = context.Authors.FirstOrDefault(x => x.Id == 1);
            var articles = context.Articles.Where(x => x.AuthorId == 1).ToList();

            context.Remove(author!);
            context.SaveChanges();
        }
    }

    /*
        DeleteBehavior.Cascade: sinh ra dưới db FOREIGN KEY CASCADE. Khi xóa data cha thì những data con đang được EF tracking sẽ được sinh ra lệnh
            xóa dưới db trước, cuối cùng là lệnh xóa data cha. Còn những data con nào không được tracking thì sẽ được db tự động xóa (trường hợp này do db
            tự xử lý nên EF sẽ không sinh ra lệnh xóa data con)
        
        DeleteBehavior.ClientCascade: sinh ra dưới db FOREIGN KEY NO ACTION. Khi xóa data cha thì những data con đang được EF tracking sẽ được sinh ra lệnh
            xóa trước sau đó mới tới lệnh xóa data cha (những điều này thực thi khi gọi saveChanges()). Và vì chỉ những data con nào đang được EF tracking
            mới được sinh ra lệnh xóa nên những data con không được tracking sẽ không sinh ra lệnh xóa. Và vì dưới db FOREIGN KEY delete rule là NO ACTION
            nên nếu toàn bộ data con không được xóa hết thì khi xóa data cha sẽ sinh ra lỗi.

        DeleteBehavior.SetNull: sinh ra dưới db FOREIGN KEY SET NULL. Khi xóa data cha thì những data con nào đang được EF tracking thì sẽ tự động được EF
            đặt FOREIGN KEY thành null ở application rồi sau đó gọi lệnh update để set FOREIGN KEY thành null dưới db, cuối cùng gọi lệnh xóa data cha.
            Còn những data con nào không được EF tracking thì sẽ được db tự động đặt FOREIGN KEY thành null (trường hợp này do db tự xử lý nên EF sẽ không 
            sinh ra lệnh cập nhật FOREIGN KEY thành null)

        DeleteBehavior.ClientSetNull: sinh ra FOREIGN KEY NO ACTION ở db. Khi xóa data cha thì những data con đang được EF tracking sẽ tự động đặt FOREIGN
            KEY thành null ở application rồi sau đó gọi lệnh update set FOREIGN KEY thành null dưới db, cuối cùng gọi lệnh xóa cha. Vì dưới db FOREIGN KEY
            với delete rule là NO ACTION nên nếu toàn bộ data con không được EF tracking hết thì => các record không được tracking sẽ không bị EF đặt FOREIGN
            KEY thành null => sẽ không có lệnh update FOREIGN KEY = null dưới db => và vì dưới db FOREIGN KEY với delete rule là NO ACTION nên 
            xóa data cha sẽ báo lỗi

        DeleteBehavior.Restrict: sinh ra ở dưới db ràng buộc FOREIGN KEY NO ACTION. Khi xóa cha thì EF sẽ kiểm tra xem các data con đang được tracked có còn
            tham chiếu đến primary key của data cha hay không ? Sẽ có 3 trường hợp xảy ra như sau:
                + TH1: Nếu còn ít nhất một data con còn tham chiếu đến primary key của data cha và khóa ngoại của data con không được phép null thì sẽ 
                        quăng exception ngay trên application (không thực hiện gọi lện dưới db, tức là ngay khi gọi dbContext.Remove()) => phải gọi lệnh xóa data con trước rồi sau đó gọi lệnh xóa cha
                + TH2: Nếu còn ít nhất một data con còn tham chiếu đến primary key của data cha và FOREIGN KEY của data con được phép đặt thành NULL
                        => khi gọi lệnh xóa data cha thì EF sẽ tự động đặt FOREIGN KEY của data con thành null rồi sau đó gọi lệnh xóa data cha
                        => Tức là sẽ có 2 lệnh được thực thi dưới db: đầu tiên là lệnh update FOREIGN KEY của data con thành null, thứ hai là lệnh xóa data cha
                +TH3: Nếu application chưa load bất kì data con nào hoặc không theo dõi bất kì data còn nào thì sao ? => EF sẽ coi như data cha không có data
                        con nào tham chiếu đến => sẽ gọi lệnh xóa như bth, còn việc có xóa được hay không thì sẽ phụ thuộc vào dữ liệu cũng như là ràng buộc 
                        thực tế dưới db
                + Lưu ý: Nếu DbContext đang không theo dõi bất kỳ data con nào thì sẽ khi gọi lệnh xóa data cha sẽ rơi vào TH3.
                
     */
}
