//hey//
# dotNet5785_7621_1590
//what's

החלטות בזמן פיתוח:
שלב 5:
בסינון של הרשימת המתנדבים סיינו לפי : ID, תפקיד, פעיל , NOUN.
שלב 6:
במסך מתנדב ראשי - כתוב בעדכון של המתנדב: 
לסמן את עצמו כלא פעיל - בתנאי שהוא לא מטפל בקריאה ברגע זה
וב BO כתוב שרק המנהל יכול לשנות. הוספתי ללוגיקה שבודק אם אין קריאות בטיפול ורק אז יכול למחוק 

הערה:
בכניסת מנהל למערכת עשינו שאי אפשר ששני מנהלים יכנסו במקביל ולכן אם אתה על המערכת נכנסת כמנהל 
למסך מתנדב
ראשי ואחר כך חזרת אחורה ואתה רוצה לפתוח את מסך מנהל ראשי אתה צריך לצאת לגמרי מהמערכת כולל החלון של LOGIN 
כי כך הבנו את הבקשה למרות שהיא תמוהה.



בונוסים שמימשנו:

סיסמא- 2 נקודות 

ססימא חזקה- 1 נקודה - ממומש ב BL , שם הפונק שבודקת: IsStrongPassword בVOLUNTEERMANAGER 

אייקון (icon) בכותרת החלון ובשורת המשימות  - 1 נק

טריגר אירועים- בהוספת קריאה מופיע בלון אחריי שהוספנו- 1 נקודה ///// לבדוק

בהזנת הסיסמא דרך המסך - הסיסמא לא מוצגת אלא מופיעות כוכביות (למשל) - (תוספת סטודנטים - 1 נק')


טריגר תכונות - VolunteerHistoryWindow  הכותרת משתנה כששמים את העכבר על הכותרת - 1 נקודות

נקודה 1
 כשולחצים על הכפתור של לא פעיל במסך-  טריגר נתונים -VolunteerMainWindow ,
 כלומר לעדכן את המתנדב ללא פעיל- הוא הופך לאדום, וכשיש וי זה ירוק	
והTOTALCALLS והצבע משתה לפי כמות הקריאות . 

צורות- כפתור BACK מופיע במסכים - גרפיקה , 1 נקודות : מופיע בחלון VolunteerInList ובחלון  CALLHYSTORY
הצורה של הככפתור מופיע ב APP 

כניסה למערכת כאשר לוחצים על ENTER ולא רק על הLOGINSYSME - נקודה 1

במסך ניהול קריאות - כפתור מחיקה מופיע רק אם ניתן למחוק את המתנדב- 2 נקודות

במסך ניהול קריאות - לקבץ רשימת קריאות לפי סוג קריאה ו-\או לפי סטטוס- 2 נקודות , אפשר לסנן לפי סוג קריאה ולמיין לפי ססטוס 

תצוגה גרפית אינטראקטיבית (שינוי צבעים וכדומה) במקרה של פורמט קלט לא תקין - 1 נקודה: במסך כניסה אם מכניסים משהו שהוא לא INT הCOMBOX הפך לאדום

(בCallDescriptionWindow ניסנוי לממש מפות - לא הצלחנו להתקין למחשב תוכנה שתסדר את זה ...)



בחלון CHOOCHECALLWINDOE 
ControlTemplate - עיצוב מותאם אישית לכפתור בעזרת ControlTemplate, שמספק גבול עם עיצוב מותאם אישית.
-1 נקודה 
Datatriger -אם זה התקף לב זה מסומן באדום



