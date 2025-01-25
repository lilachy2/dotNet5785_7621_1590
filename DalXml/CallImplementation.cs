namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

/// <param name="Create"> //Creates new entity object in DAL
/// <param name="Delete(int id)"> //Deletes an object by its Id
/// <param name="DeleteAll()"> //Delete all entity objects
/// <param name="Read(Func<Call, bool> filter)"> //Reads entity object by a filter function
/// <param name="ReadAll(Func<Call, bool>? filter)"> //Reads all entity objects, with optional filter
/// <param name="Update(Call item)"> //Updates an existing entity object
internal class CallImplementation : ICall
{
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Call item)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_Calls_xml);
        int newId = Config.NextCallId;
        Call newItem = item with { Id = newId };
        Calls.Add(newItem);

        XMLTools.SaveListToXMLSerializer(Calls, Config.s_Calls_xml);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_Calls_xml);
        if (Calls.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Course with ID={id} does Not exist");
        XMLTools.SaveListToXMLSerializer(Calls, Config.s_Calls_xml);
    }


    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Call>(), Config.s_Calls_xml);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call? Read(Func<Call, bool> filter)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_Calls_xml);
        var v=  Calls.FirstOrDefault(filter);
        return v;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call? Read(int id)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_Calls_xml);
        var v = Calls.FirstOrDefault(item => item.Id == id);
        return v;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_Calls_xml);

         return filter == null
        ? Calls.Select(item => item)
        : Calls.Where(filter);

    }


    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Call item)
    { 
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_Calls_xml);
        if (Calls.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Course with ID={item.Id} does Not exist");
        Calls.Add(item);
        XMLTools.SaveListToXMLSerializer(Calls, Config.s_Calls_xml);
    }
}
