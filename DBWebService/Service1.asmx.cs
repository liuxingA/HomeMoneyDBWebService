using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;

using System.Collections.Generic;

namespace DBWebService
{
    /// <summary>
    /// Service1 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]

    public class Service1 : System.Web.Services.WebService
    {
        MySqlDB DB = new MySqlDB();

        [WebMethod]
        public String HelloWorld()
        {
            return "Hello World 2016";
        }

        [WebMethod(Description = "登录验证")]
        public String selectADPwd(String mgNo)
        {
            return DB.selectADPwd(mgNo);
        }
        [WebMethod(Description = "删除（参数为SQL语句）")]
        public String delete(String sql)
        {

            DB.delete(sql);
            return "1";
        }

        [WebMethod(Description = "权限验证")]
        public int CheckPermitted(String mgNO)
        {
            return DB.CheckPermitted(mgNO);
        }
        [WebMethod(Description = "查询管理员")]
        //查询管理员 
        public String[] SelectAdmin(String mgNO)
        {
            return DB.SelectAdmin(mgNO);
        }
        //添加管理员
        [WebMethod(Description = "添加管理员")]
        public Boolean insertManager(String mgNO, String permitted, String password)
        {
            return DB.insertManager(mgNO, permitted, password);
        }
        //   删除管理员
        [WebMethod(Description = "删除管理员")]
        public String deleteManager(String mgNO)
        {
            DB.deleteManager(mgNO);
            return "1";
        }
        //查询管理员密码 
        [WebMethod(Description = "查询管理员密码")]
        public String selectAdminPassword(String mgNo)
        {
            //return "123";
            return DB.selectAdminPassword(mgNo);
        }

        //图书入库
        [WebMethod(Description = "图书入库")]
        public String insertBook(String isbn, String BookNo, String BookName, String Author, String Publishment, String BuyTime, String Borrowed, String Ordered, String instroduction)
        {
            DB.insertBook(isbn, BookNo, BookName, Author, Publishment, BuyTime, Borrowed, Ordered, instroduction);
            return "1";
        }
        //删除图书信息
        [WebMethod(Description = "删除图书信息")]
        public String deleteBook(String bookNO)
        {
            DB.deleteBook(bookNO);
            return "1";
        }

        //修改图书信息
        [WebMethod(Description = "修改图书信息")]
        //updateBook(String BookNo,String BookName,String Author,String Publishment,String BuyTime,String Borrowed,String Ordered,String Introduction)
        public String updateBook(String BookNo, String BookName, String Author, String Publishment, String BuyTime, String Borrowed, String Ordered, String Introduction)
        {
            DB.updateBook(BookNo, BookName, Author, Publishment, BuyTime, Borrowed, Ordered, Introduction);

            String ISBN = "";
            ISBN = DB.GetBookNoISBN(BookNo); //得到ISBN
            String sql = "update book set B_Name='" + BookName + "',B_Author='" + Author + "',B_Publishment='" + Publishment + "',B_BuyTime='" + BuyTime + "' Where  ISBN = '" + ISBN + "'";
            String sql2 = "update bdetailedinformation  set Borrowed='" + Borrowed + "',Ordered='" + Ordered + "',Introduction='" + Introduction + "' Where B_Num = '" + BookNo + "'";

            return sql;
        }
        //修改管理员密码
        [WebMethod(Description = "修改管理员密码")]
        public String updateManager(String mgNo, String password)
        {
            DB.updateManager(mgNo, password);
            String sql = "update manager set M_Pwd  = '" + password + "' where M_Num = '" + mgNo + "'";
            return sql;
        }


        //添加学生
        [WebMethod(Description = "添加学生")]
        public String addStu(String StuNO, String StuName, String StuAge, String StuSex, String Class, String Department, String Tel, String Permitted, String Password)
        {
            DB.addStu(StuNO, StuName, StuAge, StuSex, Class, Department, Tel, Permitted, Password);
            return "1";
        }

        //查询学生信息
        [WebMethod(Description = "查询学生信息")]
        public String[] selectStu(String StuNO)
        {
            return DB.selectStu(StuNO);
        }

        //删除学生信息
        [WebMethod(Description = "查询学生信息")]
        public String delectStu(String Sno)
        {
            DB.delectStu(Sno);
            return "1";
        }

        //修改学生信息
        [WebMethod(Description = "修改学生信息")]
        public String updateStu(String StuNO, String StuName, String StuAge, String StuSex, String Class, String Department, String Tel, String Permitted, String Password)
        {
            DB.updateStu(StuNO, StuName, StuAge, StuSex, Class, Department, Tel, Permitted, Password);
            return "1";
        }

        //检查是否超期
        [WebMethod(Description = "检查是否超期")]
        public int checktime(String sno, String bno, string feeflag)
        {
            if (feeflag.ToLower() == "true")
            {
                return DB.checktime(sno, bno, true);
            }
            else
            {
                return DB.checktime(sno, bno, false);
            }

        }
        //查看超期天数信息
        [WebMethod(Description = "查看超期天数信息")]
        public int selectfee(String StuNO)
        {
            return DB.selectfee(StuNO);
        }

        //交费
        [WebMethod(Description = "交费")]
        public String fee(String StuNo, String fee, string feeflag)
        {
            if (feeflag.ToLower() == "true")
            {
                return DB.fee(StuNo, fee, true);
            }
            else
            {
                return DB.fee(StuNo, fee, false);
            }
        }

        //查询借阅或预约图书
        [WebMethod(Description = "查询借阅或预约图书")]
        public String borrowororderbook(String bookNo)
        {
            return DB.borrowororderbook(bookNo);
        }

        //预约图书
        [WebMethod(Description = "预约图书")]
        public String orderbook(String bookNo, String StuNo)
        {
            DB.orderbook(bookNo, StuNo);
            return "1";
        }
        //借阅图书
        [WebMethod(Description = "借阅图书")]
        public String borrowbook(String bookNo, String StuNo)
        {
            DB.borrowbook(bookNo, StuNo);
            return "1";
        }

        [WebMethod(Description = "查询图书信息")]
        public String[] selectbookfromISBN(String ISBN)
        {
            return DB.selectbookfromISBN(ISBN).ToArray();
        }
        [WebMethod(Description = "selectfeeinformation")]
        public String[] selectfeeinformation(String StuNO)
        {
            return DB.selectfeeinformation(StuNO).ToArray();
        }
        //得到挂失图书的信息表中的记录的数量
        [WebMethod(Description = "得到挂失图书的信息表中的记录的数量")]
        public int getMaxGSBH(String VVV)
        {
            return DB.getMaxGSBH();
        }
        //执行没有返回值的插入语句的方法
        [WebMethod(Description = "执行没有返回值的插入语句的方法")]
        public String update(String sql)
        {
            DB.update(sql);
            return "1";
        }
        //已知书名，得到这个书籍的基本信息
        [WebMethod(Description = "已知书名，得到这个书籍的基本信息")]
        public String[] selectAllfrombook(String BookName)
        {
            return DB.selectAllfrombook(BookName).ToArray();
        }
        //通过书号得到书的基本信息
        [WebMethod(Description = "通过书号得到书的基本信息")]
        public String[] selectbookinformationfrombookno(String bookno)
        {
            return DB.selectbookinformationfrombookno(bookno).ToArray();
        }
        //通过学号查询借阅数量
        [WebMethod(Description = "通过学号查询借阅数量")]
        public int selectcount(String StuNO)
        {
            return DB.selectcount(StuNO);
        }
        //得到同种ISBN的书籍的数量
        [WebMethod(Description = "得到同种ISBN的书籍的数量")]
        public int getNumfrombdetailedInfo(String ISBN)
        {
            return DB.getNumfrombdetailedInfo(ISBN);
        }
        //一个ISBN号得到同种号下的这样的书的基本信息
        [WebMethod(Description = "一个ISBN号得到同种号下的这样的书的基本信息")]
        public String[] selectISBNALlfromdetailInfo(String ISBN)
        {
            return DB.selectISBNALlfromdetailInfo(ISBN).ToArray();
        }
        //根据书号得到作者名
        [WebMethod(Description = "根据书号得到作者名")]
        public String getAuthor(String BookNO)
        {
            return DB.getAuthor(BookNO);
        }
        //根据学生ID得到学生的班级和姓名
        [WebMethod(Description = "根据学生ID得到学生的班级和姓名")]
        public String[] getClassAndsname(String StuNO)
        {
            return DB.getClassAndsname(StuNO).ToArray();
        }
        //通过输入图书的作者得到图书的基本信息
        [WebMethod(Description = "通过输入图书的作者得到图书的基本信息")]
        public String[] getAuthorAllfromBook(String Author)
        {
            return DB.getAuthorAllfromBook(Author).ToArray();
        }
        //通过出版社得到图书的基本信息
        [WebMethod(Description = "通过出版社得到图书的基本信息")]
        public String[] getPubAllfrombook(String Publishment)
        {
            return DB.getPubAllfrombook(Publishment).ToArray();
        }
        //通过书名和作者得到图书的基本信息
        [WebMethod(Description = "通过书名和作者得到图书的基本信息")]
        public String[] getBnAuAllfrombook(String BookName, String Author)
        {
            return DB.getBnAuAllfrombook(BookName, Author).ToArray();
        }

        //通过书名和出版社得到图书的基本信息
        [WebMethod(Description = "通过书名和作者得到图书的基本信息")]
        public String[] getBnCbAllfrombook(String BookName, String Publishment)
        {
            return DB.getBnCbAllfrombook(BookName, Publishment).ToArray();
        }
        //通过作者和出版社
        [WebMethod(Description = "通过作者和出版社")]
        public String[] getAuCbAllfrombook(String Author, String Publishment)
        {
            return DB.getAuCbAllfrombook(Author, Publishment).ToArray();
        }
        //通过作者 书名和出版社进行查询
        [WebMethod(Description = "通过作者 书名和出版社进行查询")]
        public String[] getBnAuCbAllfrombook(String BookName, String Author, String Publishment)
        {
            return DB.getBnAuCbAllfrombook(BookName, Author, Publishment).ToArray();
        }
        //通过书号对ISBN和图书简介的查询
        [WebMethod(Description = "通过作者 书名和出版社进行查询")]
        public String[] getISinfromdetails(String BookNo)
        {
            return DB.getISinfromdetails(BookNo).ToArray();
        }

        //以下是后增加的
        //**********************************************************
        //通过ISBN对同一种ISBN号下的基本信息
        [WebMethod(Description = "通过ISBN对同一种ISBN号下的基本信息")]
        public String[] getISfrombook(String isbn)
        {
            return DB.getISfrombook(isbn).ToArray();
        }

        //根据学生的ID得到他预约图书的基本信息
        [WebMethod(Description = "根据学生的ID得到他预约图书的基本信息")]
        public String[] getBNofromOrder(String stuNo)
        {
            return DB.getBNofromOrder(stuNo).ToArray();
        }
        //根据预约图书信息表得到某同学的预约图书信息
        [WebMethod(Description = "根据预约图书信息表得到某同学的预约图书信息")]
        public int getNumfromborderreport(String stuno)
        {

            return DB.getNumfromborderreport(stuno);

        }

        //根据学生的学号得到图书的ISBN，BookNO,BookName,Author,Publishment,借阅时间，归还时间
        [WebMethod(Description = "根据学生的学号得到图书的ISBN，BookNO,BookName,Author,Publishment,借阅时间，归还时间")]
        public String[] getSomeInfo(String stuno)
        {

            return DB.getSomeInfo(stuno).ToArray();

        }



        //根据图书的书号得到图书的基本信息
        [WebMethod(Description = "根据图书的书号得到图书的基本信息")]
        public String[] getBNSomeInfo(String BookNO)
        {
            return DB.getBNSomeInfo(BookNO).ToArray();
        }

        //根据预约图书书号得到图书基本信息
        [WebMethod(Description = "根据预约图书书号得到图书基本信息")]
        public String[] getBNSomeINFO(String BookNO)
        {
            return DB.getBNSomeINFO(BookNO).ToArray();
        }

        //通过学生的ID得到学生的班级，姓名，学号
        [WebMethod(Description = "通过学生的ID得到学生的班级，姓名，学号")]
        public String[] getIDClNO(String stuno)
        {

            return DB.getIDClNO(stuno);

        }

        //通过书号得到归还时间
        [WebMethod(Description = "通过书号得到归还时间")]
        public String gettimefromrecord(String BookNo)
        {
            return DB.gettimefromrecord(BookNo);

        }


        //通过书号判断时候是再借状态
        [WebMethod(Description = "通过书号判断时候是再借状态")]
        public String getifBorrow(String BookNO)
        {
            return DB.getifBorrow(BookNO);

        }

        //通过书号查询预约人
        [WebMethod(Description = "通过书号查询预约人")]
        public String getstu(String BookNO)
        {
            return DB.getstu(BookNO);
        }
        //通过isbn获得书号
        [WebMethod(Description = "通过isbn获得书号")]
        public String getBookNumber(String vvv)
        {
            return DB.getBookNumber();
        }
        //知道学生的学号得到他的密码
        [WebMethod(Description = "知道学生的学号得到他的密码")]
        public String selectPwd(String S_Num)
        {
            return DB.selectPwd(S_Num);
        }
        //得到挂失图书的信息表中的记录的数量
        [WebMethod(Description = "得到挂失图书的信息表中的记录的数量")]
        public int getMaxLBNO()
        {
            return DB.getMaxLBNO();
        }

    }
}
