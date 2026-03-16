using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data;
using Portfolio.Models;
using System.Security.Cryptography;
using System.Text;

namespace Portfolio.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public HomeController(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public IActionResult Index() => View();

        private bool IsAdminAuthed() =>
            HttpContext.Session.GetString("AdminAuth") == "true";

        private static string HashPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes).ToLower();
        }

        [HttpPost]
        [EnableRateLimiting("adminLogin")]
        public IActionResult VerifyAdmin([FromBody] AdminLoginDto dto)
        {
            var correctHash = _config["AdminSettings:PasswordHash"];
            var inputHash = HashPassword(dto.Password ?? "");
            if (inputHash == correctHash)
            {
                HttpContext.Session.SetString("AdminAuth", "true");
                return Json(new { ok = true });
            }
            return Json(new { ok = false });
        }

        [HttpPost]
        public IActionResult AdminLogout()
        {
            HttpContext.Session.Remove("AdminAuth");
            return Json(new { ok = true });
        }

        public IActionResult GetProjects() => Json(_db.Projects.OrderBy(p => p.DisplayOrder).ThenBy(p => p.Id).ToList());
        public IActionResult GetProject(int id) => Json(_db.Projects.Find(id));
        [HttpPost]
        public IActionResult CreateProject([FromBody] Project p)
        { 
            if (!IsAdminAuthed()) return Json(new { ok = false, error = "Unauthorized" }); 
            p.DisplayOrder = _db.Projects.Any() ? _db.Projects.Max(x => x.DisplayOrder) + 1 : 0;
            _db.Projects.Add(p); 
            _db.SaveChanges(); 
            return Json(p); 
        }
        [HttpPost]
        public IActionResult UpdateProject([FromBody] Project p)
        { if (!IsAdminAuthed()) return Json(new { ok = false, error = "Unauthorized" }); _db.Entry(p).State = EntityState.Modified; _db.SaveChanges(); return Json(p); }
        [HttpPost]
        public IActionResult DeleteProject(int id)
        { if (!IsAdminAuthed()) return Json(new { ok = false, error = "Unauthorized" }); var p = _db.Projects.Find(id); if (p != null) { _db.Projects.Remove(p); _db.SaveChanges(); } return Json(new { ok = true }); }

        public IActionResult GetSkills() => Json(_db.Skills.OrderBy(s => s.DisplayOrder).ThenBy(s => s.Id).ToList());
        public IActionResult GetSkill(int id) => Json(_db.Skills.Find(id));
        [HttpPost]
        public IActionResult CreateSkill([FromBody] Skill s)
        { 
            if (!IsAdminAuthed()) return Json(new { ok = false, error = "Unauthorized" }); 
            s.DisplayOrder = _db.Skills.Any() ? _db.Skills.Max(x => x.DisplayOrder) + 1 : 0;
            _db.Skills.Add(s); 
            _db.SaveChanges(); 
            return Json(s); 
        }
        [HttpPost]
        public IActionResult UpdateSkill([FromBody] Skill s)
        { if (!IsAdminAuthed()) return Json(new { ok = false, error = "Unauthorized" }); _db.Entry(s).State = EntityState.Modified; _db.SaveChanges(); return Json(s); }
        [HttpPost]
        public IActionResult DeleteSkill(int id)
        { if (!IsAdminAuthed()) return Json(new { ok = false, error = "Unauthorized" }); var s = _db.Skills.Find(id); if (s != null) { _db.Skills.Remove(s); _db.SaveChanges(); } return Json(new { ok = true }); }

        [HttpPost]
        public IActionResult AssignSkillsToEducation([FromBody] SkillAssignmentDto dto)
        {
            if (!IsAdminAuthed()) return Json(new { ok = false, error = "Unauthorized" });

            // Remove all current skills from this education
            var currentSkills = _db.Skills.Where(s => s.EducationId == dto.EducationId).ToList();
            foreach (var skill in currentSkills)
            {
                skill.EducationId = null;
            }

            // Assign selected skills to this education
            if (dto.SkillIds != null)
            {
                foreach (var skillId in dto.SkillIds)
                {
                    var skill = _db.Skills.Find(skillId);
                    if (skill != null)
                    {
                        skill.EducationId = dto.EducationId;
                    }
                }
            }

            _db.SaveChanges();
            return Json(new { ok = true });
        }

        public IActionResult GetEducations() => Json(_db.Educations.Include(e => e.Skills).OrderBy(e => e.DisplayOrder).ThenBy(e => e.Id).ToList());
        public IActionResult GetEducation(int id) => Json(_db.Educations.Include(e => e.Skills).FirstOrDefault(e => e.Id == id));
        [HttpPost]
        public IActionResult CreateEducation([FromBody] Education e)
        { 
            if (!IsAdminAuthed()) return Json(new { ok = false, error = "Unauthorized" }); 
            e.DisplayOrder = _db.Educations.Any() ? _db.Educations.Max(x => x.DisplayOrder) + 1 : 0;
            _db.Educations.Add(e); 
            _db.SaveChanges(); 
            return Json(e); 
        }
        [HttpPost]
        public IActionResult UpdateEducation([FromBody] Education e)
        {
            if (!IsAdminAuthed()) return Json(new { ok = false, error = "Unauthorized" });
            var existing = _db.Educations.Include(x => x.Skills).FirstOrDefault(x => x.Id == e.Id);
            if (existing == null) return Json(new { ok = false, error = "Not found" });
            existing.Name = e.Name;
            existing.Title = e.Title;
            existing.Description = e.Description;
            existing.Date = e.Date;
            _db.SaveChanges();
            return Json(existing);
        }
        [HttpPost]
        public IActionResult DeleteEducation(int id)
        { if (!IsAdminAuthed()) return Json(new { ok = false, error = "Unauthorized" }); var e = _db.Educations.Find(id); if (e != null) { _db.Educations.Remove(e); _db.SaveChanges(); } return Json(new { ok = true }); }

        public IActionResult GetExperiences() => Json(_db.Experiences.ToList());
        public IActionResult GetExperience(int id) => Json(_db.Experiences.Find(id));
        [HttpPost]
        public IActionResult CreateExperience([FromBody] Experience e)
        { if (!IsAdminAuthed()) return Json(new { ok = false, error = "Unauthorized" }); _db.Experiences.Add(e); _db.SaveChanges(); return Json(e); }
        [HttpPost]
        public IActionResult UpdateExperience([FromBody] Experience e)
        { if (!IsAdminAuthed()) return Json(new { ok = false, error = "Unauthorized" }); _db.Entry(e).State = EntityState.Modified; _db.SaveChanges(); return Json(e); }
        [HttpPost]
        public IActionResult DeleteExperience(int id)
        { if (!IsAdminAuthed()) return Json(new { ok = false, error = "Unauthorized" }); var e = _db.Experiences.Find(id); if (e != null) { _db.Experiences.Remove(e); _db.SaveChanges(); } return Json(new { ok = true }); }

        public IActionResult GetCertificates() => Json(_db.Certificates.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Id).ToList());
        public IActionResult GetCertificate(int id) => Json(_db.Certificates.Find(id));
        [HttpPost]
        public IActionResult CreateCertificate([FromBody] Certificate c)
        { 
            if (!IsAdminAuthed()) return Json(new { ok = false, error = "Unauthorized" }); 
            c.DisplayOrder = _db.Certificates.Any() ? _db.Certificates.Max(x => x.DisplayOrder) + 1 : 0;
            _db.Certificates.Add(c); 
            _db.SaveChanges(); 
            return Json(c); 
        }
        [HttpPost]
        public IActionResult UpdateCertificate([FromBody] Certificate c)
        { if (!IsAdminAuthed()) return Json(new { ok = false, error = "Unauthorized" }); _db.Entry(c).State = EntityState.Modified; _db.SaveChanges(); return Json(c); }
        [HttpPost]
        public IActionResult DeleteCertificate(int id)
        { if (!IsAdminAuthed()) return Json(new { ok = false, error = "Unauthorized" }); var c = _db.Certificates.Find(id); if (c != null) { _db.Certificates.Remove(c); _db.SaveChanges(); } return Json(new { ok = true }); }

        [HttpPost]
        public IActionResult SendContact([FromBody] Contact model)
        { _db.Contacts.Add(model); _db.SaveChanges(); return Json(new { ok = true }); }

        public IActionResult GetCvUrl()
        {
            var url = _config["AdminSettings:CvUrl"] ?? "#";
            return Json(new { url });
        }

        [HttpPost]
        public IActionResult UpdateCvUrl([FromBody] CvUrlDto dto)
        {
            if (!IsAdminAuthed()) return Json(new { ok = false, error = "Unauthorized" });
            // update appsettings.json at runtime
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            var json = System.IO.File.ReadAllText(path);
            var obj = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.Nodes.JsonObject>(json)!;
            obj["AdminSettings"]!["CvUrl"] = dto.Url;
            System.IO.File.WriteAllText(path, obj.ToJsonString(new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
            return Json(new { ok = true });
        }

        // Reorder endpoints
        [HttpPost]
        public IActionResult ReorderProject([FromBody] ReorderDto dto)
        {
            if (!IsAdminAuthed()) return Json(new { ok = false, error = "Unauthorized" });
            var item = _db.Projects.Find(dto.Id);
            if (item == null) return Json(new { ok = false, error = "Not found" });

            var items = _db.Projects.OrderBy(p => p.DisplayOrder).ThenBy(p => p.Id).ToList();
            var currentIndex = items.FindIndex(p => p.Id == dto.Id);

            if (dto.Direction == "up" && currentIndex > 0)
            {
                // Swap with previous item
                var prevItem = items[currentIndex - 1];
                var tempOrder = item.DisplayOrder;
                item.DisplayOrder = prevItem.DisplayOrder;
                prevItem.DisplayOrder = tempOrder;

                // If both have same order, adjust
                if (item.DisplayOrder == prevItem.DisplayOrder)
                {
                    item.DisplayOrder = currentIndex - 1;
                    prevItem.DisplayOrder = currentIndex;
                }
            }
            else if (dto.Direction == "down" && currentIndex < items.Count - 1)
            {
                // Swap with next item
                var nextItem = items[currentIndex + 1];
                var tempOrder = item.DisplayOrder;
                item.DisplayOrder = nextItem.DisplayOrder;
                nextItem.DisplayOrder = tempOrder;

                // If both have same order, adjust
                if (item.DisplayOrder == nextItem.DisplayOrder)
                {
                    item.DisplayOrder = currentIndex + 1;
                    nextItem.DisplayOrder = currentIndex;
                }
            }

            _db.SaveChanges();
            return Json(new { ok = true });
        }

        [HttpPost]
        public IActionResult ReorderSkill([FromBody] ReorderDto dto)
        {
            if (!IsAdminAuthed()) return Json(new { ok = false, error = "Unauthorized" });
            var item = _db.Skills.Find(dto.Id);
            if (item == null) return Json(new { ok = false, error = "Not found" });

            var items = _db.Skills.OrderBy(s => s.DisplayOrder).ThenBy(s => s.Id).ToList();
            var currentIndex = items.FindIndex(s => s.Id == dto.Id);

            if (dto.Direction == "up" && currentIndex > 0)
            {
                var prevItem = items[currentIndex - 1];
                var tempOrder = item.DisplayOrder;
                item.DisplayOrder = prevItem.DisplayOrder;
                prevItem.DisplayOrder = tempOrder;

                if (item.DisplayOrder == prevItem.DisplayOrder)
                {
                    item.DisplayOrder = currentIndex - 1;
                    prevItem.DisplayOrder = currentIndex;
                }
            }
            else if (dto.Direction == "down" && currentIndex < items.Count - 1)
            {
                var nextItem = items[currentIndex + 1];
                var tempOrder = item.DisplayOrder;
                item.DisplayOrder = nextItem.DisplayOrder;
                nextItem.DisplayOrder = tempOrder;

                if (item.DisplayOrder == nextItem.DisplayOrder)
                {
                    item.DisplayOrder = currentIndex + 1;
                    nextItem.DisplayOrder = currentIndex;
                }
            }

            _db.SaveChanges();
            return Json(new { ok = true });
        }

        [HttpPost]
        public IActionResult ReorderEducation([FromBody] ReorderDto dto)
        {
            if (!IsAdminAuthed()) return Json(new { ok = false, error = "Unauthorized" });
            var item = _db.Educations.Find(dto.Id);
            if (item == null) return Json(new { ok = false, error = "Not found" });

            var items = _db.Educations.OrderBy(e => e.DisplayOrder).ThenBy(e => e.Id).ToList();
            var currentIndex = items.FindIndex(e => e.Id == dto.Id);

            if (dto.Direction == "up" && currentIndex > 0)
            {
                var prevItem = items[currentIndex - 1];
                var tempOrder = item.DisplayOrder;
                item.DisplayOrder = prevItem.DisplayOrder;
                prevItem.DisplayOrder = tempOrder;

                if (item.DisplayOrder == prevItem.DisplayOrder)
                {
                    item.DisplayOrder = currentIndex - 1;
                    prevItem.DisplayOrder = currentIndex;
                }
            }
            else if (dto.Direction == "down" && currentIndex < items.Count - 1)
            {
                var nextItem = items[currentIndex + 1];
                var tempOrder = item.DisplayOrder;
                item.DisplayOrder = nextItem.DisplayOrder;
                nextItem.DisplayOrder = tempOrder;

                if (item.DisplayOrder == nextItem.DisplayOrder)
                {
                    item.DisplayOrder = currentIndex + 1;
                    nextItem.DisplayOrder = currentIndex;
                }
            }

            _db.SaveChanges();
            return Json(new { ok = true });
        }

        [HttpPost]
        public IActionResult ReorderCertificate([FromBody] ReorderDto dto)
        {
            if (!IsAdminAuthed()) return Json(new { ok = false, error = "Unauthorized" });
            var item = _db.Certificates.Find(dto.Id);
            if (item == null) return Json(new { ok = false, error = "Not found" });

            var items = _db.Certificates.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Id).ToList();
            var currentIndex = items.FindIndex(c => c.Id == dto.Id);

            if (dto.Direction == "up" && currentIndex > 0)
            {
                var prevItem = items[currentIndex - 1];
                var tempOrder = item.DisplayOrder;
                item.DisplayOrder = prevItem.DisplayOrder;
                prevItem.DisplayOrder = tempOrder;

                if (item.DisplayOrder == prevItem.DisplayOrder)
                {
                    item.DisplayOrder = currentIndex - 1;
                    prevItem.DisplayOrder = currentIndex;
                }
            }
            else if (dto.Direction == "down" && currentIndex < items.Count - 1)
            {
                var nextItem = items[currentIndex + 1];
                var tempOrder = item.DisplayOrder;
                item.DisplayOrder = nextItem.DisplayOrder;
                nextItem.DisplayOrder = tempOrder;

                if (item.DisplayOrder == nextItem.DisplayOrder)
                {
                    item.DisplayOrder = currentIndex + 1;
                    nextItem.DisplayOrder = currentIndex;
                }
            }

            _db.SaveChanges();
            return Json(new { ok = true });
        }

        // Initialize DisplayOrder for existing records
        [HttpPost]
        public IActionResult InitializeDisplayOrders()
        {
            if (!IsAdminAuthed()) return Json(new { ok = false, error = "Unauthorized" });

            // Projects
            var projects = _db.Projects.OrderBy(p => p.Id).ToList();
            for (int i = 0; i < projects.Count; i++)
            {
                projects[i].DisplayOrder = i;
            }

            // Skills
            var skills = _db.Skills.OrderBy(s => s.Id).ToList();
            for (int i = 0; i < skills.Count; i++)
            {
                skills[i].DisplayOrder = i;
            }

            // Educations
            var educations = _db.Educations.OrderBy(e => e.Id).ToList();
            for (int i = 0; i < educations.Count; i++)
            {
                educations[i].DisplayOrder = i;
            }

            // Certificates
            var certificates = _db.Certificates.OrderBy(c => c.Id).ToList();
            for (int i = 0; i < certificates.Count; i++)
            {
                certificates[i].DisplayOrder = i;
            }

            _db.SaveChanges();
            return Json(new { ok = true });
        }


    }
}