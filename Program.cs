using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace Ass
{
    class CountryData
    {
        public string country_code;
        public float value;
        public string country_name;
        public string indicatorname;
        public string indicatorcode;
        public string year;
    }
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"D:\Indicators.csv";
            string path2 = @"C:\Users\Training\source\repos\stackedchart.json";
            string path3 = @"C:\Users\Training\source\repos\multilinechart.json";
            string path4 = @"C:\Users\Training\source\repos\barchart.json";
            string[] country = { "AFG", "ARM", "AZE", "BHR", "BGD", "BTN", "BRN", "KHM", "CHN", "CXR", "CCK", "IOT","GEO", "HKG", "IND", "IDN", "IRN", "IRQ","ISR", "JPN", "JOR", "KAZ", "KWT","KGZ", "LAO", "LBN", "MAC", "MYS", "MDV", "MNG", "MMR", "NPL","PRK", "OMN", "PAK", "PHL", "QAT", "SAU", "SGP", "KOR", "LKA", "SYR", "TWN", "TJK", "THA", "TUR", "TKM", "ARE", "UZB", "VNM", "YEM"  };
            Console.Write(country.Length);
            List<CountryData> list = new List<CountryData>();
            List<CountryData> firstlist = new List<CountryData>();  
            List<CountryData> lis = new List<CountryData>();
            StreamReader reader = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read));
            StreamWriter writer = new StreamWriter(new FileStream(path2, FileMode.OpenOrCreate, FileAccess.Write));
            StreamWriter writer2 = new StreamWriter(new FileStream(path3, FileMode.OpenOrCreate, FileAccess.Write));
            StreamWriter writer3 = new StreamWriter(new FileStream(path4, FileMode.OpenOrCreate, FileAccess.Write));
            var header = reader.ReadLine().Split(',');
            while (!reader.EndOfStream)
            {
                string[] data = reader.ReadLine().Split(',');
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i].StartsWith("\""))
                    {
                        if (data[i].EndsWith("\"")) { }
                        else
                        {
                            data[i] = data[i] + data[i + 1];
                            data = data.Where((val, idx) => idx != (i + 1)).ToArray();
                        }
                    }
                }
                float abc;
                for (int j = 0; j < country.Length; j++)   //for first json 
                {
                    if (country[j] == data[1])
                    {
                        if (data[3] == "SP.DYN.LE00.FE.IN" || data[3] == "SP.DYN.LE00.MA.IN")
                        {
                            float.TryParse(data[5], out abc);
                            firstlist.Add(new CountryData() { country_code = data[1], indicatorname = data[2],indicatorcode=data[3], year = data[4], value = abc });
                        }
                    }
                }
                if (data[1] == "IND")  //for second json
                {
                    if (data[4] == "SP.DYN.CBRT.IN" || data[4] == "SP.DYN.CDRT.IN")
                    {
                        float.TryParse(data[6], out abc);
                        lis.Add(new CountryData() { year = data[5], value = abc  ,indicatorcode=data[4],country_code=data[1],indicatorname=data[2] , country_name=data[0]});
                    }
                    writer2.Flush();
                }
                if (data[3] == "SP.DYN.LE00.IN")  //for third json
                {
                    float.TryParse(data[5], out abc);
                    list.Add(new CountryData() { country_code = data[1], value = abc, country_name = data[0] });
                }
            }//stacked chart
            var stack = from m in firstlist group new { m.value, m.indicatorcode } by m.country_code into NewG from n in (from m in NewG group new { m.value } by m.indicatorcode into xyz select new { xyz.Key, sum = xyz.Sum(o => o.value) }) group n by NewG.Key;   
            writer.WriteLine("[");
            foreach (var i in stack)
            {
                foreach (var j in i)
                {
                    var r = j.Key == "SP.DYN.LE00.FE.IN" ? "{" + "\"" + "CountryCode" + "\"" + ":" + "\"" + i.Key + "\"" + ",\n" + "\"" + "Life Expectancy Female" + "\"" + ":" + j.sum + "," : "\"" + "Life Expectancy Male" + "\"" + ":" + j.sum;
                    writer.WriteLine(r);
                }
                var res = i.Key =="ISR" ? "}" : "} ,";
                    writer.WriteLine(res);
            }
            writer.WriteLine("]");
            writer.Flush();
            var val1 = from m in lis group new { m.value,m.indicatorcode,m.country_name} by m.year  into xyz select xyz;//multiline
            writer2.WriteLine("{");
            writer2.WriteLine("\""+"India" + "\"" + ":" +"[");
            foreach (var i in val1)
            {
              writer2.WriteLine("{" +"\""+ "year" +"\""+ ":" + i.Key   + ",");
                foreach (var j in i)
                {
                    var r = j.indicatorcode == "SP.DYN.CBRT.IN" ? "\"" + "Birth_Value" + "\"" + ":" + "\"" + j.value + "\"" + "," : "\"" + "Death_Value" + "\"" + ":" + "\"" + j.value + "\"";
                    writer2.WriteLine(r);
                }
                writer2.WriteLine("},");
            }
            writer2.WriteLine("]}");
            writer2.Flush();
            var value3 = from m in list group m by m.country_name into t select new { countryname = t.Key, value = t.Sum(o => o.value) }; //barchart
            var k = value3.OrderByDescending(m => m.value).Take(5);
            writer3.WriteLine("[");
            foreach (var i in k)
            {
                var r = i.countryname == "Norway" ? "{" + "\"" + "countryname" + "\"" + ":" + "\"" + i.countryname + "\"" + "," + "\n" + "\"" + "value" + "\"" + ":" + i.value + "\n" + "}" : "{" + "\"" + "countryname" + "\"" + ":" + "\"" + i.countryname + "\"" + "," + "\n" + "\"" + "value" + "\"" + ":" + i.value + "\n" + "}" + ",";
                writer3.WriteLine(r);
            }
            writer3.WriteLine("]");
            writer3.Flush();
        }
    }
}










