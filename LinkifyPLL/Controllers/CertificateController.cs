using LinkifyBLL.ModelView;
using LinkifyBLL.Services.Abstraction;
using LinkifyDAL.Entities;
using LinkifyDAL.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SempaBLL.Helper;
using System.Security.Claims;

namespace LinkifyPLL.Controllers
{
    [Authorize]
    public class CertificateController : Controller
    {
        private readonly ICertificateService _certificateService;
        private readonly ILogger<CertificateController> _logger;
        private const string CertificatesFolder = "Certificates";

        public CertificateController(
            ICertificateService certificateService,
            ILogger<CertificateController> logger)
        {
            _certificateService = certificateService;
            _logger = logger;
        }

        private string GetCurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        // GET: Certificate/Index
        public async Task<IActionResult> Index()
        {
            try
            {
                var certificates = await _certificateService.GetByUserIdAsync(GetCurrentUserId());
                return View(certificates.Select(MapToViewModel).ToList());
            }
            catch (Exception ex)
            {
                return RedirectToAction();
            }
        }

        // GET: Certificate/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!await _certificateService.ValidateOwnershipAsync(id, userId))
                {
                    TempData["ErrorMessage"] = "You don't have permission to view this certificate";
                    return RedirectToAction(nameof(Index));
                }

                var certificate = await _certificateService.GetByIdAsync(id);
                if (certificate == null)
                {
                    TempData["ErrorMessage"] = "Certificate not found";
                    return RedirectToAction(nameof(Index));
                }

                return View(MapToViewModel(certificate));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving certificate {id}");
                TempData["ErrorMessage"] = "Error retrieving certificate details";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Certificate/Create
        public IActionResult Create()
        {
            return View(new CertificateMV
            {
                UserId = GetCurrentUserId(),
                IssueDate = DateTime.Today
            });
        }

        // POST: Certificate/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CertificateMV viewModel)
        {
            viewModel.UserId = GetCurrentUserId();

            try
            {
                // Validate file upload
                if (viewModel.Certificate == null || viewModel.Certificate.Length == 0)
                {
                    ModelState.AddModelError("CertificateFile", "Certificate file is required");
                    return View(viewModel);
                }

                // Process file upload
                var fileResult = FileManager.UploadFile(CertificatesFolder, viewModel.Certificate);
                if (!fileResult.StartsWith("/"))
                {
                    ModelState.AddModelError("CertificateFile", fileResult);
                    return View(viewModel);
                }
                viewModel.CertificatePath = fileResult;

                // Validate model
                if (viewModel.ExpirationDate.HasValue && viewModel.ExpirationDate < viewModel.IssueDate)
                {
                    ModelState.AddModelError("ExpirationDate", "Expiration date cannot be before issue date");
                    FileManager.RemoveFile(CertificatesFolder, viewModel.CertificatePath);
                    return View(viewModel);
                }

                if (!ModelState.IsValid)
                {
                    if (!string.IsNullOrEmpty(viewModel.CertificatePath))
                        FileManager.RemoveFile(CertificatesFolder, viewModel.CertificatePath);
                    return View(viewModel);
                }

                var certificate = new Certificate(
                    userId: viewModel.UserId,
                    certificatePath: viewModel.CertificatePath,
                    name: viewModel.Name,
                    issuingOrganization: viewModel.IssuingOrganization,
                    issueDate: viewModel.IssueDate,
                    expirationDate: viewModel.ExpirationDate,
                    credentialId: viewModel.CredentialId,
                    credentialUrl: viewModel.CredentialUrl);

                await _certificateService.CreateAsync(certificate);
                TempData["SuccessMessage"] = "Certificate created successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(viewModel.CertificatePath))
                    FileManager.RemoveFile(CertificatesFolder, viewModel.CertificatePath);

                _logger.LogError(ex, "Error creating certificate");
                ModelState.AddModelError("", "An error occurred while creating the certificate");
                return View(viewModel);
            }
        }

        // GET: Certificate/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!await _certificateService.ValidateOwnershipAsync(id, userId))
                {
                    TempData["ErrorMessage"] = "You don't have permission to edit this certificate";
                    return RedirectToAction(nameof(Index));
                }

                var certificate = await _certificateService.GetByIdAsync(id);
                if (certificate == null)
                {
                    TempData["ErrorMessage"] = "Certificate not found";
                    return RedirectToAction(nameof(Index));
                }

                return View(MapToViewModel(certificate));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving certificate {id} for edit");
                TempData["ErrorMessage"] = "Error retrieving certificate for editing";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Certificate/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CertificateMV viewModel)
        {
            if (id != viewModel.Id)
            {
                TempData["ErrorMessage"] = "Certificate ID mismatch";
                return RedirectToAction(nameof(Index));
            }

            viewModel.UserId = GetCurrentUserId();

            try
            {
                // Validate ownership
                if (!await _certificateService.ValidateOwnershipAsync(id, viewModel.UserId))
                {
                    TempData["ErrorMessage"] = "You don't have permission to edit this certificate";
                    return RedirectToAction(nameof(Index));
                }

                var existingCert = await _certificateService.GetByIdAsync(id);
                if (existingCert == null)
                {
                    TempData["ErrorMessage"] = "Certificate not found";
                    return RedirectToAction(nameof(Index));
                }

                string oldFilePath = null;

                // Handle file upload if new file was provided
                if (viewModel.Certificate != null && viewModel.Certificate.Length > 0)
                {
                    var fileResult = FileManager.UploadFile(CertificatesFolder, viewModel.Certificate);
                    if (!fileResult.StartsWith("/"))
                    {
                        ModelState.AddModelError("CertificateFile", fileResult);
                        return View(viewModel);
                    }
                    oldFilePath = existingCert.CertificatePath;
                    viewModel.CertificatePath = fileResult;
                }
                else
                {
                    viewModel.CertificatePath = existingCert.CertificatePath;
                }

                // Validate expiration date
                if (viewModel.ExpirationDate.HasValue && viewModel.ExpirationDate < viewModel.IssueDate)
                {
                    ModelState.AddModelError("ExpirationDate", "Expiration date cannot be before issue date");
                    if (oldFilePath != null) // Clean up new file if validation fails
                        FileManager.RemoveFile(CertificatesFolder, viewModel.CertificatePath);
                    return View(viewModel);
                }

                if (!ModelState.IsValid)
                {
                    if (oldFilePath != null) // Clean up new file if validation fails
                        FileManager.RemoveFile(CertificatesFolder, viewModel.CertificatePath);
                    return View(viewModel);
                }

                existingCert.Edit(
                    viewModel.Name,
                    viewModel.CertificatePath,
                    viewModel.IssuingOrganization,
                    viewModel.IssueDate,
                    viewModel.ExpirationDate,
                    viewModel.CredentialId,
                    viewModel.CredentialUrl,
                    viewModel.Status);

                await _certificateService.UpdateAsync(id, existingCert);

                // Clean up old file after successful update
                if (oldFilePath != null)
                    FileManager.RemoveFile(CertificatesFolder, oldFilePath);

                TempData["SuccessMessage"] = "Certificate updated successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating certificate {id}");
                ModelState.AddModelError("", "An error occurred while updating the certificate");
                return View(viewModel);
            }
        }

        // POST: Certificate/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!await _certificateService.ValidateOwnershipAsync(id, userId))
                {
                    TempData["ErrorMessage"] = "You don't have permission to delete this certificate";
                    return RedirectToAction(nameof(Index));
                }

                var certificate = await _certificateService.GetByIdAsync(id);
                if (certificate == null)
                {
                    TempData["ErrorMessage"] = "Certificate not found";
                    return RedirectToAction(nameof(Index));
                }

                await _certificateService.SoftDeleteAsync(id);

                // Optionally delete the physical file
                // FileManager.RemoveFile(CertificatesFolder, certificate.CertificatePath);

                TempData["SuccessMessage"] = "Certificate deleted successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting certificate {id}");
                TempData["ErrorMessage"] = "Error deleting certificate";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Certificate/Restore/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!await _certificateService.ValidateOwnershipAsync(id, userId))
                {
                    TempData["ErrorMessage"] = "You don't have permission to restore this certificate";
                    return RedirectToAction(nameof(Index));
                }

                await _certificateService.RestoreAsync(id);
                TempData["SuccessMessage"] = "Certificate restored successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error restoring certificate {id}");
                TempData["ErrorMessage"] = "Error restoring certificate";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Certificate/UpdateStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, CertificateStatus status)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!await _certificateService.ValidateOwnershipAsync(id, userId))
                {
                    TempData["ErrorMessage"] = "You don't have permission to update this certificate";
                    return RedirectToAction(nameof(Index));
                }

                await _certificateService.UpdateStatusAsync(id, status);
                TempData["SuccessMessage"] = "Certificate status updated successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating status for certificate {id}");
                TempData["ErrorMessage"] = "Error updating certificate status";
                return RedirectToAction(nameof(Index));
            }
        }

        private CertificateMV MapToViewModel(Certificate certificate)
        {
            return new CertificateMV
            {
                Id = certificate.Id,
                UserId = certificate.UserId,
                Name = certificate.Name,
                CertificatePath = certificate.CertificatePath,
                IssuingOrganization = certificate.IssuingOrganization,
                IssueDate = certificate.IssueDate,
                ExpirationDate = certificate.ExpirationDate,
                CredentialId = certificate.CredentialId,
                CredentialUrl = certificate.CredentialUrl,
                Status = certificate.Status
            };
        }
    }
}