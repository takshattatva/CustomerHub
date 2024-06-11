using CustomerHub.DAL.Models;
using CustomerHub.DAL.ViewModels;

namespace CustomerHub.BAL.Interface
{
    public interface ICustomerDashboard
    {
        #region -*- Add Customer -*-

        Task<bool> UpdateCustomerDataAsync(Details details);

        #endregion


        #region -*- Delete Customer/s -*-

        public Task DeleteCustomers(List<int> acId);

        #endregion


        #region -*- Edit Customer -*-

        Task<Details> GetEditCustomerDataAsync(int acId);


        #endregion


        #region -*- Get Customersss -*-

        List<CustomerList> GetFilteredCutomer(RequestDTO<CustomerList> requestDTO);

        #endregion


        #region -*- Customer Supplier Group -*-

        List<SupplierGroup> GetGroupsOfCustomer(int CustomerId);

        Task<bool> UpdateCustomerGroupDataAsync(CustomerGroupDTO details);

        Task<bool> DeleteCustomerGroupDataAsync(int groupId);

        Task<bool> AssignCustomerGroupDataAsync(int supplierGroupId);

        #endregion


        #region -*- Contact Customers -*-

        CustomerContactDTO FetchCustomerContact(int acId, int contactId);

        List<ContactDetail> GetAllContactOfCustomer(int acId);

        Task<bool> UpdateCustomerContactDataAsync(CustomerContactDTO details);

        Task<bool> DeleteCustomerContactDataAsync(int contactId);

        #endregion


        #region -*- Check Entity -*- 

        bool CheckEntiry(string input, int callId, int customerId);

        #endregion
    }
}