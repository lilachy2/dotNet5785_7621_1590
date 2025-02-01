namespace DalApi;

using System.Collections.Generic;
using System.Xml.Linq; // מרחב שמות System.Xml.Linq כולל כלים לטיפול בקבצי xml וב-DOM XML.

static class DalConfig
{
    /// <summary>
    /// internal PDS class
    /// DalConfig שמטפלת בהגדרות הקשר (Configuration) של מערכת, שמבוססת על XML.
    /// הוא מבצע טעינה של קובץ הגדרות XML ומבצע סריקת נתונים מתוך הקובץ
    /// </summary>
    internal record DalImplementation
    (string Package,   // package/dll name
     string Namespace, // namespace where DAL implementation class is contained in
     string Class   // DAL implementation class name
    );

    internal static string s_dalName;
    internal static Dictionary<string, DalImplementation> s_dalPackages;

    static DalConfig()
    {
        XElement dalConfig = XElement.Load(@"..\xml\dal-config.xml") ??
  throw new DalConfigException("dal-config.xml file is not found");

        s_dalName =
           dalConfig.Element("dal")?.Value ?? throw new DalConfigException("<dal> element is missing");
        //אם האלמנט dal בקובץ נראה כך:  <dal>list</dal>, אזי המחרוזת שתחזור לתוך s_dalName תהיה list
        //ואםהאלמנט dal בקובץ נראה כך:  < dal > xml </ dal >, אזי המחרוזת שתחזור לתוך s_dalName תהיה xml


                var packages = dalConfig.Element("dal-packages")?.Elements() ??
  throw new DalConfigException("<dal-packages> element is missing");
        s_dalPackages = (from item in packages
                         let pkg = item.Value
                         let ns = item.Attribute("namespace")?.Value ?? "Dal"
                         let cls = item.Attribute("class")?.Value ?? pkg
                         select (item.Name, new DalImplementation(pkg, ns, cls))
                        ).ToDictionary(p => "" + p.Name, p => p.Item2);

        //        מחזיר אוסף של כל האלמנטים שהם "בניו" של אלמנט בשם "dal-packages" בעץ של DOM, זאת אומרת כל תת-האלמנטים של אלמנט<dal-packages >.כלומר:
        //< list > DalList </ list >
        //< xml > DalXml </ xml >


        // ToDictionary---- > בונה ומחזירה טבלה מגובבת [hash table]
        // ("מילון") ששם תת-האלמנט הוא מפתח גיבוב, וערך האלמנט הינו הערך הַמֻּצְמָד למפתח הגיבוב
    }
}

[Serializable]
public class DalConfigException : Exception
{
    public DalConfigException(string msg) : base(msg) { }
    public DalConfigException(string msg, Exception ex) : base(msg, ex) { }
}
