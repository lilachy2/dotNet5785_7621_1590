using DO;

//namespace Dal;
//internal static class Config
//    // כל אחד שונה ? ההגדרה של ההתחלה של המספר המזהה הרץ
//    // אני די בטוחה שאת הערות לא צריך
//    // לבדוק את האתחול 
//    // הערות וכו 
//{
//    // Call

//    internal const int StartCallId = 1000;
//    private static int nextCallId = StartCallId;
//    internal static int NextCalld { get => nextCallId++; }

//    //// Additional features to read
//    //internal static string Description { get; set; } = string.Empty;
//    //internal static string Address { get; set; } = "Default Address";
//    //internal static double Latitude { get; set; } = 0.0;
//    //internal static double Longitude { get; set; } = 0.0;
//    //internal static DateTime OpeningTime { get; set; } = DateTime.Now;
//    //internal static DateTime? MaxEndTime { get; set; } = null;


//    // Assignment id
//    internal const int start_Assignment_Id = 1000;
//    private static int next_Assignment_Id = start_Assignment_Id;
//    internal static int Next_Assignment_Id { get => next_Assignment_Id++; }    


//    //// משתני תצורה נוספים להקצאה
//    //internal static int CallId { get; set; }
//    //internal static int VolunteerId { get; set; }
//    //internal static DateTime EntryTime { get; set; }/* = Clock*/
//    //internal static DateTime? ActualCompletionTime { get; set; } = null;

//    // Assignment CallId

//    //internal const int start_Assignment_CallId = 1000;
//    //private static int next_Assignment_CallId = start_Assignment_CallId;
//    //internal static int Next_Assignment_CallId { get => next_Assignment_CallId++; }


//    // configuration
//    //configuration NextCallId

//    internal const int start_NextCallId = 1000;
//    private static int next_NextCallId = start_NextCallId;
//    internal static int Next_NextCallId { get => next_NextCallId++; }

//    //configuration Clock
//    internal static DateTime Clock { get; set; }
//    //configuration RiskRange
//    internal static DateTime RiskRange { get; set; } 

//     //configuration NextAssignmentId

//    internal const int start_NextAssignmentId = 1000;
//    private static int next_NextAssignmentId = start_NextAssignmentId;
//    internal static int Next_NextAssignmentId { get => next_NextAssignmentId++; }

//    internal static void Reset()
//    {

//        next_NextAssignmentId = start_NextAssignmentId;
//        next_NextCallId = start_NextCallId;


//        nextCallId = StartCallId;
//        Description = string.Empty;
//        Address = "Default Address";
//        Latitude = 0.0;
//        Longitude = 0.0;
//        OpeningTime = DateTime.Now;
//        MaxEndTime = null;
//        Clock = DateTime.Now;
//        RiskRange = DateTime.Now.AddHours(1);

//        // Assignment
//        next_Assignment_CallId = start_Assignment_CallId;
//        CallId = 0;
//        VolunteerId = 0;
//        EntryTime = Clock;
//        ActualCompletionTime = null;
//        CompletionType = null;
//    }
//}

namespace Dal;

internal static class Config
{
    // הגדרת מזהה רץ לישות קריאה
    internal const int startCallId = 1000;
    private static int nextCallId = startCallId;
    internal static int NextCallId { get => nextCallId++; }

    // הגדרת מזהה רץ לישות מתנדב
    internal const int startVolunteerId = 1000;

    // הגדרת מזהה רץ לישות הקצאה
    internal const int startAssignmentId = 1000;
    private static int nextAssignmentId = startAssignmentId;
    internal static int NextAssignmentId { get => nextAssignmentId++; }

    // משתנים נוספים בהתאם למשתני התצורה של המערכת
    internal static DateTime Clock { get; set; } = DateTime.Now;

    // "זמן סיכון" עבור קריאות מתקרבות לזמן סיום
    internal static TimeSpan RiskRange { get; set; } = TimeSpan.FromHours(1);

    // פונקציה לאיפוס הערכים להתחלתיים
    internal static void Reset()
    {
        nextCallId = startCallId;
        nextAssignmentId = startAssignmentId;

        // משתני תצורה נוספים לאיפוס
        Clock = DateTime.Now;
        RiskRange = TimeSpan.FromHours(1);
    }
}
