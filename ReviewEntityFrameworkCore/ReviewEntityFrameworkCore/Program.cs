using Microsoft.EntityFrameworkCore;
using ReviewEntityFrameworkCore.Database;

namespace ReviewEntityFrameworkCore
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var context = new AppDbContext();
            // context.Database.EnsureDeleted();
            // context.Database.EnsureCreated();

            var a = new Article
            {
              Title = "New bài viết",  
              Description = "Ok bro!",
              AuthorId = 3
            };

            context.Add(a);
            await context.SaveChangesAsync();
        }
    }

    /*
        EF: không có OnUpdate

        Add(), AddAsync(), RemoveRange(), Remove(), Update(): không gọi db khi thực thi
        => chỉ gọi Db khi SaveChanges() hoặc SaveChangesAsync()

        Giả sử bạn có 1 data con và 1 data cha. Sẽ có những Trường hợp sau:
            + TH1: cả 2 đều là data mới chưa có trong db. Bạn không gọi Add() cho data cha mà chỉ gọi Add() cho duy nhất data con => Khi Add() data con
                    EF sẽ tracking data con và đồng thơi tracking cả data cha trong đó luôn. =>Khi gọi SaveChanges() thì EF sẽ sinh ra lệnh insert cả data
                    cha và data con dưới db, tất nhiên là sẽ sinh ra lệnh insert data cha trước.
            + TH2: data con là data cũ dưới db (đang được tracking) và data cha là data mới chưa có trong db. Khi bạn thay đổi object reference của data con
                    trỏ đến reference của data cha mới thì khi gọi SaveChanges() EF sẽ tự động tracking data cha mới luôn => sinh ra cả lệnh insert data
                    cha mới và lệnh update data con cũ.
            + TH3: data cha là data cũ dưới db (đang được EF tracking), data con là data mới chưa con trong db. Khi bạn add data con mới vào navigation
                    collection của data cha (không gọi Add() data con bằng dbContext) thì gọi SaveChanges() EF sẽ tự động tracking data con mới này =>
                    sẽ sinh ra lệnh insert data con mới dưới db

        DeleteBehavior.NoAction: sinh ra dưới db FOREIGN KEY NO ACTION. Khác với những enum không có tiền tố Client khác, trường hợp này sẽ chặn ngay trên
            application nếu phát hiện ra data cha vẫn còn data con tham chiếu. Tức là ngày khi gọi lệnh remove nếu EF phát hiện ra vẫn còn data con chưa
            xóa thì sẽ quăng exception luôn, không thực hiện gọi lệnh dưới db.
            => Tóm lại nếu EF phát hiện ra vẫn còn data con chưa xóa thì sẽ chặn luôn ngay trên application, không thực hiện gọi lệnh dưới db.
        
        DeleteBehavior.ClientNoAction: sinh ra dưới db FOREEIGN KEY NO ACTION. EF sẽ không làm gì cả, không chặn, không kiểm tra, bạn yêu cầu xóa gì thì
            nó sẽ sinh ra lệnh xóa đó dưới db (khi gọi saveChanges()), còn việc có xóa được hay không thì phụ thuộc hoàn toàn vào db.
            => Tóm lại, ngay khi bạn gọi saveChanges() thì EF sẽ sinh ra lệnh xóa data cha luôn, còn việc có xóa được hay không thì phụ thuộc hoàn toàn vào
            db
            => Lưu ý: Vì nó sinh ra FOREIGN KEY NO ACTION dưới db nên dù EF không chặn thì khi xóa nếu db phát hiện ra vẫn con data con tham chiếu đến
            PRIMARY KEY của data cha thì db sẽ quăng exception luôn, không thực hiện xóa cha

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
        
        Lưu ý: - EF không có tùy chọn SET DEFAULT như trong SQL Server.
               - entity.Property(a => a.Id)
                    .ValueGeneratedOnAdd() => EF hiểu rằng giá trị của cột này là do Db tự sinh nên khi insert nếu Entity không có giá trị (null) của cột
                                            này thì câu lệnh insert được sinh ra sẽ không có cột này. Còn nếu Entity có giá trị (khác null) của cột này thì
                                            câu lệnh insert sẽ kèm thêm cột này và giá trị của nó.  
                    .UseIdentityColumn(100,5); => bắt đầu từ 100 và mỗi lần tăng lên 5 đơn vị
     */
}
