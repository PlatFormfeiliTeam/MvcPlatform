using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Drawing;

namespace MvcPlatform.Common
{
    class Code128C
    {
        private DataTable dt_Code128 = new DataTable();
        private uint m_Height = 35;
        /// <summary>       
        /// 高度       
        /// </summary>       
        public uint Height { get { return m_Height; } set { m_Height = value; } }
        private Font m_ValueFont = null;
        /// <summary>       
        /// 是否显示可见号码 如果为NULL不显示号码      
        /// /// </summary>       
        public Font ValueFont { get { return m_ValueFont; } set { m_ValueFont = value; } }
        private byte m_Magnify = 0;
        /// <summary>       
        /// 放大倍数       
        /// </summary>       
        public byte Magnify { get { return m_Magnify; } set { m_Magnify = value; } }

        public Bitmap GetCodeImage(string code)
        {

            string ViewText = code;
            string Text = "";
            IList<int> TextNumb = new List<int>();

            int _Examine = 105;
            if (!((code.Length & 1) == 0))
                throw new Exception("128C长度必须是偶数");

            while (code.Length != 0)
            {
                int _Temp = 0;
                try
                {
                    int _CodeNumb128 = Int32.Parse(code.Substring(0, 2));
                }
                catch
                {
                    throw new Exception("128C必须是数字！");
                }
                Text += GetValue(code.Substring(0, 2), ref _Temp);
                TextNumb.Add(_Temp);
                code = code.Remove(0, 2);
            }

            if (TextNumb.Count == 0)
                throw new Exception("错误的编码,无数据");
            Text = Text.Insert(0, GetValue(_Examine)); //获取开始位         
            for (int i = 0; i != TextNumb.Count; i++)
            {
                _Examine += TextNumb[i] * (i + 1);
            }
            _Examine = _Examine % 103;
            //获得严效位       
            Text += GetValue(_Examine);
            //获取严效位        
            Text += "2331112";
            //结束位       
            Bitmap _CodeImage = GetImage(Text);
            return _CodeImage;

        }

        private Bitmap GetImage(string p_Text)
        {
            char[] _Value = p_Text.ToCharArray();
            int _Width = 0;
            for (int i = 0; i != _Value.Length; i++)
            {
                _Width += Int32.Parse(_Value[i].ToString()) * (m_Magnify + 1);
            }
            Bitmap _CodeImage = new Bitmap(_Width, (int)m_Height);
            Graphics _Garphics = Graphics.FromImage(_CodeImage);
            //Pen _Pen;        
            int _LenEx = 0;
            for (int i = 0; i != _Value.Length; i++)
            {
                int _ValueNumb = Int32.Parse(_Value[i].ToString()) * (m_Magnify + 1); //获取宽和放大系数     
                if (!((i & 1) == 0))
                {
                    //_Pen = new Pen(Brushes.White, _ValueNumb);             
                    _Garphics.FillRectangle(Brushes.White, new Rectangle(_LenEx, 0, _ValueNumb, (int)m_Height));
                }
                else
                {
                    //_Pen = new Pen(Brushes.Black, _ValueNumb);           
                    _Garphics.FillRectangle(Brushes.Black, new Rectangle(_LenEx, 0, _ValueNumb, (int)m_Height));
                }
                //_Garphics.(_Pen, new Point(_LenEx, 0), new Point(_LenEx, m_Height));          
                _LenEx += _ValueNumb;
            }
            _Garphics.Dispose();
            return _CodeImage;
        }

        private string GetValue(int p_CodeId)
        {
            DataRow[] dr = dt_Code128.Select("ID='" + p_CodeId.ToString() + "'");
            if (dr.Length != 1) throw new Exception("验效位的编码错误" + p_CodeId.ToString());
            return dr[0]["BandCode"].ToString();
        }

        private string GetValue(string p_Value, ref int p_SetID)
        {
            if (dt_Code128 == null)
                return "";
            DataRow[] dr = dt_Code128.Select("Code128C='" + p_Value + "'");
            if (dr.Length != 1)
                throw new Exception("错误的编码" + p_Value.ToString());

            p_SetID = Int32.Parse(dr[0]["ID"].ToString());
            return dr[0]["BandCode"].ToString();
        }

        public Code128C()
        {
            dt_Code128.Columns.Add("ID");
            dt_Code128.Columns.Add("Code128A");
            dt_Code128.Columns.Add("Code128B");
            dt_Code128.Columns.Add("Code128C");
            dt_Code128.Columns.Add("BandCode");
            dt_Code128.CaseSensitive = true;
            #region 数据表
            dt_Code128.Rows.Add("0", " ", " ", "00", "212222");
            dt_Code128.Rows.Add("1", "!", "!", "01", "222122");
            dt_Code128.Rows.Add("2", "\"", "\"", "02", "222221");
            dt_Code128.Rows.Add("3", "#", "#", "03", "121223");
            dt_Code128.Rows.Add("4", "$", "$", "04", "121322");
            dt_Code128.Rows.Add("5", "%", "%", "05", "131222");
            dt_Code128.Rows.Add("6", "&", "&", "06", "122213");
            dt_Code128.Rows.Add("7", "'", "'", "07", "122312");
            dt_Code128.Rows.Add("8", "(", "(", "08", "132212");
            dt_Code128.Rows.Add("9", ")", ")", "09", "221213");
            dt_Code128.Rows.Add("10", "*", "*", "10", "221312");
            dt_Code128.Rows.Add("11", "+", "+", "11", "231212");
            dt_Code128.Rows.Add("12", ",", ",", "12", "112232");
            dt_Code128.Rows.Add("13", "-", "-", "13", "122132");
            dt_Code128.Rows.Add("14", ".", ".", "14", "122231");
            dt_Code128.Rows.Add("15", "/", "/", "15", "113222");
            dt_Code128.Rows.Add("16", "0", "0", "16", "123122");
            dt_Code128.Rows.Add("17", "1", "1", "17", "123221");
            dt_Code128.Rows.Add("18", "2", "2", "18", "223211");
            dt_Code128.Rows.Add("19", "3", "3", "19", "221132");
            dt_Code128.Rows.Add("20", "4", "4", "20", "221231");
            dt_Code128.Rows.Add("21", "5", "5", "21", "213212");
            dt_Code128.Rows.Add("22", "6", "6", "22", "223112");
            dt_Code128.Rows.Add("23", "7", "7", "23", "312131");
            dt_Code128.Rows.Add("24", "8", "8", "24", "311222");
            dt_Code128.Rows.Add("25", "9", "9", "25", "321122");
            dt_Code128.Rows.Add("26", ":", ":", "26", "321221");
            dt_Code128.Rows.Add("27", ";", ";", "27", "312212");
            dt_Code128.Rows.Add("28", "<", "<", "28", "322112");
            dt_Code128.Rows.Add("29", "=", "=", "29", "322211");
            dt_Code128.Rows.Add("30", ">", ">", "30", "212123");
            dt_Code128.Rows.Add("31", "?", "?", "31", "212321");
            dt_Code128.Rows.Add("32", "@", "@", "32", "232121");
            dt_Code128.Rows.Add("33", "A", "A", "33", "111323");
            dt_Code128.Rows.Add("34", "B", "B", "34", "131123");
            dt_Code128.Rows.Add("35", "C", "C", "35", "131321");
            dt_Code128.Rows.Add("36", "D", "D", "36", "112313");
            dt_Code128.Rows.Add("37", "E", "E", "37", "132113");
            dt_Code128.Rows.Add("38", "F", "F", "38", "132311");
            dt_Code128.Rows.Add("39", "G", "G", "39", "211313");
            dt_Code128.Rows.Add("40", "H", "H", "40", "231113");
            dt_Code128.Rows.Add("41", "I", "I", "41", "231311");
            dt_Code128.Rows.Add("42", "J", "J", "42", "112133");
            dt_Code128.Rows.Add("43", "K", "K", "43", "112331");
            dt_Code128.Rows.Add("44", "L", "L", "44", "132131");
            dt_Code128.Rows.Add("45", "M", "M", "45", "113123");
            dt_Code128.Rows.Add("46", "N", "N", "46", "113321");
            dt_Code128.Rows.Add("47", "O", "O", "47", "133121");
            dt_Code128.Rows.Add("48", "P", "P", "48", "313121");
            dt_Code128.Rows.Add("49", "Q", "Q", "49", "211331");
            dt_Code128.Rows.Add("50", "R", "R", "50", "231131");
            dt_Code128.Rows.Add("51", "S", "S", "51", "213113");
            dt_Code128.Rows.Add("52", "T", "T", "52", "213311");
            dt_Code128.Rows.Add("53", "U", "U", "53", "213131");
            dt_Code128.Rows.Add("54", "V", "V", "54", "311123");
            dt_Code128.Rows.Add("55", "W", "W", "55", "311321");
            dt_Code128.Rows.Add("56", "X", "X", "56", "331121");
            dt_Code128.Rows.Add("57", "Y", "Y", "57", "312113");
            dt_Code128.Rows.Add("58", "Z", "Z", "58", "312311");
            dt_Code128.Rows.Add("59", "[", "[", "59", "332111");
            dt_Code128.Rows.Add("60", "\\", "\\", "60", "314111");
            dt_Code128.Rows.Add("61", "]", "]", "61", "221411");
            dt_Code128.Rows.Add("62", "^", "^", "62", "431111");
            dt_Code128.Rows.Add("63", "_", "_", "63", "111224");
            dt_Code128.Rows.Add("64", "NUL", "`", "64", "111422");
            dt_Code128.Rows.Add("65", "SOH", "a", "65", "121124");
            dt_Code128.Rows.Add("66", "STX", "b", "66", "121421");
            dt_Code128.Rows.Add("67", "ETX", "c", "67", "141122");
            dt_Code128.Rows.Add("68", "EOT", "d", "68", "141221");
            dt_Code128.Rows.Add("69", "ENQ", "e", "69", "112214");
            dt_Code128.Rows.Add("70", "ACK", "f", "70", "112412");
            dt_Code128.Rows.Add("71", "BEL", "g", "71", "122114");
            dt_Code128.Rows.Add("72", "BS", "h", "72", "122411");
            dt_Code128.Rows.Add("73", "HT", "i", "73", "142112");
            dt_Code128.Rows.Add("74", "LF", "j", "74", "142211");
            dt_Code128.Rows.Add("75", "VT", "k", "75", "241211");
            dt_Code128.Rows.Add("76", "FF", "I", "76", "221114");
            dt_Code128.Rows.Add("77", "CR", "m", "77", "413111");
            dt_Code128.Rows.Add("78", "SO", "n", "78", "241112");
            dt_Code128.Rows.Add("79", "SI", "o", "79", "134111");
            dt_Code128.Rows.Add("80", "DLE", "p", "80", "111242");
            dt_Code128.Rows.Add("81", "DC1", "q", "81", "121142");
            dt_Code128.Rows.Add("82", "DC2", "r", "82", "121241");
            dt_Code128.Rows.Add("83", "DC3", "s", "83", "114212");
            dt_Code128.Rows.Add("84", "DC4", "t", "84", "124112");
            dt_Code128.Rows.Add("85", "NAK", "u", "85", "124211");
            dt_Code128.Rows.Add("86", "SYN", "v", "86", "411212");
            dt_Code128.Rows.Add("87", "ETB", "w", "87", "421112");
            dt_Code128.Rows.Add("88", "CAN", "x", "88", "421211");
            dt_Code128.Rows.Add("89", "EM", "y", "89", "212141");
            dt_Code128.Rows.Add("90", "SUB", "z", "90", "214121");
            dt_Code128.Rows.Add("91", "ESC", "{", "91", "412121");
            dt_Code128.Rows.Add("92", "FS", "|", "92", "111143");
            dt_Code128.Rows.Add("93", "GS", "}", "93", "111341");
            dt_Code128.Rows.Add("94", "RS", "~", "94", "131141");
            dt_Code128.Rows.Add("95", "US", "DEL", "95", "114113");
            dt_Code128.Rows.Add("96", "FNC3", "FNC3", "96", "114311");
            dt_Code128.Rows.Add("97", "FNC2", "FNC2", "97", "411113");
            dt_Code128.Rows.Add("98", "SHIFT", "SHIFT", "98", "411311");
            dt_Code128.Rows.Add("99", "CODEC", "CODEC", "99", "113141");
            dt_Code128.Rows.Add("100", "CODEB", "FNC4", "CODEB", "114131");
            dt_Code128.Rows.Add("101", "FNC4", "CODEA", "CODEA", "311141");
            dt_Code128.Rows.Add("102", "FNC1", "FNC1", "FNC1", "411131");
            dt_Code128.Rows.Add("103", "StartA", "StartA", "StartA", "211412");
            dt_Code128.Rows.Add("104", "StartB", "StartB", "StartB", "211214");
            dt_Code128.Rows.Add("105", "StartC", "StartC", "StartC", "211232");
            dt_Code128.Rows.Add("106", "Stop", "Stop", "Stop", "2331112");
            #endregion
        }



    }
}
