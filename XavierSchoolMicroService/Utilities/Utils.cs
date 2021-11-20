using System;
using System.Collections.Generic;

namespace XavierSchoolMicroService.Utilities
{
    public class Utils
    {
        public static readonly int LENT = 4;
        public static Dictionary<string, int?> BuildDicDormitorio(int? Departamento, int? Piso)
        {
            Dictionary<string, int?> dic = new Dictionary<string, int?>();
            dic.Add("Departamento", Departamento);
            dic.Add("Piso", Piso);
            return dic;
        }

        public static TimeSpan ConvertirHoraToTimeSpan(string hour)
        {
            string[] sep = hour.Split(':');
            int h = int.Parse(sep[0]);
            int m = int.Parse(sep[1]);

            TimeSpan timeSpan = new DateTime(2010, 1, 1, h, m, 0)  - new DateTime(2010, 1, 1, 0, 0, 0);
            return timeSpan;
        }

        public static string ConvertirTimeSpanToStringHora(TimeSpan? time)
        {
            return time == null ? null : time.Value.ToString(@"hh\:mm");
        }
    }
}