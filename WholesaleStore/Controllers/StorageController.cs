using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WholesaleStore;

namespace WholesaleStore.Controllers
{
    public class StorageController : Controller
    {
        private WholesaleStoreContext db = new WholesaleStoreContext();

        // GET: Storage
        public async Task<ActionResult> Index()
        {
            var storages = db.Storages.Include(s => s.Address);
            return View(await storages.ToListAsync());
        }

        // GET: Storage/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Storage storage = await db.Storages.FindAsync(id);
            if (storage == null)
            {
                return HttpNotFound();
            }
            return View(storage);
        }

        // GET: Storage/Create
        public ActionResult Create()
        {
            ViewBag.AddressId = new SelectList(db.Addresses, "Id", "ZipCode");
            return View();
        }

        // POST: Storage/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Number,AddressId,ContactPhone")] Storage storage)
        {
            if (ModelState.IsValid)
            {
                db.Storages.Add(storage);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.AddressId = new SelectList(db.Addresses, "Id", "ZipCode", storage.AddressId);
            return View(storage);
        }

        // GET: Storage/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Storage storage = await db.Storages.FindAsync(id);
            if (storage == null)
            {
                return HttpNotFound();
            }
            ViewBag.AddressId = new SelectList(db.Addresses, "Id", "ZipCode", storage.AddressId);
            return View(storage);
        }

        // POST: Storage/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Number,AddressId,ContactPhone")] Storage storage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(storage).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.AddressId = new SelectList(db.Addresses, "Id", "ZipCode", storage.AddressId);
            return View(storage);
        }

        // GET: Storage/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Storage storage = await db.Storages.FindAsync(id);
            if (storage == null)
            {
                return HttpNotFound();
            }
            return View(storage);
        }

        // POST: Storage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Storage storage = await db.Storages.FindAsync(id);
            db.Storages.Remove(storage);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
