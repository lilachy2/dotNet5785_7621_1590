namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;


/// <param name="Create"> //Creates new entity object in DAL
/// <param name="Delete(int id)"> //Deletes an object by its Id
/// <param name="DeleteAll()"> //Delete all entity objects
/// <param name="Read(Func<Assignment, bool> filter)"> //Reads entity object by a filter function
/// <param name="ReadAll(Func<Assignment, bool>? filter)"> //Reads all entity objects, with optional filter
/// <param name="Update(Assignment item)"> //Updates an existing entity object
internal class AssignmentImplementation : IAssignment
{
    [MethodImpl(MethodImplOptions.Synchronized)]

    public void Create(Assignment item)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_Assignments_xml);
        int newId1 = Config.NextAssignmentID;
        Assignment newItem = item with { Id = newId1};
        Assignments.Add(newItem);
        XMLTools.SaveListToXMLSerializer(Assignments, Config.s_Assignments_xml);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_Assignments_xml);
        if (Assignments.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Course with ID={id} does Not exist");
        XMLTools.SaveListToXMLSerializer(Assignments, Config.s_Assignments_xml);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]

    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Assignment>(), Config.s_Assignments_xml);
    }


    [MethodImpl(MethodImplOptions.Synchronized)]
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_Assignments_xml);
        var v = Assignments.FirstOrDefault(filter);
        return v;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Assignment? Read(int id)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_Assignments_xml);
        var v = Assignments.FirstOrDefault(item => item.Id == id);
        return v;
    }


    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_Assignments_xml);
       return filter == null
          ? Assignments.Select(item => item)
          : Assignments.Where(filter);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Assignment item)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_Assignments_xml);
        if (Assignments.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Course with ID={item.Id} does Not exist");
        Assignments.Add(item);
        XMLTools.SaveListToXMLSerializer(Assignments, Config.s_Assignments_xml);
    }
}
