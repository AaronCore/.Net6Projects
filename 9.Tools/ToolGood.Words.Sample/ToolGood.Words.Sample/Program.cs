using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolGood.Words.Pinyin;

namespace ToolGood.Words.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            // 转成简体
            var str = WordsHelper.ToSimplifiedChinese("壹佰贰拾叁億肆仟伍佰陆拾柒萬捌仟玖佰零壹元壹角贰分");
            Console.WriteLine(str);

            // 转成繁体
            var str2 = WordsHelper.ToTraditionalChinese("壹佰贰拾叁亿肆仟伍佰陆拾柒万捌仟玖佰零壹元壹角贰分");
            Console.WriteLine(str2);

            // 转成全角
            var str3 = WordsHelper.ToSBC("abcABC123");
            Console.WriteLine(str3);

            // 转成半角
            var str4 = WordsHelper.ToDBC("ａｂｃＡＢＣ１２３");
            Console.WriteLine(str4);

            // 数字转成中文大写
            var str5 = WordsHelper.ToChineseRMB(12345678901.12);
            Console.WriteLine(str5);

            // 中文转成数字
            var str6 = WordsHelper.ToNumber("壹佰贰拾叁亿肆仟伍佰陆拾柒万捌仟玖佰零壹元壹角贰分");
            Console.WriteLine(str6);

            // 获取全拼
            var str7 = WordsHelper.GetPinyin("我爱中国");
            Console.WriteLine(str7);

            // 获取首字母
            var str8 = WordsHelper.GetFirstPinyin("我爱中国");
            Console.WriteLine(str8);

            // 获取全部拼音
            var str9 = WordsHelper.GetAllPinyin('传');
            Console.WriteLine(str9);

            // 获取姓名
            var str10 = WordsHelper.GetPinyinForName("单一一");
            Console.WriteLine(str10);
            var str11 = WordsHelper.GetPinyinForName("单一一", ",");
            Console.WriteLine(str11);
            var str12 = WordsHelper.GetPinyinForName("单一一", true);
            Console.WriteLine(str12);

            string s = "北京|天津|河北|辽宁|吉林|黑龙江|山东|江苏|上海|浙江|安徽|福建|江西|广东|广西|海南|河南|湖南|湖北|山西|内蒙古|宁夏|青海|陕西|甘肃|新疆|四川|贵州|云南|重庆|西藏|香港|澳门|台湾";
            PinyinMatch match = new PinyinMatch();
            match.SetKeywords(s.Split('|').ToList());
            var all = match.Find("BJ");
            Console.WriteLine(all[0]);
            Console.WriteLine(all.Count);

            all = match.Find("北J");
            Console.WriteLine(all[0]);
            Console.WriteLine(all.Count);
        }
    }
}
