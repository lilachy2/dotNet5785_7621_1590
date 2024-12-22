
namespace BlApi;
/// <summary>Interface for logical service 'Call'</summary>
/// <param name="GetCallStatusesCounts">Method to request counts of calls by status</param>
/// <param name="callInList">Method to request a filtered and sorted list of calls</param>
/// <param name="Read">Method to request details of a specific call by ID</param>
/// <param name="Update">Method to update the details of a specific call</param>
/// <param name="Delete">Method to delete a call by its ID</param>
/// <param name="Create">Method to create a new call</param>
/// <param name="GetCloseCall">Method to get a list of closed calls handled by a specific volunteer</param>
/// <param name="GetOpenCall">Method to get a list of open calls available for selection by a volunteer</param>
/// <param name="UpdateEndTreatment">Method to update the completion of treatment for a call</param>
/// <param name="UpdateCancelTreatment">Method to update the cancellation of treatment for a call</param>
/// <param name="ChooseCall">Method for a volunteer to choose a call</param>
public interface ICall : IObservable //stage 5
{
    int[] GetCallStatusesCounts();
    public IEnumerable<BO.CallInList> GetCallsList(BO.CallInListField? filterField = null,object? filterValue = null,BO.CallInListField? sortField = null);

    BO.Call? Read(int id);
    void Update(BO.Call boCall);
    void Delete(int id);
    void Create(BO.Call boCall); 
    List<BO.ClosedCallInList> GetCloseCall(int id, BO.Calltype? callType, BO.ClosedCallInListEnum?  closedCallInListEnum);
    /*List*/
    IEnumerable<BO.OpenCallInList> GetOpenCall(int id, BO.Calltype? callType, BO.OpenCallInListEnum?  openedCallInListEnum);
    void UpdateEndTreatment(int id, int callid);
    void UpdateCancelTreatment(int id, int callid);
    void ChooseCall(int id, int callid);
}
