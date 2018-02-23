using System.Security.Cryptography;
using System.Text;
using LSports.DVS.Framework.DataAccess.Models;
using LSports.Framework.Models.CustomClasses;

namespace LSports.DVS.Framework.DataAccess.CustomClasses
{
    public class ValidationResultsHashModel
    {
        public val_validationresult Result;

        public bool ContainsSingleMarket { get; set; }
        public bool ContainsSingleProvider { get; set; }

        protected string ProtecteEventProtuctdHash = "";

        public string IndexEventProductHash
        {
            get
            {
                if (ProtecteEventProtuctdHash == "")
                {
                    ProtecteEventProtuctdHash = GetEventProductHash(Result.EventId, Result.ProductId, Result.IsActive);
                }

                return ProtecteEventProtuctdHash;
            }
        }

        protected string ProtectedUniqueHash = "";

        public string UniqueHash
        {
            get
            {
                if (ProtectedUniqueHash == "")
                {
                    ProtectedUniqueHash = GetUniqueHash(Result.EventId, Result.ValidationSettingId, Result.ProductId, Result.Market, Result.Provider, ContainsSingleMarket, ContainsSingleProvider);
                }

                return ProtectedUniqueHash;
            }
        }

        public static string GetEventProductHash(long EventId, int ProductId, bool IsActive)
        {
            using (MD5 md5Hasher = MD5.Create())
            {
                byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(EventId + "-" + ProductId + "-" + IsActive));

                StringBuilder sBuilder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                return sBuilder.ToString();
            }
        }

        public static string GetUniqueHash(long EventId, int ValidationSettingId, int ProductId, string Market, string Provider, bool ContainsSingleMarket, bool ContainsSingleProvider)
        {
            using (MD5 md5Hasher = MD5.Create())
            {
                byte[] data;
                if (ContainsSingleMarket)
                {
                    if (ContainsSingleProvider)
                    {
                        data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(EventId + "-" + ValidationSettingId + "-" + ProductId + "-"
                                                        + Market + "-" + Provider));
                    }
                    else
                    {
                        data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(EventId + "-" + ValidationSettingId + "-" + ProductId + "-" + Market));
                    }
                }
                else
                {
                    data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(EventId + "-" + ValidationSettingId + "-" + ProductId));
                }

                StringBuilder sBuilder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                return sBuilder.ToString();//Encoding.Default.GetString(data));
            }
        }
    }
}
