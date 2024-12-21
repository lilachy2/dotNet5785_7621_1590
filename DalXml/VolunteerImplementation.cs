namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

//XElement



internal class VolunteerImplementation : IVolunteer
{
    static Volunteer GetVolunteer(XElement v)
    {
        Volunteer s= new DO.Volunteer()
        {
            Id = int.TryParse((string?)v.Element("Id"), out var ID) ? ID : throw new FormatException("can't convert id"),
            Name = (string?)v.Element("Name") ?? "",
            Number_phone = (string?)v.Element("Number") ?? "", 
            Email = (string?)v.Element("Email") ?? "", 
            Password = (string?)v.Element("Password") ?? "",
            Active = bool.TryParse((string?)v.Element("Active"), out bool active) ? active : throw new FormatException("can't convert active"),
            Role = Role.TryParse((string?)v.Element("Role"), out Role role) ? role : throw new FormatException("can't convert role "),
            FullCurrentAddress = (string?)v.Element("FullCurrentAddress") ?? null,
            Longitude = double.TryParse((string?)v.Element("longitude"), out double longitude) ? longitude : null,
            Latitude = double.TryParse((string?)v.Element("latitude"), out double latitude) ? latitude : null,
            distance = double.TryParse((string?)v.Element("maxDistance"), out double maxDis) ? maxDis : null,
        };
        return s;
    }
    public void Create(Volunteer item)
    {
        XElement volunteersRoot = XMLTools.LoadListFromXMLElement(Config.s_Volunteers_xml);

        if (volunteersRoot.Elements().Any(v => (int?)v.Element("Id") == item.Id))
            throw new DalAlreadyExistsException($"Volunteer with ID={item.Id} already exists.");

        volunteersRoot.Add(new XElement(CreateVolunteerElement(item)));
        Console.WriteLine("Saving XML data...");
        XMLTools.SaveListToXMLElement(volunteersRoot, Config.s_Volunteers_xml);
        Console.WriteLine("Data saved.");

    }
    private static XElement CreateVolunteerElement(Volunteer v)
    {
        return new XElement("Volunteer",
            new XElement("Id", v.Id),
            new XElement("Name", v.Name),
            new XElement("Number_phone", v.Number_phone),
            new XElement("Email", v.Email),
            new XElement("Password", v.Password),
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
    public Volunteer? Read(int id)
    {
        XElement? studentElem =
    XMLTools.LoadListFromXMLElement(Config.s_Volunteers_xml).Elements().FirstOrDefault(st => (int?)st.Element("Id") == id);
        return studentElem is null ? null : GetVolunteer(studentElem);
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

        XElement? volunteerElem = volunteersRoot.Elements().FirstOrDefault(v => (int?)v.Element("Id") == item.Id)
                                 ?? throw new DalAlreadyExistsException($"Volunteer with ID={item.Id} does not exist.");

        volunteerElem.Remove();

        volunteersRoot.Add("Volunteer", CreateVolunteerElement(item));
        XMLTools.SaveListToXMLElement(volunteersRoot, Config.s_Volunteers_xml);
    }
}