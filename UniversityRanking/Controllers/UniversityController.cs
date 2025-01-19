using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UniversityRanking.Models;
using UniversityRanking.Models.University;


namespace UniversityRanking.Controllers
{
    [Authorize]
    public class UniversityController : Controller
    {
        private readonly UniversityContext _context;

        public UniversityController(UniversityContext context)
        {
            _context = context;
        }

        // GET: Univerrsity
        public async Task<IActionResult> Index(int page = 1, int size = 20)
        {
            return View( PagingListAsync<University>.Create(
                (p, s) => 
                    _context.Universities.Include(u => u.Country)
                        .OrderBy(b => b.UniversityName)
                        .Skip((p - 1) * s)
                        .Take(s)
                        .AsAsyncEnumerable(),
                _context.Universities.Count(),
                page,
                size));
        }
        
        public async Task<IActionResult> ScoresByYear(int universityId, int rankingSystemId, string rankingSystemName , string NameUniversity)
        {
            var scores = await _context.UniversityRankingYears
                .Include(ur => ur.University)
                .Include(ur => ur.RankingCriteria)  
                .ThenInclude(rc => rc.RankingSystem)
                .Where(ur => ur.University.Id == universityId && ur.RankingCriteria.RankingSystemId == rankingSystemId)
                .OrderBy(ur => ur.Year)
                .ToListAsync();
            ViewBag.NameUniversity = NameUniversity;
            ViewBag.NameRankingSystem =  rankingSystemName;
            return View(scores);
        }
        
        [HttpGet]
        public IActionResult CreateRanking(int universityId)
        {
            var university = _context.Universities.FirstOrDefault(u => u.Id == universityId);
            if (university == null)
            {
                return NotFound();
            }

            ViewData["UniversityName"] = university.UniversityName; // Название университета
            ViewData["RankingSystems"] = new SelectList(_context.RankingSystems, "Id", "SystemName"); // Список систем
            return View(new UniversityRankingYear { UniversityId = universityId });
        }

        [HttpGet]
        public JsonResult GetRankingCriteria(int rankingSystemId)
        {
            var criteria = _context.RankingCriteria
                .Where(rc => rc.RankingSystemId == rankingSystemId)
                .Select(rc => new { rc.Id, rc.CriteriaName })
                .ToList();

            return Json(criteria);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRanking(UniversityRankingYear ranking)
        {
            if (ranking.Year <= 2016)
            {
                ModelState.AddModelError("Year", "Year must be above 2016.");
            }

            if (!ModelState.IsValid)
            {
                var university = _context.Universities.FirstOrDefault(u => u.Id == ranking.UniversityId);
                if (university == null)
                {
                    return NotFound();
                }

                ViewData["UniversityName"] = university.UniversityName; // Название университета
                ViewData["RankingSystems"] = new SelectList(_context.RankingSystems, "Id", "SystemName"); // Список систем
                ViewData["RankingCriteria"] = new SelectList(_context.RankingCriteria, "Id", "CriteriaName", ranking.RankingCriteriaId); // Список критериев

                return View(ranking); // Возвращаем форму с ошибками
            }
            
            _context.UniversityRankingYears.Add(ranking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    

        
        // GET: Univerrsity/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var university = await _context.Universities
                .Include(u => u.Country)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (university == null)
            {
                return NotFound();
            }

            return View(university);
        }

        // GET: Univerrsity/Create
        public IActionResult Create()
        {
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Id");
            return View();
        }

        // POST: Univerrsity/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CountryId,UniversityName")] University university)
        {
            if (ModelState.IsValid)
            {
                _context.Add(university);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Id", university.CountryId);
            return View(university);
        }

        // GET: Univerrsity/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var university = await _context.Universities.FindAsync(id);
            if (university == null)
            {
                return NotFound();
            }
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Id", university.CountryId);
            return View(university);
        }

        // POST: Univerrsity/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CountryId,UniversityName")] University university)
        {
            if (id != university.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(university);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UniversityExists(university.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Id", university.CountryId);
            return View(university);
        }

        // GET: Univerrsity/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var university = await _context.Universities
                .Include(u => u.Country)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (university == null)
            {
                return NotFound();
            }

            return View(university);
        }

        // POST: Univerrsity/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var university = await _context.Universities.FindAsync(id);
            if (university != null)
            {
                _context.Universities.Remove(university);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UniversityExists(int id)
        {
            return _context.Universities.Any(e => e.Id == id);
        }
    }
}
