namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

//XElement



internal class VolunteerImplementation : IVolunteer
{
    static Volunteer GetVolunteer(XElement v)
    {
        return new Volunteer()
        {
             id = v.ToIntNullable("Id") ?? throw new FormatException("Invalid ID format."),
            // id = (int)v.Element("Id"),  // אם אתה בטוח שהערך תמיד קיים כ- int
            //id = (int?)v.Element("id") ?? throw new FormatException("ID element is missing or invalid."),

            Name = (string?)v.Element("Name") ?? "",
            Number_phone = v.ToIntNullable("Number_phone") ?? throw new FormatException("Invalid phone number format."),
            Email = (string?)v.Element("Email") ?? "",

            Role = Enum.Parse<Role>((string?)v.Element("Role") ?? Role.Volunteer.ToString()),
            Distance_Type = Enum.Parse<distance_type>((string?)v.Element("Distance_Type") ?? distance_type.Aerial_distance.ToString()),

            //Role = v.ToEnumNullable<Role>("Role") ?? Role.Volunteer,
            //Distance_Type = v.ToEnumNullable<distance_type>("Distance_Type") ?? distance_type.Aerial_distance,
            FullCurrentAddress = (string?)v.Element("FullCurrentAddress"),
            Latitude = v.ToDoubleNullable("Latitude"),
            Longitude = v.ToDoubleNullable("Longitude"),
            Active = (bool?)v.Element("Active") ?? true,
            distance = v.ToDoubleNullable("distance")
        };
    }
    public void Create(Volunteer item)
    {
        XElement volunteersRoot = XMLTools.LoadListFromXMLElement(Config.s_Volunteers_xml);

        if (volunteersRoot.Elements().Any(v => (int?)v.Element("Id") == item.id))
            throw new DalAlreadyExistsException($"Volunteer with ID={item.id} already exists.");

        volunteersRoot.Add(new XElement("Volunteers", CreateVolunteerElement(item)));
        Console.WriteLine("Saving XML data...");
        XMLTools.SaveListToXMLElement(volunteersRoot, Config.s_Volunteers_xml);
        Console.WriteLine("Data saved.");

    }
    private static XElement CreateVolunteerElement(Volunteer v)
    {
        return new XElement("Volunteer",
            new XElement("Id", v.id),
            new XElement("Name", v.Name),
            new XElement("Number_phone", v.Number_phone),
            new XElement("Email", v.Email),
            new XElement("Role", v.Role),
            new XElement("Distance_Type", v.Distance_Type),
            new XElement("FullCurrentAddress", v.FullCurrentAddress),
            new XElement("Latitude", v.Latitude),
            new XElement("Longitude", v.Longitude),
            new XElement("Active", v.Active),
            new XElement("distance", v.distance)
        );
    }

    public void Delete(int id)
    {
        XElement volunteersRoot = XMLTools.LoadListFromXMLElement(Config.s_Volunteers_xml);

        XElement? volunteerElem = volunteersRoot.Elements()
                                                 .FirstOrDefault(v => (int?)v.Element("Id") == id)
                             ?? throw new DalDoesNotExistException($"Volunteer with ID={id} does not exist.");

        volunteerElem.Remove();

        XMLTools.SaveListToXMLElement(volunteersRoot, Config.s_Volunteers_xml);
    }

    public void DeleteAll()
    {
        XElement volunteersRoot = XMLTools.LoadListFromXMLElement(Config.s_Volunteers_xml);

        volunteersRoot.RemoveAll();

        XMLTools.SaveListToXMLElement(volunteersRoot, Config.s_Volunteers_xml);
    }

    public Volunteer? Read(Func<Volunteer, bool> filter)
    
    {
        return XMLTools.LoadListFromXMLElement(Config.s_Volunteers_xml)
                                  .Elements()
                                  .Select(v => GetVolunteer(v))
                                  .FirstOrDefault(filter);
    }

    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        var volunteers = XMLTools.LoadListFromXMLElement(Config.s_Volunteers_xml)
                                            .Elements()
                                            .Select(v => GetVolunteer(v));
        return filter == null ? volunteers : volunteers.Where(filter);
    }

    public void Update(Volunteer item)
    {
        XElement volunteersRoot = XMLTools.LoadListFromXMLElement(Config.s_Volunteers_xml);

        XElement? volunteerElem = volunteersRoot.Elements().FirstOrDefault(v => (int?)v.Element("Id") == item.id)
                                 ?? throw new DalAlreadyExistsException($"Volunteer with ID={item.id} does not exist.");

        volunteerElem.Remove();

        volunteersRoot.Add(new XElement("Volunteer", CreateVolunteerElement(item)));
        XMLTools.SaveListToXMLElement(volunteersRoot, Config.s_Volunteers_xml);
    }
}
