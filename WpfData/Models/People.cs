using SimpleMvvmToolkit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfData.Models
{
    public static class People
    {
        public static ObservableCollection<Person> GetPeople()
        {
            List<Person> result = new List<Person>();
            String connString = ConfigurationManager.ConnectionStrings["ConnString"].ToString();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnString"].ToString()))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("select * from Person", conn))
                {
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            Person person = new Person();
                            person.UserId = Convert.ToInt32(rdr["UserId"]);
                            person.FirstName = rdr["FirstName"].ToString();
                            person.LastName = rdr["LastName"].ToString();
                            person.Birthdate = (DateTime)rdr["Birthdate"];
                            person.Birthplace = rdr["Birthplace"].ToString();
                            person.Gender = rdr["Gender"].ToString().ToCharArray()[0];
                            person.Employed = rdr.GetBoolean(rdr.GetOrdinal("Employed"));
                            person.Phone = rdr["Phone"].ToString();
                            person.MartialStatus = rdr["MartialStatus"].ToString();
                            person.Mode = emMode.none;
                            result.Add(person);
                        }
                    }
                }
                conn.Close();
            }

            var oc = new ObservableCollection<Person>();
            result.ForEach(x => oc.Add(x));
            return oc;
        }

        internal static void Flush(ObservableCollection<Person> people)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnString"].ToString()))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();

                try
                {
                    //ObservableCollection<Person> list = products.Distinct(x => x.Mode != emMode.none);
                    foreach (Person person in people)
                    {
                        if (person.Mode == emMode.none)
                            continue;
                        else
                            if (person.Mode == emMode.add)
                                Insert(person, conn, tran);
                            else
                                if (person.Mode == emMode.update)
                                    Update(person, conn, tran);
                                else if (person.Mode == emMode.delete)
                                    Delete(person.UserId, conn, tran);
                    }
                    tran.Commit();
                }
                catch (Exception e)
                {
                    tran.Rollback();
                    throw e;
                }
                conn.Close();
            }
        }

        public static void Delete(int id, SqlConnection conn, SqlTransaction tran)
        {
            using (SqlCommand cmd = new SqlCommand("delete from Person where UserId = @id", conn, tran))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        private static void Update(Person person, SqlConnection conn, SqlTransaction tran)
        {
            using (SqlCommand cmd = new SqlCommand("update Person set FirstName=@FirstName, LastName=@LastName, Birthplace=@Birthplace, Birthdate=@Birthdate, Employed=@Employed, Phone=@Phone, MartialStatus=@MartialStatus, Gender=@Gender where UserId = @id", conn, tran))
            {
                cmd.Parameters.AddWithValue("@id", person.UserId);
                cmd.Parameters.AddWithValue("@FirstName", person.FirstName);
                cmd.Parameters.AddWithValue("@LastName", person.LastName);
                cmd.Parameters.AddWithValue("@Birthplace", person.Birthplace);
                cmd.Parameters.AddWithValue("@Birthdate", person.Birthdate);
                cmd.Parameters.AddWithValue("@Employed", person.Employed);
                cmd.Parameters.AddWithValue("@Phone", person.Phone);
                cmd.Parameters.AddWithValue("@MartialStatus", person.MartialStatus);
                cmd.Parameters.AddWithValue("@Gender", person.Gender);
                cmd.ExecuteNonQuery();
            }
        }

        private static void Insert(Person person, SqlConnection conn, SqlTransaction tran)
        {
            using (SqlCommand cmd = new SqlCommand("insert into Person(FirstName, LastName, Birthplace, Birthdate, Employed, Phone, MartialStatus, Gender) values (@FirstName, @LastName, @Birthplace, @Birthdate, @Employed, @Phone, @MartialStatus, @Gender)", conn, tran))
            {
                cmd.Parameters.AddWithValue("@FirstName", person.FirstName);
                cmd.Parameters.AddWithValue("@LastName", person.LastName);
                cmd.Parameters.AddWithValue("@Birthplace", person.Birthplace);
                cmd.Parameters.AddWithValue("@Birthdate", person.Birthdate);
                cmd.Parameters.AddWithValue("@Employed", person.Employed);
                cmd.Parameters.AddWithValue("@Phone", person.Phone);
                cmd.Parameters.AddWithValue("@MartialStatus", person.MartialStatus);
                cmd.Parameters.AddWithValue("@Gender", person.Gender);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public class Person : ModelBase<Person>
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Birthplace { get; set; }
        public DateTime Birthdate { get; set; }
        public string MartialStatus { get; set; }
        public bool Employed { get; set; }
        public char Gender { get; set; }
        public string Phone { get; set; }
        public emMode Mode { get; set; }
    }
    public enum emMode { add, update, delete, none };
}
