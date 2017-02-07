using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;

/*#include<iostream>
#include<cstring>
#include<cstdlib>
using namespace std;
*/

namespace DBWebService
{
    public class MySqlDB : IDisposable
    {
        private string siteUrl = ConfigurationManager.AppSettings["SiteURL"];
        static string strLogDBConn = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["LogDBConn"].ToString();

        public static SqlConnection sqlCon;  //用于连接数据库
        private String ConServerStr = strLogDBConn;//"Data Source=(local);database=BOOK;uid=sa;pwd=1";
        public MySqlDB()
        {
            if (sqlCon == null)
            {
                sqlCon = new SqlConnection();
                sqlCon.ConnectionString = ConServerStr;
                sqlCon.Open();
            }
        }


        public void Dispose()
        {
            if (sqlCon != null)
            {
                sqlCon.Close();
                sqlCon = null;
            }
        }
        // ---------------------------------------------------------------------------
        /**
         * 對string加密
         * @param out IN/OUT 返回值
         * @param in IN/OUT 待加密的string
         * @see #DeleteSecret'
         */
         /*
        private void AddSecret(string ansiout, string ansiin)
        {
                if (ansiin == "")
                {
                    ansiout = "135D8A9F";
                    return;
                }
                int win_i, win_len, i;
                wchar_t out1[200], in1[40];
                wcscpy(in1, "");
                wcscpy(out1, "");
                for (i = 0; i < (int)wcslen(ansiout); i++)
                {
                    wcscpy(out1 + i, ansiout->SubString(i + 1, 1).c_str());
                }
                for (i = 0; i < (int)wcslen(ansiin); i++)
                {
                    wcscpy(in1 + i, ansiin->SubString(i + 1, 1).c_str());
                }

                win_len = (int)wcslen(in1);
                wcscpy(out1, L"");
                for (win_i = 0; win_i < win_len; win_i++)
                    swprintf(out1 + win_i * 2, L"%02X", in1[win_i] + win_i + win_len);
                ansiout = out1;
                ansiout->Trim();

        }
        */
        // ---------------------------------------------------------------------------
        /**
         * 對string解密
         * @param out IN/OUT 返回值
         * @param in IN/OUT 待解密的string
         * @see #AddSecret
         */
         /*
        private void DeleteSecret(string ansiout, string ansiin)
        {
                if (*ansiin == L"135D8A9F")
                {
                    *ansiout = L"";
                    return;
                }
                else if (*ansiin == "")
                {
                    *ansiout = L"不可能為空";
                    return;
                }
                int win_i, win_j, i;
                int win_len;
                wchar_t win_cm[3];
                wchar_t *end;
                wchar_t out[200], in[40];
                wcscpy(in, L"");
                wcscpy(out, L"");

                for (i = 0; i < (int)wcslen(ansiout->c_str()); i++)
                {
                    wcscpy(out + i, ansiout->SubString(i + 1, 1).c_str());
                }
                for (i = 0; i < (int)wcslen(ansiin->c_str()); i++)
                {
                    wcscpy(in + i, ansiin->SubString(i + 1, 1).c_str());
                }

                win_len = (int)wcslen(in);
                for (win_i = 0; win_i < win_len / 2; win_i++)
                {
                    swprintf(win_cm, L"%-2.2s", in + win_i * 2);
                    win_j = (int)wcstoul(win_cm, &end, 16) - win_i - win_len / 2;
                    out[win_i] = (wchar_t)win_j;
                }
                out[win_i] = '\0';
                *ansiout = out;
                ansiout->Trim();
                *ansiin = in;
                ansiin->Trim();
        }
        */
        #region 管理端函数
        //登录验证
        public String selectADPwd(String mgNo)
        {
            String result = "";
            try
            {

                String sql = "select '1' from tam05 where ae01='" + mgNo + "'";


                SqlCommand command = new SqlCommand(sql, sqlCon);
                SqlDataReader dr = command.ExecuteReader();

                while (dr.Read())
                {
                    //result = "M_pwd ";
                    result = Convert.ToString(dr[0]);
                }
                dr.Close();
                command.Dispose();
            }
            catch (Exception e)
            {
                return result;
            }

            return result;

        }

        public String delete(String sql)
        {
            try
            {
                SqlCommand command = new SqlCommand(sql, sqlCon);
                command.ExecuteNonQuery();
                command.Dispose();
            }
            catch (Exception e)
            {
                // 
            }
            return "1";
        }


        //权限验证
        public int CheckPermitted(String mgNO)
        {
            int permitted = 0;
            String result = null;
            try
            {

                String sql = "select M_Permitted from student where M_Num='" + mgNO + "'";
                SqlCommand command = new SqlCommand(sql, sqlCon);
                SqlDataReader dr = command.ExecuteReader();

                while (dr.Read())
                {
                    result = Convert.ToString(dr[0]);
                    if (result.Equals("普通"))
                    {
                        permitted = 1;
                    }
                    else if (result.Equals("高级"))
                    {
                        permitted = 0;
                    }
                }
                dr.Close();
                command.Dispose();

            }
            catch (Exception e)
            {
                //
            }


            return permitted;
        }

        //查询管理员 
        public String[] SelectAdmin(String mgNO)
        {
            String[] sa = new String[3];
            try
            {


                String sql = "select M_Permitted,M_Pwd from manager where M_Num='" + mgNO + "'";
                SqlCommand command = new SqlCommand(sql, sqlCon);
                SqlDataReader rs = command.ExecuteReader();

                while (rs.Read())
                {

                    sa[0] = rs[0].ToString();
                    sa[1] = rs[1].ToString();
                    sa[2] = mgNO;
                }
                rs.Close();
                command.Dispose();
            }
            catch (Exception e)
            {
                //
            }

            return sa;
        }

        //添加管理员
        public Boolean insertManager(String mgNO, String permitted, String password)
        {
            Boolean falg = false;
            try
            {

                String sql = "insert into manager( M_Num,M_Permitted,M_Pwd ) values ( '" + mgNO + "','" + permitted + "','" + password + "')";
                SqlCommand command = new SqlCommand(sql, sqlCon);
                int J = 0;
                J = command.ExecuteNonQuery();
                command.Dispose();
                falg = true;
            }
            catch (Exception e)
            {
                falg = false;
            }

            return falg;

        }

        //   删除管理员
        public String deleteManager(String mgNO)
        {
            try
            {
                String sql = "delete from manager where M_Num='" + mgNO + "'";

                SqlCommand command = new SqlCommand(sql, sqlCon);
                int J = 0;
                J = command.ExecuteNonQuery();

                command.Dispose();

            }
            catch (Exception e)
            {

            }
            return "1";
        }


        //查询管理员密码 
        public String selectAdminPassword(String mgNo)
        {
            String pwd = null;
            try
            {


                String sql = "select ae04 from tam05 where ae01='" + mgNo + "'";
                SqlCommand command = new SqlCommand(sql, sqlCon);
                SqlDataReader dr = command.ExecuteReader();

                while (dr.Read())
                {
                    pwd = dr[0].ToString();
                }
                dr.Close();
                command.Dispose();


            }
            catch (Exception e)
            {
                //
            }

            return pwd;

        }


        //修改管理员密码
        public String updateManager(String mgNo, String password)
        {
            try
            {

                String sql = "update manager set M_Pwd  = '" + password + "' where M_Num = '" + mgNo.Trim() + "'";
                SqlCommand command = new SqlCommand(sql, sqlCon);
                command.ExecuteNonQuery();
                command.Dispose();

            }
            catch (Exception e)
            {
                //
            }
            return "1";
        }

        //图书入库
        public String insertBook(String isbn, String BookNo, String BookName, String Author, String Publishment, String BuyTime, String Borrowed, String Ordered, String instroduction)
        {
            try
            {

                String sql1 = "insert into book(ISBN,B_Name,B_Author,B_Publishment,B_BuyTime) values('" + isbn + "'," +
                        "'" + BookName + "','" + Author + "','" + Publishment + "','" + BuyTime + "')";
                String sql2 = "insert into bdetailedinformation(B_Num,ISBN,Borrowed,Ordered,Introduction) values('" + BookNo + "'," +
                "'" + isbn + "','" + Borrowed + "','" + Ordered + "','" + instroduction + "')";
                SqlCommand command1 = new SqlCommand(sql1, sqlCon);
                command1.ExecuteNonQuery();
                SqlCommand command2 = new SqlCommand(sql2, sqlCon);
                command2.ExecuteNonQuery();
                command1.Dispose();
                command2.Dispose();
            }
            catch (Exception e)
            {
                //
            }
            return "1";
        }
        //添加一笔支出记录
        public String insertHm11(int seq,int flow,String type, String date, String dalei, String xiaolei, double amt, String zhanghu, String otherzhanhu)
        {
            try
            {

                String sql1 = "insert into hm11(seq,flow,mk01,mk02,mk03,mk04,mk05,mk06,mk07,mk08,mk09,mk10,mk11) values("+seq+","+flow+",'" + type + "'," +
                "'" + date + "','" + dalei + "','" + xiaolei + "'," + amt + ",'"+zhanghu+"','','','"+otherzhanhu+"',1,1)";
                SqlCommand command1 = new SqlCommand(sql1, sqlCon);
                command1.ExecuteNonQuery();
                command1.Dispose();
            }
            catch (Exception e)
            {
                //
            }
            return "1";
        }
        //删除图书信息
        public String deleteBook(String bookNO)
        {
            try
            {

                String sql = "delete from bdetailedinformation where B_Num='" + bookNO + "'";
                String sql1 = "delete from book where B_Num='" + bookNO + "'";
                SqlCommand command1 = new SqlCommand(sql1, sqlCon);
                command1.ExecuteNonQuery();
                SqlCommand command2 = new SqlCommand(sql, sqlCon);
                command2.ExecuteNonQuery();
                command1.Dispose();
                command2.Dispose();
            }
            catch (Exception e)
            {
                //
            }
            return "1";
        }

        //修改图书信息
        public String updateBook(String BookNo, String BookName, String Author, String Publishment, String BuyTime, String Borrowed, String Ordered, String Introduction)
        {
            try
            {
                String ISBN = "";
                ISBN = GetBookNoISBN(BookNo); //得到ISBN
                String sql = "update book set B_Name='" + BookName + "',B_Author='" + Author + "',B_Publishment='" + Publishment + "',B_BuyTime='" + BuyTime + "' Where  ISBN = '" + ISBN + "'";
                String sql2 = "update bdetailedinformation  set Borrowed='" + Borrowed + "',Ordered='" + Ordered + "',Introduction='" + Introduction + "' Where B_Num = '" + BookNo + "'";
                SqlCommand command1 = new SqlCommand(sql2, sqlCon);
                command1.ExecuteNonQuery();
                SqlCommand command2 = new SqlCommand(sql, sqlCon);
                command2.ExecuteNonQuery();
                command1.Dispose();
                command2.Dispose();
            }
            catch (Exception e)
            {
                //
            }
            return "1";
        }

        //借阅图书
        public String GetBookNoISBN(String bookNo)
        {

            String sql = "select ISBN from bdetailedinformation  Where B_Num = '" + bookNo + "'";
            SqlCommand command = new SqlCommand(sql, sqlCon);
            SqlDataReader dr = command.ExecuteReader();

            while (dr.Read())
            {
                sql = dr[0].ToString();

            }
            dr.Close();
            command.Dispose();
            return sql;


        }


        //添加学生
        public String addStu(String StuNO, String StuName, String StuAge, String StuSex, String Class, String Department, String Tel, String Permitted, String Password)
        {
            try
            {

                String sql = "insert into student(S_Num,S_Name,S_Age,S_Sex,S_Class,S_Department,S_Phone,S_Permitted,S_Pwd) values('" + StuNO + "','" + StuName + "','" + StuAge + "','" + StuSex + "','" + Class + "','" + Department + "','" + Tel + "','" + Permitted + "','" + Password + "')";
                SqlCommand command = new SqlCommand(sql, sqlCon);
                command.ExecuteNonQuery();
                command.Dispose();
            }
            catch (Exception e)
            {

            }
            return "1";
        }

        //查询学生信息
        public String[] selectStu(String StuNO)
        {
            String[] ss = new String[8];
            try
            {

                String sql = "select S_Name,S_Age,S_Sex,S_Class,S_Department,S_Phone,S_Permitted,S_Pwd from student where S_Num='" + StuNO + "'";
                SqlCommand command = new SqlCommand(sql, sqlCon);
                SqlDataReader dr = command.ExecuteReader();

                while (dr.Read())
                {
                    ss[0] = dr[0].ToString();
                    ss[1] = dr[1].ToString();
                    ss[2] = dr[2].ToString();
                    ss[3] = dr[3].ToString();
                    ss[4] = dr[4].ToString(); ;
                    ss[5] = dr[5].ToString();
                    ss[6] = dr[6].ToString();
                    ss[7] = dr[7].ToString();

                }
                dr.Close();
                command.Dispose();

            }
            catch (Exception e)
            {
                //
            }
            return ss;

        }

        //删除学生信息
        public String delectStu(String Sno)
        {
            try
            {

                String sql = "delete from student where S_Num='" + Sno + "'";
                SqlCommand command = new SqlCommand(sql, sqlCon);
                command.ExecuteNonQuery();
                command.Dispose();

            }
            catch (Exception e)
            {
                //
            }
            return "1";
        }

        //修改学生信息
        public void updateStu(String StuNO, String StuName, String StuAge, String StuSex, String Class, String Department, String Tel, String Permitted, String Password)
        {
            try
            {

                String sql = "update student set S_Num='" + StuNO + "',S_Name='" + StuName + "',S_Age='" + StuAge + "',S_Sex='" + StuSex + "',S_Class='" + Class + "',S_Department='" + Department + "',S_Phone='" + Tel + "',S_Permitted='" + Permitted + "',S_Pwd='" + Password + "'   where  S_Num='" + StuNO + "'";
                SqlCommand command = new SqlCommand(sql, sqlCon);
                command.ExecuteNonQuery();
                command.Dispose();
            }
            catch (Exception e)
            {

            }
        }

        //检查是否超期 int
        public int checktime(String sno, String bno, Boolean feeflag)
        {//-1代表超期没交罚款  0代表当天借的书   1代表正常还的书  -2表示超期交罚款
            int day = 0;
            int flag = 0;
            String returntime = "";
            try
            {

                String sqlr = "select ReturnTime from record where S_Num = '" + sno + "' and B_Num = '" + bno + "'";
                SqlCommand commandr = new SqlCommand(sqlr, sqlCon);
                SqlDataReader rrs = commandr.ExecuteReader();

                while (rrs.Read())
                {


                    returntime = rrs[0].ToString();//获取应rrs[0].ToString()该归还的时间

                }
                rrs.Close();
                commandr.Dispose();
                //return returntime;

            }
            catch (Exception e)
            {

            }
            String[] strday = Regex.Split(returntime, "\\-");//这里使用了简单的正则式，规定了时间的格式
            int ryear = int.Parse(strday[0]);
            int rmonth = int.Parse(strday[1].Trim());
            int rday = int.Parse(strday[2].Trim());

            DateTime D1 = DateTime.Now;
            DateTime D2 = new DateTime(ryear, rmonth, rday);
            TimeSpan ts = D1 - D2;
            day = ts.Days;

            //return day;
            if (day == 0)
            {
                //表示当天借的书
                flag = 0;
            }
            else if (day > 0)
            {//代表超期了
                if (feeflag)
                {//超期交费
                    flag = -2;
                }
                else
                {//超期没交费
                    try
                    {
                        int jf = 0;
                        String sql = "select * From  overtime  where S_Num = '" + sno + "' and B_Num = '" + bno + "'";
                        SqlCommand command = new SqlCommand(sql, sqlCon);
                        SqlDataReader dr = command.ExecuteReader();
                        String BNAME = "";
                        BNAME = GetBookName(bno);
                        while (dr.Read())
                        {
                            jf = 1;
                        }
                        command.Dispose();
                        dr.Dispose();

                        if (jf == 1)
                        {
                            String bname;
                            String sql1 = "update overtime set overtime='" + day + "' , S_Num = '" + sno + "' , B_Num='" + bno + "' , B_Name = '" + BNAME + "'";
                            SqlCommand command1 = new SqlCommand(sql1, sqlCon);
                            command1.ExecuteNonQuery();
                            command1.Dispose();
                            //return 2;
                        }
                        else
                        {
                            //String sql = "insert into student(S_Num,S_Name,S_Age,S_Sex,S_Class,S_Department,S_Phone,S_Permitted,S_Pwd) values('" + StuNO + "','" + StuName + "','" + StuAge + "','" + StuSex + "','" + Class + "','" + Department + "','" + Tel + "','" + Permitted + "','" + Password + "')";
                            String sql12 = "insert into  overtime (overtime,S_Num,B_Num,B_Name) Values ('" + day + "', '" + sno + "','" + bno + "','" + BNAME + "')";
                            SqlCommand command111 = new SqlCommand(sql12, sqlCon);
                            command111.ExecuteNonQuery();
                            command111.Dispose();
                            //return 3;
                        }

                    }
                    catch (Exception e)
                    {

                    }
                    flag = -1;
                }
            }
            else
            {
                //可以正常归还的书
                flag = 1;
            }
            //return returntime;
            return flag;

        }

        //查看超期天数信息
        public int selectfee(String StuNO)
        {
            int day = 0;
            try
            {
                int flag = 0;

                String sql = "select Overtime from overtime where S_Num='" + StuNO + "'";
                SqlCommand command = new SqlCommand(sql, sqlCon);
                SqlDataReader rs = command.ExecuteReader();
                while (rs.Read())
                {
                    flag++;
                    day += int.Parse(rs[0].ToString());
                }
                rs.Close();
                command.Dispose();

            }
            catch (Exception e)
            {

            }
            return day;
        }

        //交费
        public String fee(String StuNo, String fee, Boolean feeflag)
        {
            try
            {

                String sql = "update Overtime set overtime='0' where S_Num='" + StuNo + "'";
                SqlCommand command = new SqlCommand(sql, sqlCon);
                feeflag = true;
                command.Dispose();
                return "true";
            }
            catch (Exception e)
            {
                //
            }
            return "false";
        }
        //查询借阅或预约图书
        public String borrowororderbook(String bookNo)
        {
            String s = null;
            try
            {

                String sql = "select Borrowed from record where B_Num='" + bookNo + "'";
                SqlCommand command = new SqlCommand(sql, sqlCon);
                SqlDataReader rs = command.ExecuteReader();
                while (rs.Read())
                {
                    s = rs[0].ToString();

                }
                rs.Close();
                command.Dispose();
            }
            catch (Exception e)
            {
                // 
            }
            return s;
        }
        //预约图书
        public String orderbook(String bookNo, String StuNo)
        {
            try
            {

                String sql = "update record set ordered=‘是’,S_Num='" + StuNo + "' where B_Num='" + bookNo + "'";
                SqlCommand command = new SqlCommand(sql, sqlCon);
                command.ExecuteNonQuery();
                command.Dispose();
            }
            catch (Exception e)
            {

            }
            return "1";
        }

        //借阅图书
        public String borrowbook(String bookNo, String StuNo)
        {

            String day = DateTime.Now.ToString("yyyy-MM-dd");
            //try
            //{

            String sql = "update record set Borrowed = '是',S_Num='" + StuNo + "',borrowtime = '" + day + "' where B_Num='" + bookNo + "'";
            SqlCommand command = new SqlCommand(sql, sqlCon);
            command.ExecuteNonQuery();
            command.Dispose();

            //}
            //catch (Exception e)
            ///{

            //}
            return "1";
        }

        public List<String> selectbookfromISBN(String ISBN)
        {

            List<String> v = new List<String>();

            try
            {
                //测试在后台打印    		

                String sql = "select book.B_Name,book.B_Author,book.B_Publishment,book.B_Buytime," +
                        "bdetailedinformation.B_Num,bdetailedinformation.Borrowed," +
                        "bdetailedinformation.Ordered,bdetailedinformation.Introduction from " +
                        "book,bdetailedinformation where bdetailedinformation.ISBN=book.ISBN " +
                        "And book.ISBN='" + ISBN + "'";
                SqlCommand command = new SqlCommand(sql, sqlCon);
                SqlDataReader rs = command.ExecuteReader();
                while (rs.Read())
                {//将结果集信息添加到返回向量中								
                    v.Add(rs[0].ToString());
                    v.Add(rs[1].ToString());
                    v.Add(rs[2].ToString());
                    v.Add(rs[3].ToString());
                    v.Add(rs[4].ToString());
                    v.Add(rs[5].ToString());
                    v.Add(rs[6].ToString());
                    v.Add(rs[7].ToString());

                }
                rs.Close();
                command.Dispose();

            }
            catch (Exception e)
            {

            }

            return v;
        }
        public List<String> selectfeeinformation(String StuNO)
        {
            List<String> v = new List<String>();

            try
            {


                String sql = "select B_Num,B_Name,Overtime from overtime where S_Num='" + StuNO + "'";
                SqlCommand command = new SqlCommand(sql, sqlCon);
                SqlDataReader rs = command.ExecuteReader();
                while (rs.Read())
                {//将结果集信息添加到返回向量中								

                    v.Add(rs[0].ToString());
                    v.Add(rs[1].ToString());
                    v.Add(rs[2].ToString());

                }
                rs.Close();
                command.Dispose();
            }
            catch (Exception e)
            {
                //

            }



            return v;

        }

        //得到挂失图书的信息表中的记录的数量
        public int getMaxGSBH()
        {
            int result = 0;
            try
            {

                String sql = "select MAX(GSBH) from losebook";
                SqlCommand command = new SqlCommand(sql, sqlCon);
                SqlDataReader rs = command.ExecuteReader();
                while (rs.Read())
                {
                    result = int.Parse(rs[0].ToString());
                }
                rs.Close();
                command.Dispose();
            }
            catch (Exception e)
            {

            }
            return result;

        }
        //执行没有返回值的插入语句的方法
        public String update(String sql)
        {
            try
            {
                SqlCommand command = new SqlCommand(sql, sqlCon);
                command.ExecuteNonQuery();
                command.Dispose();
            }
            catch (Exception e)
            {
            }
            return "1";
        }

        //已知书名，得到这个书籍的基本信息
        public List<String> selectAllfrombook(String BookName)
        {
            List<String> v = new List<String>();
            //    	int lenght=0;
            try
            {
                //测试在后台打印    		

                String sql = "select ISBN,B_Name,B_Author,B_Publishment from book where B_Name like '%" + BookName + "%'";
                SqlCommand command = new SqlCommand(sql, sqlCon);
                SqlDataReader rs = command.ExecuteReader();
                while (rs.Read())
                {//将结果集信息添加到返回向量中										
                    v.Add(rs[0].ToString());
                    v.Add(rs[1].ToString());
                    v.Add(rs[2].ToString());
                    v.Add(rs[3].ToString());
                }
                rs.Close();
                command.Dispose();

            }
            catch (Exception e)
            {
                //
            }

            return v;
        }

        //通过书号得到书的基本信息
        public String[] selectbookinformationfrombookno(String bookno)
        {
            String[] info = new String[6];
            try
            {

                String sql = "select book.B_Name,book.B_Author,book.B_Publishment,bdetailedinformation.Borrowed,bdetailedinformation.Ordered from book,bdetailedinformation where book.ISBN = bdetailedinformation.ISBN and bdetailedinformation.B_Num='" + bookno + "'";
                SqlCommand command = new SqlCommand(sql, sqlCon);
                SqlDataReader rs = command.ExecuteReader();

                while (rs.Read())
                {
                    info[1] = rs[0].ToString();
                    info[2] = rs[1].ToString();
                    info[3] = rs[2].ToString();
                    info[4] = rs[3].ToString();
                    info[5] = rs[4].ToString();
                }
                rs.Close();
                command.Dispose();
            }
            catch (Exception e)
            {

            }
            return info;
        }

        //通过学号查询借阅数量
        public int selectcount(String StuNO)
        {
            int a = 0;
            try
            {

                String sql = "select count(B_Num) from record where S_Num='" + StuNO + "'";
                SqlCommand command = new SqlCommand(sql, sqlCon);
                SqlDataReader rs = command.ExecuteReader();
                while (rs.Read())
                {
                    a = int.Parse(rs[0].ToString());
                }
            }
            catch (Exception e)
            {

            }

            return a;
        }
        //得到同种ISBN的书籍的数量
        public int getNumfrombdetailedInfo(String ISBN)
        {
            int num = 0;
            try
            {
                //测试在后台打印    		
                String sql = "select count(B_Num) from bdetailedinformation where ISBN='" + ISBN + "'";
                SqlCommand command = new SqlCommand(sql, sqlCon);
                SqlDataReader rs = command.ExecuteReader();
                while (rs.Read())
                {//将结果集信息添加到返回向量中								
                 //String[] middle=new String[6];
                    num = int.Parse(rs[0].ToString());
                }
                rs.Close();
                command.Dispose();
            }

            catch (Exception e)
            {

            }
            return num;

        }


        //一个ISBN号得到同种号下的这样的书的基本信息
        public List<String> selectISBNALlfromdetailInfo(String ISBN)
        {
            List<String> v = new List<String>();
            int lenght = 0;
            try
            {
                //测试在后台打印    		

                String sql = "select B_Num,Borrowed,Ordered,Introduction from bdetailedinformation where ISBN='" + ISBN + "'";
                SqlCommand command = new SqlCommand(sql, sqlCon);
                SqlDataReader rs = command.ExecuteReader();
                while (rs.Read())
                {//将结果集信息添加到返回向量中								
                 //String[] middle=new String[6];				
                    v.Add(rs[0].ToString());
                    v.Add(rs[1].ToString());
                    v.Add(rs[2].ToString());
                    v.Add(rs[3].ToString());
                }
                rs.Close();
                command.Dispose();
            }
            catch (Exception e)
            {

            }
            return v;
        }

        //根据书号得到作者名
        public String getAuthor(String BookNO)
        {
            String result = null;
            try
            {

                String sql = "select B_Author from book where B_Num='" + BookNO + "'";
                SqlCommand command = new SqlCommand(sql, sqlCon);
                SqlDataReader rs = command.ExecuteReader();
                while (rs.Read())
                {
                    result = rs[0].ToString();
                }
                rs.Close();
                command.Dispose();
            }
            catch (Exception e)
            {

            }
            return result;

        }

        //根据学生ID得到学生的班级和姓名
        public List<String> getClassAndsname(String StuNO)
        {
            List<String> result = new List<String>();
            try
            {

                String sql = "select S_Name,S_Class,S_Num from student where S_Num='" + StuNO + "'";
                SqlCommand command = new SqlCommand(sql, sqlCon);
                SqlDataReader rs = command.ExecuteReader();
                while (rs.Read())
                {//将结果集信息添加到返回向量中


                    String[] middle = new String[3];
                    middle[0] = rs[0].ToString();
                    middle[1] = rs[1].ToString();
                    middle[2] = rs[2].ToString();
                    String[] str = new String[1];
                    str[0] = middle[0] + "/" + middle[1] + "/" + middle[2];
                    result.Add(str[0]);
                }
                rs.Close();
                command.Dispose();

            }
            catch (Exception e)
            {

            }
            return result;


        }
        //通过输入图书的作者得到图书的基本信息
        public List<String> getAuthorAllfromBook(String Author)
        {
            List<String> v = new List<String>();
            try
            {
                //测试在后台打印    		

                String sql = "select ISBN,B_Name,B_Author,B_Publishment from book where B_Author like '%" + Author + "%'";
                SqlCommand command = new SqlCommand(sql, sqlCon);
                SqlDataReader rs = command.ExecuteReader();
                while (rs.Read())
                {//将结果集信息添加到返回向量中
                    v.Add(rs[0].ToString());
                    v.Add(rs[1].ToString());
                    v.Add(rs[2].ToString());
                    v.Add(rs[3].ToString());
                }

                rs.Close();
                command.Dispose();
            }
            catch (Exception e)
            {

            }
            return v;
        }
        //通过出版社得到图书的基本信息
        public List<String> getPubAllfrombook(String Publishment)
        {
            List<String> v = new List<String>();
            try
            {
                //测试在后台打印   		

                String sql = "select ISBN,B_Name,B_Author,B_Publishment from book where B_Publishment like '%" + Publishment + "%'";
                SqlCommand command = new SqlCommand(sql, sqlCon);
                SqlDataReader rs = command.ExecuteReader();
                while (rs.Read())
                {//将结果集信息添加到返回向量中				
                    v.Add(rs[0].ToString());
                    v.Add(rs[1].ToString());
                    v.Add(rs[2].ToString());
                    v.Add(rs[3].ToString());
                }

                rs.Close();
                command.Dispose();
            }
            catch (Exception e)
            {

            }
            return v;

        }
        //通过书名和作者得到图书的基本信息
        public List<String> getBnAuAllfrombook(String BookName, String Author)
        {
            List<String> v = new List<String>();
            try
            {
                //测试在后台打印   		

                String sql = "select ISBN,B_Name,B_Author,B_Publishment from book where B_Name like '%" + BookName + "%' and B_Author like '%" + Author + "%'";
                SqlCommand command = new SqlCommand(sql, sqlCon);
                SqlDataReader rs = command.ExecuteReader();
                while (rs.Read())
                {//将结果集信息添加到返回向量中
                    v.Add(rs[0].ToString());
                    v.Add(rs[1].ToString());
                    v.Add(rs[2].ToString());
                    v.Add(rs[3].ToString());
                }

                rs.Close();
                command.Dispose();
            }
            catch (Exception e)
            {

            }
            return v;
        }

        //通过书名和出版社得到图书的基本信息
        public List<String> getBnCbAllfrombook(String BookName, String Publishment)
        {
            List<String> v = new List<String>();
            try
            {
                //测试在后台打印   		

                String sql = "select ISBN,B_Name,B_Author,B_Publishment from book where B_Name like '%" + BookName + "%' and B_Publishment like '%" + Publishment + "%'";
                SqlCommand command = new SqlCommand(sql, sqlCon);
                SqlDataReader rs = command.ExecuteReader();
                while (rs.Read())
                {//将结果集信息添加到返回向量中
                    v.Add(rs[0].ToString());
                    v.Add(rs[1].ToString());
                    v.Add(rs[2].ToString());
                    v.Add(rs[3].ToString());
                }
                rs.Close();
                command.Dispose();
            }
            catch (Exception e)
            {

            }
            return v;
        }
        //通过作者和出版社
        public List<String> getAuCbAllfrombook(String Author, String Publishment)
        {
            List<String> v = new List<String>();
            try
            {
                //测试在后台打印   		

                String sql = "select ISBN,B_Name,B_Author,B_Publishment from book where B_Author like '%" + Author + "%' and B_Publishment like '%" + Publishment + "%'";
                SqlCommand command = new SqlCommand(sql, sqlCon);
                SqlDataReader rs = command.ExecuteReader();
                while (rs.Read())
                {//将结果集信息添加到返回向量中
                    v.Add(rs[0].ToString());
                    v.Add(rs[1].ToString());
                    v.Add(rs[2].ToString());
                    v.Add(rs[3].ToString());
                }
                rs.Close();
                command.Dispose();
            }
            catch (Exception e)
            {

            }
            return v;
        }

        //通过作者 书名和出版社进行查询
        public List<String> getBnAuCbAllfrombook(String BookName, String Author, String Publishment)
        {
            List<String> v = new List<String>();
            try
            {
                //测试在后台打印   		

                String sql = "select ISBN,B_Name,B_Author,B_Publishment from book where B_Name like '%" + BookName + "%' and B_Author like '%" + Author + "%' and B_Publishment like '%" + Publishment + "%'";
                SqlCommand command = new SqlCommand(sql, sqlCon);
                SqlDataReader rs = command.ExecuteReader();
                while (rs.Read())
                {//将结果集信息添加到返回向量中
                    v.Add(rs[1].ToString());
                    v.Add(rs[1].ToString());
                    v.Add(rs[1].ToString());
                    v.Add(rs[1].ToString());
                }
                rs.Close();
                command.Dispose();
            }
            catch (Exception e)
            {

            }
            return v;
        }

        //通过书号对ISBN和图书简介的查询
        public List<String> getISinfromdetails(String BookNo)
        {
            List<String> v = new List<String>();
            try
            {
                //测试在后台打印   		

                String sql = "select ISBN,Borrowed,Ordered,Introduction from bdetailedinformation where B_Num='" + BookNo + "'";
                SqlCommand command = new SqlCommand(sql, sqlCon);
                SqlDataReader rs = command.ExecuteReader();
                while (rs.Read())
                {//将结果集信息添加到返回向量中
                    v.Add(rs[0].ToString());
                    v.Add(rs[1].ToString());
                    v.Add(rs[2].ToString());
                    v.Add(rs[3].ToString());
                }
                rs.Close();
                command.Dispose();
            }
            catch (Exception e)
            {

            }
            return v;
        }

        //以下是后增加的
        //**********************************************************
        //通过ISBN对同一种ISBN号下的基本信息
        public List<String> getISfrombook(String isbn)
        {
            List<String> v = new List<String>();
            try
            {
                //测试在后台打印   		


                String sql = "select ISBN,B_Name,B_Author,B_Publishment from book where ISBN ='" + isbn + "'";
                SqlCommand command = new SqlCommand(sql, sqlCon);
                SqlDataReader rs = command.ExecuteReader();
                while (rs.Read())
                {//将结果集信息添加到返回向量中
                    v.Add(rs[0].ToString());
                    v.Add(rs[1].ToString());
                    v.Add(rs[2].ToString());
                    v.Add(rs[3].ToString());
                }
                rs.Close();
                command.Dispose();
            }
            catch (Exception e)
            {

            }
            return v;
        }

        //根据学生的ID得到他预约图书的基本信息
        public List<String> getBNofromOrder(String stuNo)
        {
            List<String> v = new List<String>();
            try
            {
                //测试在后台打印   		


                String sql = "select B_Num,S_Name,S_Num,B_Author from orderbook where S_Num ='" + stuNo + "'";
                SqlCommand command = new SqlCommand(sql, sqlCon);
                SqlDataReader rs = command.ExecuteReader();
                while (rs.Read())
                {//将结果集信息添加到返回向量中
                    v.Add(rs[0].ToString());
                    v.Add(rs[1].ToString());
                    v.Add(rs[2].ToString());
                    v.Add(rs[3].ToString());
                }
                rs.Close();
                command.Dispose();
            }
            catch (Exception e)
            {

            }
            return v;
        }
        //根据预约图书信息表得到某同学的预约图书信息
        public int getNumfromborderreport(String stuno)
        {
            int num = 0;
            try
            {
                //测试在后台打印    		


                String sql = "select count(B_Num) from orderbook where B_Num='" + stuno + "'";
                SqlCommand command = new SqlCommand(sql, sqlCon);
                SqlDataReader rs = command.ExecuteReader();
                while (rs.Read())
                {//将结果集信息添加到返回向量中								
                 //String[] middle=new String[6];
                    num = int.Parse(rs[0].ToString());
                }
                rs.Close();
                command.Dispose();
            }

            catch (Exception e)
            {

            }
            return num;

        }

        //根据学生的学号得到图书的ISBN，BookNO,BookName,Author,Publishment,借阅时间，归还时间
        public String[] getSomeInfo(String stuno)
        {
            List<String> result = new List<String>();
            try
            {
                //测试在后台打印    		

                String sql = " SELECT record.B_Num, record.BorrowTime, record.ReturnTime, book.ISBN, book.B_Name, book.B_Author, book.B_Publishment FROM record INNER JOIN  bdetailedinformation ON record.B_Num = bdetailedinformation.B_Num INNER JOIN    book ON bdetailedinformation.ISBN = book.ISBN WHERE (record.S_Num = '" + stuno + "')";
                //String sql="select record.B_Num,record.BorrowTime,record.ReturnTime,book.ISBN,book.B_Name,book.B_Author,book.B_Publishment from record,book,bdetailedinformation where record.B_Num=bdetailedinformation.B_Num AND bdetailedinformation.ISBN=book.ISBN And record.S_Num='"+stuno+"'";
                int num = 0;
                SqlCommand command = new SqlCommand(sql, sqlCon);
                SqlDataReader rs = command.ExecuteReader();
                while (rs.Read())
                {//将结果集信息添加到返回向量中								
                 //String[] middle=new String[6];
                    result.Add(rs[0].ToString());
                    result.Add(rs[1].ToString());
                    result.Add(rs[2].ToString());
                    result.Add(rs[3].ToString());
                    result.Add(rs[4].ToString());
                    result.Add(rs[5].ToString());
                    result.Add(rs[6].ToString());
                    num++;
                    // return result;

                }
                rs.Close();
                command.Dispose();
            }

            catch (Exception e)
            {

            }
            return result.ToArray();

        }



        //根据图书的书号得到图书的基本信息
        public List<String> getBNSomeInfo(String BookNO)
        {
            List<String> result = new List<String>();
            try
            {
                //测试在后台打印    		


                String sql = "select record.B_Num,record.BorrowTime,record.ReturnTime,book.ISBN,book.B_Name,book.B_Author,book.B_Publishment from record,book,bdetailedinformation where record.B_Num=bdetailedinformation.B_Num AND bdetailedinformation.ISBN=book.ISBN And record.B_Num='" + BookNO + "'";
                SqlCommand command = new SqlCommand(sql, sqlCon);
                SqlDataReader rs = command.ExecuteReader();
                while (rs.Read())
                {//将结果集信息添加到返回向量中								
                 //String[] middle=new String[6];
                    result.Add(rs[0].ToString());
                    result.Add(rs[1].ToString());
                    result.Add(rs[2].ToString());
                    result.Add(rs[3].ToString());
                    result.Add(rs[4].ToString());
                    result.Add(rs[5].ToString());
                    result.Add(rs[6].ToString());
                }
                rs.Close();
                command.Dispose();
            }

            catch (Exception e)
            {

            }
            return result;

        }

        //根据预约图书书号得到图书基本信息
        public List<String> getBNSomeINFO(String BookNO)
        {
            List<String> result = new List<String>();
            try
            {
                //测试在后台打印    		


                String sql = "select orderbook.B_Num,book.ISBN,book.B_Name,book.B_Author,book.B_Publishment,bdetailedinformation.Borrowed from orderreport,book,bdetailedinformation where orderbook.B_Num=bdetailedinformation.B_Num AND bdetailedinformation.ISBN=book.ISBN And orderbook.B_Num='" + BookNO + "'";
                SqlCommand command = new SqlCommand(sql, sqlCon);
                SqlDataReader rs = command.ExecuteReader();
                while (rs.Read())
                {//将结果集信息添加到返回向量中								
                 //String[] middle=new String[6];
                    result.Add(rs[0].ToString());
                    result.Add(rs[1].ToString());
                    result.Add(rs[2].ToString());
                    result.Add(rs[3].ToString());
                    result.Add(rs[4].ToString());
                    result.Add(rs[5].ToString());
                }
                rs.Close();
                command.Dispose();
            }

            catch (Exception e)
            {

            }
            return result;

        }

        String GetBookName(string BNUM)
        {

            //测试在后台打印    		

            String S1 = "555";
            String sql = "select Book.B_Name from bdetailedinformation, Book where bdetailedinformation.ISBN =Book.ISBN and bdetailedinformation.B_NUM = '" + BNUM + "'";
            SqlCommand command = new SqlCommand(sql, sqlCon);
            SqlDataReader rs = command.ExecuteReader();

            while (rs.Read())
            {//将结果集信息添加到返回向量中								
             //String[] middle=new String[6];
                S1 = rs[0].ToString();

            }
            rs.Close();
            command.Dispose();

            return S1;
        }

        //通过学生的ID得到学生的班级，姓名，学号
        public String[] getIDClNO(String stuno)
        {
            String[] result = new String[3];
            try
            {
                //测试在后台打印    		


                String sql = "select S_Num,S_Class,S_Name from student where S_Num='" + stuno + "'";
                SqlCommand command = new SqlCommand(sql, sqlCon);
                SqlDataReader rs = command.ExecuteReader();
                while (rs.Read())
                {//将结果集信息添加到返回向量中								
                 //String[] middle=new String[6];
                    result[0] = rs[0].ToString();
                    result[1] = rs[1].ToString();
                    result[2] = rs[2].ToString();
                }
                rs.Close();
                command.Dispose();
            }

            catch (Exception e)
            {

            }
            return result;

        }

        //通过书号得到归还时间
        public String gettimefromrecord(String BookNo)
        {
            String result = null;
            try
            {
                //测试在后台打印    		


                String sql = "select ReturnTime from record where B_Num='" + BookNo + "'";
                SqlCommand command = new SqlCommand(sql, sqlCon);
                SqlDataReader rs = command.ExecuteReader();
                while (rs.Read())
                {//将结果集信息添加到返回向量中								
                    result = rs[0].ToString();
                }
                rs.Close();
                command.Dispose();
                ;
            }

            catch (Exception e)
            {

            }
            return result;

        }


        //通过书号判断时候是再借状态
        public String getifBorrow(String BookNO)
        {
            String result = null;
            try
            {
                //测试在后台打印    		


                String sql = "select ReturnTime from record where B_Num='" + BookNO + "'";
                SqlCommand command = new SqlCommand(sql, sqlCon);
                SqlDataReader rs = command.ExecuteReader();
                while (rs.Read())
                {//将结果集信息添加到返回向量中								
                    result = rs[0].ToString();
                }
                rs.Close();
                command.Dispose();
            }

            catch (Exception e)
            {

            }
            return result;

        }

        //通过书号查询预约人
        public String getstu(String BookNO)
        {
            String stu = null;
            try
            {

                stu = "   ";
                String sql = "select S_Num from orderbook where B_Num='" + BookNO + "'";
                SqlCommand command = new SqlCommand(sql, sqlCon);
                SqlDataReader rs = command.ExecuteReader();
                while (rs.Read())
                {
                    stu = rs[0].ToString();
                }
                rs.Close();
                command.Dispose();
            }
            catch (Exception e)
            {

            }
            return stu;
        }
        //通过isbn获得书号
        public String getBookNumber()
        {
            String bookno = null;
            int num = 0;
            String a = null;
            try
            {


                String sql = "select count(B_Num) from bdetailedinformation";
                SqlCommand command = new SqlCommand(sql, sqlCon);
                SqlDataReader rs = command.ExecuteReader();
                while (rs.Read())
                {
                    a = rs[0].ToString();
                }
                num = int.Parse(a) + 1 + 10000;
                bookno = num + "";
                rs.Close();
                command.Dispose();
            }
            catch (Exception e)
            {

            }
            return bookno;
        }
        #endregion


        //以下部分是学生的相关数据库函数
        #region 学生部分函数
        //知道学生的学号得到他的密码
        public String selectPwd(String S_Num)
        {
            String result = null;
            try
            {


                String sql = "select S_Pwd from student where S_Num='" + S_Num + "'";
                SqlCommand command = new SqlCommand(sql, sqlCon);
                SqlDataReader rs = command.ExecuteReader();
                while (rs.Read())
                {
                    result = rs[0].ToString();
                }
                rs.Close();
                command.Dispose();
            }
            catch (Exception e)
            {

            }
            return result;
        }


        //得到挂失图书的信息表中的记录的数量
        public int getMaxLBNO()
        {
            int result = 0;
            try
            {


                String sql = "select MAX(GSBH) from losebook";
                SqlCommand command = new SqlCommand(sql, sqlCon);
                SqlDataReader rs = command.ExecuteReader();
                while (rs.Read())
                {
                    result = int.Parse(rs[0].ToString());
                }
                rs.Close();
                command.Dispose(); ;
            }
            catch (Exception e)
            {

            }
            return result;

        }


        #endregion

    }
}
