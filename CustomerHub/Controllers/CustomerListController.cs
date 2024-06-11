using CustomerHub.BAL.Interface;
using CustomerHub.DAL.Models;
using CustomerHub.DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace CustomerHub.Controllers
{
    public class CustomerListController : Controller
    {
        private readonly CustomerDbContext _context;
        private readonly ICustomerDashboard _iCustomerRepo;


        public CustomerListController(CustomerDbContext context, ICustomerDashboard icustomerDash)
        {
            _context = context;
            _iCustomerRepo = icustomerDash;
        }

        #region -*- List Tab -*-

        public IActionResult CustomerList(int pageNumber = 1)
        {
            ViewBag.PageNumber = pageNumber;
            return View();
        }

        #endregion


        #region -*- Add Customer -*-

        public IActionResult AddCustomer()
        { 
            return View(); 
        }

        public bool AcCodeCheck(string acCode)
        {
            if (acCode == null) 
            { 
                return false; 
            }
            else if (_iCustomerRepo.CheckEntiry(acCode,1,0) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CompanyCheck(string companyName)
        {
            if (companyName == null)
            {
                return false;
            }
            else if (_iCustomerRepo.CheckEntiry(companyName, 2, 0) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer(Details details)
        {
            if (await _iCustomerRepo.UpdateCustomerDataAsync(details) == true)
            {
                TempData["success"] = "Customer Added Successfully!";
                return Json (new { newId = details.AcId, success = true });
            }
            else
            {
                TempData["error"] = "Customer Already Exists!";
                return Json(new { newId = details.AcId, success = false }); ;
            }
        }

        #endregion


        #region -*- Delete Selected Customer -*-

        public async Task<IActionResult> DeleteSelectedCustomer(List<int> acId)
        {
            await _iCustomerRepo.DeleteCustomers(acId);
            return Ok();
        }

        #endregion


        #region -*- Details Tab -*-

        public async Task<IActionResult> DetailsTab(int id, int pageNumber = 1)
        {
            CustomerDetail? customer = await _context.CustomerDetails.FirstOrDefaultAsync(x => x.AcId == id);
            Details details = new()
            {
                AcId = customer == null ? 0 : id,
                CompanyName = customer == null ? "No Records Found" : customer.CompanyName,
                AcCode = customer == null ? "---" : customer.AcCode,
                PageNumber = pageNumber
            };
            return View(details);
        }

        #endregion


        #region -*- Edit Customer -*-

        public async Task<IActionResult> EditCustomerData(int acId)
        {
            if (acId == 0)
            {
                return Json(new { code = 404, message = "No records found" });
            }
            Details details = await _iCustomerRepo.GetEditCustomerDataAsync(acId);
            return PartialView("_EditDetailsForm", details);
        }

        [HttpPost]
        public async Task<IActionResult> EditCustomerPost(Details details)
        {
            if (await _iCustomerRepo.UpdateCustomerDataAsync(details) == true)
            {
                return Json(new { success = true, acId = details.AcId });
            }
            else
            {
                return Json(new { success = false, acId = details.AcId });
            }
        }

        #endregion


        #region -*- Filter Customer By Input -*-

        [HttpPost]
        public IActionResult FilterCustomersByInput(RequestDTO<CustomerList> requestDTO)
        {
            List<CustomerList> model = _iCustomerRepo.GetFilteredCutomer(requestDTO);
            _ = new ResponseDTO();

            ViewBag.TotalPages = requestDTO.TotalPages;
            ViewBag.PageNumber = requestDTO.PageNumber;
            ViewBag.PageSize = requestDTO.PageSize;
            ViewBag.TotalRecords = requestDTO.TotalRecords;
            ViewBag.StartRecords = (requestDTO.PageNumber - 1) * requestDTO.PageSize + 1;
            ViewBag.EndRecord = (requestDTO.PageNumber - 1) * requestDTO.PageSize + model.Count;

            return PartialView("_CustomerTable", model);
        }

        #endregion


        #region -*- Customer Groups -*- 

        public IActionResult OpenCustomerGroupModal(int customerId, int callId)
        {
            CustomerGroupDTO customerGroupDTO = new()
            {
                CustomerId = customerId,
                SupplierGroups = _iCustomerRepo.GetGroupsOfCustomer(customerId),
                CallId = callId,
            };
            return PartialView("_CustomerGroupModal", customerGroupDTO);
        }

        public IActionResult AddCustomerGroup(int customerId)
        {
            CustomerGroupDTO customerGroupDTO = new()
            {
                CustomerId = customerId,
            };
            return PartialView("_AddCustomerGroupModal", customerGroupDTO);
        }

        [HttpPost]
        public async Task<bool> CreateCustomerGroup(CustomerGroupDTO details)
        {
            if (await _iCustomerRepo.UpdateCustomerGroupDataAsync(details) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpPost]
        public async Task<bool> DeleteCustomerGroup(int groupId)
        {
            if (await _iCustomerRepo.DeleteCustomerGroupDataAsync(groupId) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpPost]
        public async Task<bool> AssignCustomerPost(int supplierGroupId)
        {
            if (await _iCustomerRepo.AssignCustomerGroupDataAsync(supplierGroupId) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpPost]
        public async Task<bool> PostEditCustomerGroup(CustomerGroupDTO customerGroupDTO)
        {
            if (await _iCustomerRepo.UpdateCustomerGroupDataAsync(customerGroupDTO) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion


        #region -*- Customer Contacts -*-

        public IActionResult GetEditContactCustomerData(int acId, int contactId)
        {
            CustomerContactDTO customerContactDTO = _iCustomerRepo.FetchCustomerContact(acId, contactId);

            if (customerContactDTO == null || acId == 0)
            {
                return Json(new { code = 404, message = "No records found About Contact Information" });
            }
            return PartialView("_EditContactForm", customerContactDTO);
        }

        public IActionResult OpenCustomerContactModal(int customerId, int callId)
        {
            CustomerContactDTO customerContactDTO = new()
            {
                ContactDetails = _iCustomerRepo.GetAllContactOfCustomer(customerId),
                CallId = callId,
                CustomerId = customerId,
            };

            return PartialView("_CustomerContactModal", customerContactDTO);
        }

        public IActionResult AddCustomerContact(int customerId)
        {
            CustomerContactDTO customerContactDTO = new()
            {
                CustomerId = customerId,
            };
            return PartialView("_AddCustomerContactModal", customerContactDTO);
        }

        public bool CheckContactName(string fullName, int customerId)
        {
            if (fullName == null)
            {
                return false;
            }
            else if (_iCustomerRepo.CheckEntiry(fullName, 3, customerId) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomerContact(CustomerContactDTO details)
        {
            if (await _iCustomerRepo.UpdateCustomerContactDataAsync(details) == true)
            {
                return Json(new { success = true, contactId = details.ContactId });
            }
            else
            {
                return Json(new { success = false, contactId = details.ContactId });
            }
        }

        [HttpPost]
        public async Task<bool> DeleteCustomerContact(int contactId)
        {
            if (await _iCustomerRepo.DeleteCustomerContactDataAsync(contactId) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpPost]
        public async Task<bool> PostEditCustomerContact(CustomerContactDTO customerContactDTO)
        {
            if (await _iCustomerRepo.UpdateCustomerContactDataAsync(customerContactDTO) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpPost]
        public async Task<bool> PostEditCustomerContactData(CustomerContactDTO customerContactDTO)
        {
            if (await _iCustomerRepo.UpdateCustomerContactDataAsync(customerContactDTO) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}