using CustomerHub.BAL.Interface;
using CustomerHub.DAL.Models;
using CustomerHub.DAL.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Reflection.Emit;
using System.Text.RegularExpressions;

namespace CustomerHub.BAL.Repository
{
    public class CustomerDashboardRepo : ICustomerDashboard
    {
        private readonly CustomerDbContext _context;

        public CustomerDashboardRepo(CustomerDbContext context)
        {
            _context = context;
        }

        #region -*- Update Customer Data -*-

        public async Task<bool> UpdateCustomerDataAsync(Details details)
        {
            try
            {
                CustomerDetail? customer = await _context.CustomerDetails.FirstOrDefaultAsync(x => x.AcId == details.AcId);

                if (customer == null)
                {
                    if (await _context.CustomerDetails.AnyAsync(x => x.AcCode.Trim().ToLower() == details.AcCode.Trim().ToLower() || x.CompanyName.Trim().ToLower() == details.CompanyName.Trim().ToLower()))
                    {
                        return false;
                    }

                    customer = new CustomerDetail();
                    _context.CustomerDetails.Add(customer);
                }
                else
                {
                    if (await _context.CustomerDetails.AnyAsync(x => x.AcCode == details.AcCode && x.AcCode != customer.AcCode) ||
                        await _context.CustomerDetails.AnyAsync(x => x.CompanyName == details.CompanyName && x.CompanyName != customer.CompanyName))
                    {
                        return false;
                    }
                }

                customer.AcCode = details.AcCode;
                customer.CompanyName = details.CompanyName;
                customer.Address1 = details.Address1;
                customer.Address2 = details.Address2;
                customer.Town = details.Town;
                customer.Country = details.Country;
                customer.PostalCode = details.PostalCode;
                customer.TelePhone = details.TelePhone;
                customer.Email = details.Email;
                customer.IsSubscribed = details.IsSubscribed == null ? false : details.IsSubscribed == "UnSubscribed" ? false : true;
                if (details.AcId == 0)
                {
                    customer.Currency = details.Currency;
                    customer.Relation = details.Relation;
                    customer.CreatedDate = DateTime.Now;
                }
                else
                {
                    customer.UpdatedDate = DateTime.Now;
                }
                await _context.SaveChangesAsync();

                details.AcId = customer.AcId;
                if (details.AcId != 0 && details.IsSubscribed != null)
                {
                    await ChangeSubscribtionStatus(details.IsSubscribed, customer.Email ?? "", customer.AcId);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating customer data: {ex.Message}");
                return false;
            }
        }

        public async Task ChangeSubscribtionStatus(string isSubscribed, string email, int customerId)
        {
            List<ContactDetail> contactDetails = await _context.ContactDetails.Where(x => x.Email == email).ToListAsync();
            contactDetails.ForEach(x =>
            {
                x.MailingList = isSubscribed;
            });

            List<CustomerDetail> customerDetails = await _context.CustomerDetails.Where(x => x.Email == email).ToListAsync();
            customerDetails.ForEach(x =>
            {
                x.IsSubscribed = isSubscribed == "UnSubscribed" ? false : true;
            });

            await _context.SaveChangesAsync();
        }

        #endregion


        #region -*- Delete Customer/s -*-

        public async Task DeleteCustomers(List<int> acId)
        {
            List<Mapping> mappings = _context.Mappings.Where(x => acId.Contains(x.CustomerId)).ToList();
            if (mappings.Count > 0)
            {
                _context.Mappings.RemoveRange(mappings);
                await _context.SaveChangesAsync();
            }

            List<SupplierGroup> supplierGroups = await _context.SupplierGroups.Where(x => acId.Contains(x.CustomerId ?? 0)).ToListAsync();
            Parallel.ForEach(supplierGroups, cd => cd.IsDeleted = true);

            //Parallel.ForEach(acId, async id =>
            //{
            //    SupplierGroup? supplier = await _context.SupplierGroups.FirstOrDefaultAsync(y => y.CustomerId == id);
            //    if (supplier != null)
            //    {
            //        supplier.IsDeleted = true;
            //    }
            //});
            await _context.SaveChangesAsync();

            await _context.CustomerDetails.Where(x => acId.Contains(x.AcId)).ExecuteUpdateAsync(x => x.SetProperty(y => y.IsDeleted, y => true));
        }

        #endregion


        #region -*- Get Edit Customer Data -*-

        public async Task<Details> GetEditCustomerDataAsync(int acId)
        {
            CustomerDetail? customer = await _context.CustomerDetails.FirstOrDefaultAsync(x => x.AcId == acId);
            List<SupplierGroup> supplierList = await _context.SupplierGroups.Where(x => x.CustomerId == acId && x.IsDeleted != true).ToListAsync();
            if (customer != null)
            {
                Details editCurtomerData = new()
                {
                    AcId = acId == 0 ? 0 : acId,
                    AcCode = customer == null ? "-" : customer.AcCode,
                    CompanyName = customer == null ? "-" : customer.CompanyName,
                    PostalCode = customer == null ? "-" : customer.PostalCode,
                    Country = customer == null ? "-" : customer.Country,
                    TelePhone = customer == null ? "-" : customer.TelePhone,
                    IsSubscribed = customer == null ? "-" : customer.IsSubscribed != true ? "Unsubscribed" : "Subscribed",
                    Address1 = customer == null ? "-" : customer.Address1,
                    Address2 = customer == null ? "-" : customer.Address2,
                    Email = customer == null ? "-" : customer.Email ?? "",
                    Town = customer == null ? "-" : customer.Town,
                    Currency = customer == null ? "-" : customer.Currency,
                    Relation = customer == null ? "-" : customer.Relation,
                    SupplierList = supplierList.Count == 0 ? "No Supplier Added Yet" : supplierList.Count == 1 ? supplierList.FirstOrDefault(x => x.CustomerId == acId)?.SupplierName : supplierList.FirstOrDefault(x => x.CustomerId == acId)?.SupplierName + " + " + (supplierList.Count - 1) + " more...",
                };
                return editCurtomerData;
            }
            else
            {
                return new Details();
            }
        }

        #endregion


        #region -*- Get Customersssss Data -*-

        public List<CustomerList> GetFilteredCutomer(RequestDTO<CustomerList> requestDTO)
        {
            IEnumerable<CustomerList> recordList = _context.CustomerDetails
               .Where(x => x.IsDeleted != true && (string.IsNullOrEmpty(requestDTO.SearchValue) || x.AcCode.Trim().ToLower().Contains(requestDTO.SearchValue.Trim().ToLower())
                                                                                        || x.CompanyName.Trim().ToLower().Contains(requestDTO.SearchValue.Trim().ToLower())
                                                                                        || x.PostalCode.Trim().ToLower().Contains(requestDTO.SearchValue.Trim().ToLower())
                                                                                        || x.Country.Trim().ToLower().Contains(requestDTO.SearchValue.Trim().ToLower())
                                                                                        || x.TelePhone.Trim().ToLower().Contains(requestDTO.SearchValue.Trim().ToLower())
                                                                                        || x.Relation.Trim().ToLower().Contains(requestDTO.SearchValue.Trim().ToLower())
                                                                                        || x.Currency.Trim().ToLower().Contains(requestDTO.SearchValue.Trim().ToLower()))

                                               && (requestDTO.SearchString.AcCode == null || x.AcCode.Trim().ToLower().Contains(requestDTO.SearchString.AcCode.Trim().ToLower()))
                                               && (requestDTO.SearchString.CompanyName == null || x.CompanyName.Trim().ToLower().Contains(requestDTO.SearchString.CompanyName.Trim().ToLower()))
                                               && (requestDTO.SearchString.PostalCode == null || x.PostalCode.Trim().ToLower().Contains(requestDTO.SearchString.PostalCode.Trim().ToLower()))
                                               && (requestDTO.SearchString.Country == null || x.Country.Trim().ToLower().Contains(requestDTO.SearchString.Country.Trim().ToLower()))
                                               && (requestDTO.SearchString.TelePhone == null || x.TelePhone.Trim().ToLower().Contains(requestDTO.SearchString.TelePhone.Trim().ToLower()))
                                               && (requestDTO.SearchString.Relation == null || x.Relation.Trim().ToLower().Contains(requestDTO.SearchString.Relation.Trim().ToLower()))
                                               && (requestDTO.SearchString.Currency == null || x.Currency.Trim().ToLower().Contains(requestDTO.SearchString.Currency.Trim().ToLower())))
               .Select(cd => new CustomerList
               {
                   AcId = cd.AcId,
                   AcCode = cd.AcCode,
                   CompanyName = cd.CompanyName,
                   PostalCode = cd.PostalCode,
                   Country = cd.Country,
                   TelePhone = cd.TelePhone,
                   Relation = cd.Relation,
                   Currency = cd.Currency
               });

            recordList = requestDTO.SortColumn switch
            {

                1 => (requestDTO.OrderByAsc == "true") ? recordList.AsEnumerable().Select(x => new
                {
                    Record = x,
                    Letters = new string(x.AcCode.Where(char.IsDigit).ToArray()).Length == 0 ? "0" : new string(x.AcCode.Where(char.IsLetter).ToArray()),
                    Numbers = new string(x.AcCode.Where(char.IsDigit).ToArray()),
                })
                .OrderBy(x => x.Letters.Length)
                .ThenBy(x => x.Numbers.Length)
                .ThenBy(x => x.Letters)
                .ThenBy(x => x.Numbers)
                .Select(x => x.Record)
                :
                recordList.AsEnumerable().Select(x => new
                {
                    Record = x,
                    Letters = new string(x.AcCode.Where(char.IsDigit).ToArray()).Length == 0 ? "0" : new string(x.AcCode.Where(char.IsLetter).ToArray()),
                    Numbers = new string(x.AcCode.Where(char.IsDigit).ToArray()),
                })
                .OrderByDescending(x => x.Letters)
                .ThenByDescending(x => x.Letters.Length)
                .ThenByDescending(x => x.Numbers)
                .Select(x => x.Record),

                2 => (requestDTO.OrderByAsc == "true") ? recordList.OrderBy(x => x.CompanyName) : recordList.OrderByDescending(x => x.CompanyName),
                3 => (requestDTO.OrderByAsc == "true") ? recordList.OrderBy(x => x.PostalCode) : recordList.OrderByDescending(x => x.PostalCode),
                4 => (requestDTO.OrderByAsc == "true") ? recordList.OrderBy(x => x.Country) : recordList.OrderByDescending(x => x.Country),
                5 => (requestDTO.OrderByAsc == "true") ? recordList.OrderBy(x => x.TelePhone) : recordList.OrderByDescending(x => x.TelePhone),
                6 => (requestDTO.OrderByAsc == "true") ? recordList.OrderBy(x => x.Relation) : recordList.OrderByDescending(x => x.Relation),
                7 => (requestDTO.OrderByAsc == "true") ? recordList.OrderBy(x => x.Currency) : recordList.OrderByDescending(x => x.Currency),
                _ => recordList.OrderByDescending(x => x.AcId)
            };

            if (requestDTO.PageSize == -1)
            {
                requestDTO.TotalPages = 1;
                requestDTO.TotalRecords = recordList.ToList().Count;
                return recordList.ToList();
            }
            else
            {
                requestDTO.TotalPages = (int)Math.Ceiling((decimal)recordList.ToList().Count / requestDTO.PageSize);
                requestDTO.TotalRecords = recordList.ToList().Count;
                return recordList.Skip((requestDTO.PageNumber - 1) * requestDTO.PageSize).Take(requestDTO.PageSize).ToList();
            }
        }

        #endregion


        #region -*- Customer Groups -*-

        public List<SupplierGroup> GetGroupsOfCustomer(int customerId)
        {
            return _context.Mappings.Include(x => x.SupplierGroup).Where(x => x.CustomerId == customerId && x.SupplierGroup.IsDeleted != true).Select(g => g.SupplierGroup).OrderByDescending(x => x.IsAssigned).ToList();
        }

        public async Task<bool> UpdateCustomerGroupDataAsync(CustomerGroupDTO details)
        {
            if (details.SupplierGroupId == 0)
            {
                bool group = await _context.SupplierGroups.AnyAsync(x => x.SupplierName.Trim().ToLower() == details.SupplierGroupName.Trim().ToLower() && x.CustomerId == details.CustomerId && x.IsDeleted == false);
                if (group)
                {
                    return false;
                }
                else
                {
                    SupplierGroup grp = new()
                    {
                        SupplierName = details.SupplierGroupName,
                        CustomerId = details.CustomerId,
                        CreatedDate = DateTime.Now,
                    };
                    _context.SupplierGroups.Add(grp);
                    await _context.SaveChangesAsync();

                    Mapping mapping = new()
                    {
                        CustomerId = details.CustomerId,
                        SupplierGroupId = grp.SupplierGroupId,
                    };
                    _context.Mappings.Add(mapping);
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            else
            {
                bool checkName = await _context.SupplierGroups.Where(x => x.SupplierGroupId != details.SupplierGroupId && x.SupplierName.Trim().ToLower() == details.SupplierGroupName.Trim().ToLower()).AnyAsync();
                if (checkName)
                {
                    return false;
                }
                else
                {
                    SupplierGroup? existedSupplier = await _context.SupplierGroups.FirstOrDefaultAsync(x => x.SupplierGroupId == details.SupplierGroupId);
                    if (existedSupplier != null)
                    {
                        existedSupplier.SupplierName = details.SupplierGroupName;
                        existedSupplier.ModifiedDate = DateTime.Now;
                        await _context.SaveChangesAsync();
                    }
                    return true;
                }
            }

        }

        public async Task<bool> DeleteCustomerGroupDataAsync(int groupId)
        {
            SupplierGroup? group = await _context.SupplierGroups.FirstOrDefaultAsync(x => x.SupplierGroupId == groupId);
            if (group == null)
            {
                return false;
            }
            else
            {
                if (group.IsDeleted == true)
                {
                    return false;
                }
                else
                {
                    group.IsDeleted = true;
                    await _context.SaveChangesAsync();
                }
                List<Mapping> mappings = _context.Mappings.Where(i => i.SupplierGroupId == groupId).ToList();
                _context.Mappings.RemoveRange(mappings);
                await _context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> AssignCustomerGroupDataAsync(int supplierGroupId)
        {
            SupplierGroup? suppliergroup = await _context.SupplierGroups.FirstOrDefaultAsync(x => x.SupplierGroupId == supplierGroupId);
            if (suppliergroup == null)
            {
                return false;
            }
            else
            {
                if (suppliergroup.IsAssigned == true)
                {
                    suppliergroup.IsAssigned = false;
                }
                else
                {
                    suppliergroup.IsAssigned = true;
                }
                suppliergroup.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();

                return true;
            }
        }

        #endregion


        #region -*- Contact Customers -*-

        public CustomerContactDTO FetchCustomerContact(int acId, int contactId)
        {
            ContactDetail? contactDetail = contactId == 0 ? _context.ContactDetails.Where(x => x.CustomerId == acId).OrderByDescending(x => x.CreatedDate).FirstOrDefault() : _context.ContactDetails.FirstOrDefault(x => x.CustomerId == acId && x.ContactId == contactId);
            List<ContactDetail> contactList = _context.ContactDetails.Where(x => x.CustomerId == acId && x.IsDeleted != true).ToList();

            CustomerContactDTO customerContactDTO = new()
            {
                CustomerId = acId,
                ContactId = contactDetail == null ? 0 : contactDetail.ContactId,
                FullName = contactDetail == null ? "-" : contactDetail.FullName,
                Email = contactDetail == null ? "-" : contactDetail.Email,
                MailingList = contactDetail == null ? "-" : contactDetail.MailingList,
                Telephone = contactDetail == null ? "-" : contactDetail.TelePhone,
                UserName = contactDetail == null ? "-" : contactDetail.UserName,
                ContactList = contactId == 0 ? contactList.Count == 0 ? "No Contact Added Yet" : contactList.Count == 1 ? contactList.Where(x => x.CustomerId == acId).OrderByDescending(x => x.CreatedDate).FirstOrDefault()?.FullName : contactList.Where(x => x.CustomerId == acId).OrderByDescending(x => x.CreatedDate).FirstOrDefault()?.FullName + " + " + (contactList.Count - 1) + " more..." : contactDetail?.FullName,
            };
            return customerContactDTO;
        }

        public List<ContactDetail> GetAllContactOfCustomer(int customerId)
        {
            return _context.ContactDetails.Where(x => x.CustomerId == customerId && x.IsDeleted != true).ToList();
        }

        public async Task<bool> DeleteCustomerContactDataAsync(int contactId)
        {
            ContactDetail? contact = await _context.ContactDetails.FirstOrDefaultAsync(x => x.ContactId == contactId);
            if (contact == null)
            {
                return false;
            }
            else
            {
                if (contact.IsDeleted == true)
                {
                    return false;
                }
                else
                {
                    contact.IsDeleted = true;
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
        }

        public async Task<bool> UpdateCustomerContactDataAsync(CustomerContactDTO details)
        {
            if (details.ContactId == 0)
            {
                bool contact = await _context.ContactDetails.AnyAsync(x => x.FullName.Trim().ToLower() == details.FullName.Trim().ToLower() && x.CustomerId == details.CustomerId && x.IsDeleted == false);
                if (contact)
                {
                    return false;
                }
                else
                {
                    ContactDetail contactDetail = new()
                    {
                        CustomerId = details.CustomerId,
                        FullName = details.FullName,
                        UserName = details.UserName ?? string.Concat(details.FullName.AsSpan(0, 4), "12345"),
                        TelePhone = details.Telephone,
                        Email = details.Email,
                        MailingList = details.MailingList,
                        CreatedDate = DateTime.Now,
                    };
                    _context.ContactDetails.Add(contactDetail);
                    await _context.SaveChangesAsync();
                    details.ContactId = contactDetail.ContactId;
                }
            }
            else
            {
                bool checkName = await _context.ContactDetails.Where(x => x.ContactId != details.ContactId && x.FullName.Trim().ToLower() == details.FullName.Trim().ToLower() && x.CustomerId == details.CustomerId).AnyAsync();
                if (checkName)
                {
                    return false;
                }
                else
                {
                    ContactDetail? existedContact = await _context.ContactDetails.FirstOrDefaultAsync(x => x.ContactId == details.ContactId && x.CustomerId == details.CustomerId);
                    if (existedContact != null)
                    {
                        existedContact.FullName = details.FullName ?? existedContact.FullName;
                        existedContact.UserName = details.UserName ?? existedContact.UserName;
                        existedContact.TelePhone = details.Telephone ?? existedContact.TelePhone;
                        existedContact.Email = details.Email;
                        existedContact.MailingList = details.MailingList ?? existedContact.MailingList;
                        existedContact.ModifiedDate = DateTime.Now;

                    }
                    await _context.SaveChangesAsync();
                }
            }
            if (details.CustomerId != 0 && details.MailingList != null)
            {
                await ChangeSubscribtionStatus(details.MailingList, details.Email ?? "", details.CustomerId);
            }
            return true;
        }

        #endregion


        #region -*- Check Entity -*-

        public bool CheckEntiry(string input, int callId, int customerId)
        {
            if (callId == 1)
            {
                if (_context.CustomerDetails.Any(x => x.AcCode.Trim().ToLower() == input.Trim().ToLower()))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            if (callId == 2)
            {
                if (_context.CustomerDetails.Any(x => x.CompanyName.Trim().ToLower() == input.Trim().ToLower()))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            if (callId == 3)
            {
                if (_context.ContactDetails.Any(x => x.FullName.Trim().ToLower() == input.Trim().ToLower() && x.CustomerId == customerId))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}