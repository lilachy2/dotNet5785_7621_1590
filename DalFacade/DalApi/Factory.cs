namespace DalApi;
public static class Factory
{
    // הקוד מיועד למערכת שבה יש צורך להחליף את הגישה לנתונים (DAL) בצורה דינמית, לפי הגדרות שנמצאות בקובץ חיצוני
    // (כמו dal-config.xml).
    // המערכת יכולה להחליט בזמן ריצה איזה DAL (Data Access Layer)
    // להשתמש, מבלי לשנות את הקוד עצמו, פשוט על ידי עדכון הקובץ החיצוני.
    public static IDal Get
    {
        get
        {
            // dalType יכיל בסוף  את המחרוזת "xml" או "list".
            //ניגשים לטבלת גיבוב שנוצרה בDalConfig ומחזירים את הערך המקושר למפתח גיבוב שהוא dalType 
            string dalType = DalApi.DalConfig.s_dalName ?? throw new DalConfigException($"DAL name is not extracted from the configuration");
            DalApi.DalConfig.DalImplementation dal = DalApi.DalConfig.s_dalPackages[dalType] ?? throw new DalConfigException($"Package for {dalType} is not found in packages list in dal-config.xml");

            // dal.Package יכיל את שם ה dll שמכיל את מחלקת המימוש שצריכה להיטען (DalXml.dll or DalList.dll)
            try { System.Reflection.Assembly.Load(dal.Package ?? throw new DalConfigException($"Package {dal.Package} is null")); }
            catch (Exception ex) { throw new DalConfigException($"Failed to load {dal.Package}.dll package", ex); }

            // TYPE יכיל את המחלקה של DalList או DalXml 
            Type type = Type.GetType($"{dal.Namespace}.{dal.Class}, {dal.Package}") ??
                throw new DalConfigException($"Class {dal.Namespace}.{dal.Class} was not found in {dal.Package}.dll");

            // GetProperty - מחזיר מטא דטא של התכונה "Instance" של המחלקה DalXml/DalList שחייבת להיות סטטית ועם הרשאה public
            // GetValue - מחזיר את הערך של התכונה "Instance" שהיא מחלקה סטטית   
            return type.GetProperty("Instance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)?.GetValue(null) as IDal ??
                throw new DalConfigException($"Class {dal.Class} is not a singleton or wrong property name for Instance");
        }
    }
}
