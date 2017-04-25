using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text;

namespace MvcPlatform.Filters
{
    public class DecodeFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
          
            HttpRequestBase req = filterContext.HttpContext.Request;
            string url = req.RawUrl;
            string queryString  = req.QueryString.ToString();

            //queryString = HttpUtility.UrlDecode(queryString);
            //byte[] bpath = Convert.FromBase64String(queryString);
            //queryString = System.Text.ASCIIEncoding.Default.GetString(bpath);

            //queryString = Encrypt(queryString,"12345678");
            queryString = HttpUtility.UrlDecode(queryString);
            queryString = Decrypt(queryString, "12345678");

             string path=req.FilePath;
             NameValueCollection QS = ParseQueryString(queryString);
             ParameterDescriptor[] pds = filterContext.ActionDescriptor.GetParameters();
             filterContext.ActionParameters.Clear();
            
             foreach (var pd in pds)
             {
               string ParameterValue= QS.Get(pd.ParameterName);
               string typeName = pd.ParameterType.Name;
               
               filterContext.ActionParameters.Add(pd.ParameterName,Convert.ChangeType(ParameterValue,pd.ParameterType));
             }


        }


        public static NameValueCollection ParseQueryString(string url)
        {
            NameValueCollection cPar = new NameValueCollection();
            url = url.StartsWith("?") ? url.Substring(1) : url;
            string[] pairs = url.Split(new char[] { '&' });
            int tmpIndex;
            foreach (string pair in pairs)
            {
                if (string.IsNullOrEmpty(pair))
                    continue;
                tmpIndex = pair.IndexOf('=');
                if (tmpIndex == -1)
                    continue;
                cPar.Add(pair.Substring(0, tmpIndex), pair.Substring(tmpIndex + 1));
            }
            return cPar;
        }


        /// <summary>
        /// 进行DES加密。
        /// </summary>
        /// <param name="pToEncrypt">要加密的字符串。</param>
        /// <param name="sKey">密钥，且必须为8位。</param>
        /// <returns>以Base64格式返回的加密字符串。</returns>
        public string Encrypt(string pToEncrypt, string sKey)
        {
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                byte[] inputByteArray = Encoding.UTF8.GetBytes(pToEncrypt);
                des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
                des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    cs.Close();
                }
                string str = Convert.ToBase64String(ms.ToArray());
                ms.Close();
                return str;
            }
        }


        // <summary>
        // 进行DES解密。
        // </summary>
        // <param name="pToDecrypt">要解密的以Base64</param>
        // <param name="sKey">密钥，且必须为8位。</param>
        // <returns>已解密的字符串。</returns>
        public string Decrypt(string pToDecrypt, string sKey)
        {
            byte[] inputByteArray = Convert.FromBase64String(pToDecrypt);
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
                des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    cs.Close();
                }
                string str = Encoding.UTF8.GetString(ms.ToArray());
                ms.Close();
                return str;
            }
        }

		
    }
}